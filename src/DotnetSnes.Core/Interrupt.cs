using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Interrupt
{
    [NativeFunctionCall("WaitForVBlank", Constants.HeaderFile)]
    public static void WaitForVBlank() { }
}