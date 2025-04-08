using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Misc
{
    [NativeFunctionCall("rand", Constants.HeaderFile)]
    public static ushort Random()
    {
        return 0;
    }
}