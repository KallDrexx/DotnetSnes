using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class SnesConsole
{
    [NativeFunctionCall("consoleSetTextVramBGAdr", Constants.HeaderFile)]
    public static void SetTextVramBgAddress(short offsetFont) { }

    [NativeFunctionCall("consoleSetTextVramAdr", Constants.HeaderFile)]
    public static void SetTextVramAddress(short vramFont) { }

    [NativeFunctionCall("consoleSetTextOffset", Constants.HeaderFile)]
    public static void SetTextOffset(short offsetFont) { }

    [NativeFunctionCall("consoleInitText", Constants.HeaderFile)]
    public static void InitText(byte paletteEntry, byte paletteSize, ref byte tileFont, ref byte paletteFont) { }

    [NativeFunctionCall("consoleDrawText", Constants.HeaderFile)]
    public static void DrawText(short x, short y, string text) { }
}