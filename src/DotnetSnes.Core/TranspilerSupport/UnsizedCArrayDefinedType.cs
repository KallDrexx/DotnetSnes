using Dntc.Common;
using Dntc.Common.Conversion;
using Dntc.Common.Definitions.CustomDefinedTypes;
using Dntc.Common.Syntax.Expressions;
using Dntc.Common.Syntax.Statements;

namespace DotnetSnes.Core.TranspilerSupport;

/// <summary>
/// Allows defining types for unsized C arrays passed around as pointers.
/// </summary>
public class UnsizedCArrayDefinedType : ArrayDefinedType
{
    public UnsizedCArrayDefinedType(
        IlTypeName elementType,
        IlTypeName arrayTypeName) 
        : base(elementType, arrayTypeName, null, null, [])
    {
    }

    public override CustomCodeStatementSet? GetCustomTypeDeclaration(ConversionCatalog catalog)
    {
        // No direct type needs to be defined, as this relies on the element type to be
        // defined.
        return null;
    }

    public override CBaseExpression? GetArraySizeExpression(
        CBaseExpression expressionToArray,
        ConversionCatalog conversionCatalog)
    {
        return null;
    }

    public override CBaseExpression GetItemsAccessorExpression(
        CBaseExpression expressionToArray,
        ConversionCatalog conversionCatalog)
    {
        // The array index can go against the array itself
        return expressionToArray;
    }

    public override CStatementSet GetLengthCheckExpression(
        CBaseExpression arrayLengthField,
        CBaseExpression arrayInstance,
        CBaseExpression index)
    {
        return new CustomCodeStatementSet(""); // We can't check length for these
    }

    public override CTypeName FormTypeName(TypeConversionInfo elementTypeInfo)
    {
        return elementTypeInfo.NameInC;
    }
}