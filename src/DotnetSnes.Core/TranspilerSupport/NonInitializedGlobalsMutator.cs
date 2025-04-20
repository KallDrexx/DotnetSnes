using System.Collections.Generic;
using Dntc.Common;
using Dntc.Common.Conversion;
using Dntc.Common.Conversion.Mutators;
using Mono.Cecil;

namespace DotnetSnes.Core.TranspilerSupport;

/// <summary>
/// The compiler that pvsneslib uses does not support the `= {0};` syntax for globals.
/// So we need to make sure those are not included.
/// </summary>
public class NonInitializedGlobalsMutator : IFieldConversionMutator
{
    public IReadOnlySet<IlTypeName> RequiredTypes => new HashSet<IlTypeName>();

    public void Mutate(FieldConversionInfo conversionInfo, FieldDefinition fieldDefinition)
    {
        if (fieldDefinition.IsStatic && conversionInfo.InitialValue == null)
        {
            conversionInfo.HasNoInitialValue = true;
        }
    }
}