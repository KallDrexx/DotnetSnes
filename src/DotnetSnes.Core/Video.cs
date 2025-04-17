using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Video
{
    [NativeFunctionCall("setScreenOn", Constants.HeaderFile)]
    public static void SetScreenOn() { }

    [NativeFunctionCall("setMode", Constants.HeaderFile)]
    public static void SetMode(BackgroundMode mode, byte size) { }

    /// <summary>
    /// Sets the brightness of the screen
    /// </summary>
    /// <param name="level">0 = black, 15 = full brightness</param>
    [NativeFunctionCall("setBrightness", Constants.HeaderFile)]
    public static void SetBrightness(byte level)
    {

    }
}