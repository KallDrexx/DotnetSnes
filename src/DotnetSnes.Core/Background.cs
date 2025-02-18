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

    /// <summary>
    /// Initializes a tile set and loads a tile GFX compressed with LZ algorithm into VRAM
    /// </summary>
    /// <param name="backgroundNumber">Background number (0 to 3)</param>
    /// <param name="tileSource">Address of the tile graphics entry compressed with the LZ algorithm</param>
    /// <param name="tilePalette">Address of the palette entry</param>
    /// <param name="paletteEntry">Palette number (0..16 for 16 colors mode) of the beginning of each colors</param>
    /// <param name="paletteSize">Size of the palette</param>
    /// <param name="colorMode">Used for correct palette entry (BG_4COLORS0, BG_16COLORS, BG_256COLORS)</param>
    /// <param name="address">Address of tile graphics (4K aligned)</param>
    public static void InitTileSet(
        byte backgroundNumber,
        ref byte tileSource,
        ref byte tilePalette,
        byte paletteEntry,
        ushort paletteSize,
        ushort colorMode,
        ushort address) {}
}