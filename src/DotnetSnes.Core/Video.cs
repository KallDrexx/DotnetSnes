using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Video
{
    [NativeFunctionCall("setScreenOn", Constants.HeaderFile)]
    public static void SetScreenOn() { }

    [NativeFunctionCall("setMode", Constants.HeaderFile)]
    public static void SetMode(BackgroundMode mode, byte size) { }
}