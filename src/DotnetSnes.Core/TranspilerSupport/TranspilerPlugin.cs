using Dntc.Common;
using Dntc.Common.Definitions;
using Dntc.Common.Definitions.CustomDefinedTypes;

namespace DotnetSnes.Core.TranspilerSupport;

public class TranspilerPlugin : ITranspilerPlugin
{
    public bool BypassBuiltInNativeDefinitions => true;

    public void Customize(TranspilerContext context)
    {
        AddNativeTypes(context);
        context.Definers.Append(new AssemblyLabelAttribute.Definer());
        context.ConversionInfoCreator.AddFieldMutator(new NonInitializedGlobalsMutator());
    }

    private static void AddNativeTypes(TranspilerContext context)
    {
        context.DefinitionCatalog.Add([
            new NativeDefinedType(
                new IlTypeName(typeof(void).FullName!),
                null,
                new CTypeName("void"),
                []),

            new NativeDefinedType(
                new IlTypeName(typeof(string).FullName!),
                null,
                new CTypeName("char*"),
                []),

            new NativeDefinedType(
                new IlTypeName(typeof(int).FullName!),
                new HeaderName(Constants.HeaderFile),
                new CTypeName("s32"),
                []),

            new NativeDefinedType(
                new IlTypeName(typeof(uint).FullName!),
                new HeaderName(Constants.HeaderFile),
                new CTypeName("u32"),
                []),

            new NativeDefinedType(
                new IlTypeName(typeof(byte).FullName!),
                new HeaderName(Constants.HeaderFile),
                new CTypeName("u8"),
                []),

            new NativeDefinedType(
                new IlTypeName(typeof(char).FullName!),
                new HeaderName(Constants.HeaderFile),
                new CTypeName("u8"),
                []),

            new NativeDefinedType(
                new IlTypeName(typeof(sbyte).FullName!),
                new HeaderName(Constants.HeaderFile),
                new CTypeName("s8"),
                []),

            new NativeDefinedType(
                new IlTypeName(typeof(short).FullName!),
                new HeaderName(Constants.HeaderFile),
                new CTypeName("s16"),
                []),

            new NativeDefinedType(
                new IlTypeName(typeof(ushort).FullName!),
                new HeaderName(Constants.HeaderFile),
                new CTypeName("u16"),
                []),

            new NativeDefinedType(
                new IlTypeName(typeof(bool).FullName!),
                new HeaderName(Constants.HeaderFile),
                new CTypeName("unsigned char"),
                []),

            new UnsizedCArrayDefinedType(
                new IlTypeName(typeof(ushort).FullName!),
                new IlTypeName(typeof(ushort[]).FullName!)),

            new UnsizedCArrayDefinedType(
                new IlTypeName(typeof(byte).FullName!),
                new IlTypeName(typeof(byte[]).FullName!)),
        ]);

        context.DefinitionCatalog.Add([new StringGetCharsMethodDefinition()]);
    }
}