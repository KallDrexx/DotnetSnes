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
            var code = attribute.ConstructorArguments[0].Value.ToString()!;

            return new MacroDefinition(
                code,
                new CFunctionName(name),
                new IlMethodId(method.FullName),
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

        public MacroDefinition(
            string code,
            CFunctionName macroName,
            IlMethodId methodId,
            IlTypeName returnType,
            IlNamespace ilNamespace,
            IReadOnlyList<Parameter> parameters)
            : base(methodId, returnType, ilNamespace, Utils.GetHeaderName(ilNamespace), Utils.GetSourceFileName(ilNamespace), macroName, parameters)
        {
            _code = code;
        }

        public override CustomCodeStatementSet? GetCustomDeclaration()
        {
            return new CustomCodeStatementSet(_code);
        }

        public override CustomCodeStatementSet? GetCustomImplementation()
        {
            return null;
        }
    }
}