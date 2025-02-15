using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Background
{
    [NativeFunctionCall("bgSetDisable", Constants.HeaderFile)]
    public static void Disable(byte backgroundNumber) { }

    [NativeFunctionCall("bgSetGfxPtr", Constants.HeaderFile)]
    public static void SetGfxPointer(byte backgroundNumber, short address) { }

    [NativeFunctionCall("bgSetMapPtr", Constants.HeaderFile)]
    public static void SetMapPointer(byte backgroundNumber, short address, MapSizes mapSize) { }
}