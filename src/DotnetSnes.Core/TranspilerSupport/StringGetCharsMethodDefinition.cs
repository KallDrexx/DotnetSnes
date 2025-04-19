using Dntc.Common;
using Dntc.Common.Definitions;
using Dntc.Common.Syntax.Statements;

namespace DotnetSnes.Core.TranspilerSupport;

/// <summary>
/// Allows C code to get a character from a string by index. Needed since .net doesn't do a normal index
/// operation, but instead uses the `get_Chars()` method call.
/// </summary>
public class StringGetCharsMethodDefinition : CustomDefinedMethod
{
    private const string MacroName = "getStringChars";

    public StringGetCharsMethodDefinition()
        : base(new IlMethodId("System.Char System.String::get_Chars(System.Int32)"),
            new IlTypeName("System.Char"),
            new IlNamespace("System"),
            new HeaderName("system.h"),
            new CSourceFileName("system.c"),
            new CFunctionName(MacroName),
            [new Parameter(new IlTypeName("System.Int32"), "index", false)])
    {
    }

    public override CustomCodeStatementSet? GetCustomDeclaration()
    {
        return new CustomCodeStatementSet($"#define {MacroName}(string,index) ((string)[(index)])");
    }

    public override CustomCodeStatementSet? GetCustomImplementation()
    {
        return null;
    }
}