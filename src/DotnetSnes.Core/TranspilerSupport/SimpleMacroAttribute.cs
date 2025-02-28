using System;
using System.Collections.Generic;
using System.Linq;
using Dntc.Common;
using Dntc.Common.Definitions;
using Dntc.Common.Definitions.Definers;
using Dntc.Common.Syntax.Statements;
using Mono.Cecil;

namespace DotnetSnes.Core.TranspilerSupport;

[AttributeUsage(AttributeTargets.Method)]
public class SimpleMacroAttribute(string name, string code) : Attribute
{
    public class Definer : IDotNetMethodDefiner
    {
        public DefinedMethod? Define(MethodDefinition method)
        {
            var attribute = method.CustomAttributes
                .SingleOrDefault(x => x.AttributeType.FullName == typeof(SimpleMacroAttribute).FullName);

            if (attribute == null)
            {
                return null;
            }

            var name = attribute.ConstructorArguments[0].Value.ToString()!;
            var code = attribute.ConstructorArguments[1].Value.ToString()!;

            // TODO: Generic methods need their ids normalized. There should be an easier way
            // to do this so you can't forget to do so in a custom definer.
            var methodId = method.HasGenericParameters
                ? Utils.NormalizeGenericMethodId(method.FullName, method.GenericParameters)
                : new IlMethodId(method.FullName);

            return new MacroDefinition(
                code,
                new CFunctionName(name),
                method,
                methodId,
                new IlTypeName(method.ReturnType.FullName),
                Utils.GetNamespace(method.DeclaringType),
                method.Parameters
                    .Select(x => new DefinedMethod.Parameter(new IlTypeName(x.ParameterType.FullName), x.Name, true))
                    .ToArray());
        }
    }

    private class MacroDefinition : CustomDefinedMethod
    {
        private readonly string _code;
        private readonly MethodDefinition _methodDefinition;

        public MacroDefinition(
            string code,
            CFunctionName macroName,
            MethodDefinition methodDefinition,
            IlMethodId methodId,
            IlTypeName returnType,
            IlNamespace ilNamespace,
            IReadOnlyList<Parameter> parameters)
            : base(methodId, returnType, ilNamespace, Utils.GetHeaderName(ilNamespace),
                Utils.GetSourceFileName(ilNamespace), macroName, parameters)
        {
            _code = code;
            _methodDefinition = methodDefinition;
        }

        public override CustomCodeStatementSet? GetCustomDeclaration()
        {
            return new CustomCodeStatementSet(_code);
        }

        public override CustomCodeStatementSet? GetCustomImplementation()
        {
            return null;
        }

        public override DefinedMethod MakeGenericInstance(
            IlMethodId methodId,
            IReadOnlyList<IlTypeName> genericArguments)
        {
            // TODO: this is common code that should be moved into dntc
            var newParameters = new List<Parameter>();
            foreach (var parameter in Parameters)
            {
                var genericIndex = (int?)null;
                for (var x = 0; x < _methodDefinition.GenericParameters.Count; x++)
                {
                    var generic = _methodDefinition.GenericParameters[x];
                    if (generic.FullName == parameter.Type.GetNonPointerOrRef().Value)
                    {
                        genericIndex = x;
                        break;
                    }
                }

                if (genericIndex != null)
                {
                    newParameters.Add(parameter with { Type = genericArguments[genericIndex.Value] });
                }
                else
                {
                    newParameters.Add(parameter);
                }
            }

            var returnType = ReturnType.GetNonPointerOrRef();
            for (var x = 0; x < _methodDefinition.GenericParameters.Count; x++)
            {
                if (_methodDefinition.GenericParameters[x].FullName == returnType.Value)
                {
                    returnType = genericArguments[x];
                }
            }

            if (ReturnType.IsPointer())
            {
                returnType = returnType.AsPointerType();
            }

            return new MacroDefinition(_code, NativeName, _methodDefinition, methodId, returnType, Namespace, newParameters);
        }
    }
}