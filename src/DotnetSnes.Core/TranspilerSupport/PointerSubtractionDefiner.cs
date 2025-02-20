using System.Collections.Generic;
using System.Linq;
using Dntc.Common;
using Dntc.Common.Definitions;
using Dntc.Common.Definitions.Definers;
using Dntc.Common.Syntax.Statements;
using Mono.Cecil;

namespace DotnetSnes.Core.TranspilerSupport;

public class PointerSubtractionDefiner : IDotNetMethodDefiner
{
    public DefinedMethod? Define(MethodDefinition method)
    {
        if (method.DeclaringType.FullName != typeof(CUtils).FullName)
        {
            return null;
        }

        if (method.Name != nameof(CUtils.BytesBetweenAddress))
        {
            return null;
        }

        return new PointerSubDefinedMethod(
            new IlMethodId(method.FullName),
            new IlTypeName(method.ReturnType.FullName),
            Utils.GetNamespace(method.DeclaringType),
            method.Parameters
                .Select(x => new DefinedMethod.Parameter(new IlTypeName(x.ParameterType.FullName), x.Name, true))
                .ToArray());
    }

    private class PointerSubDefinedMethod : CustomDefinedMethod
    {
        public PointerSubDefinedMethod(
            IlMethodId methodId, 
            IlTypeName returnType, 
            IlNamespace ilNamespace, 
            IReadOnlyList<Parameter> parameters)
            : base(methodId, returnType, ilNamespace, Utils.GetHeaderName(ilNamespace), Utils.GetSourceFileName(ilNamespace), new CFunctionName("subtractPointers"), parameters)
        {
        }

        public override CustomCodeStatementSet? GetCustomDeclaration()
        {
            var content = $"#define ${NativeName}(a,b) ((a) - (b))";

            return new CustomCodeStatementSet(content);
        }

        public override CustomCodeStatementSet? GetCustomImplementation()
        {
            return null;
        }
    }
}