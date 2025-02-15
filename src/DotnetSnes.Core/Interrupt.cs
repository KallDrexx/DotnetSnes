using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Interrupt
{
    [NativeFunctionCall("waitForVBlank", Constants.HeaderFile)]
    public static void WaitForVBlank() { }
}