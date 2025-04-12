using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Sprite
{
    public enum SpriteSize
    {
        Small = 0,
        Large = 1,
    }

    /// <summary>
    /// Current sprite buffer for dynamic engine
    /// </summary>
    [NativeGlobal("oambuffer", Constants.HeaderFile)]
    public static SpriteDefinition[] Buffer;

    /// <summary>
    /// Initialize the dynamic sprite engine with each sprite size entries
    /// </summary>
    /// <param name="largeGraphicsEntryAddress">
    /// Address of large sprite graphics entry
    /// </param>
    /// <param name="smallGraphicsEntryAddress">
    /// Address of small sprite graphics entry
    /// </param>
    /// <param name="largeSpriteNumberAddress">
    /// Address of large sprite number (useful when we have some hud sprites which are not update each frame)
    /// </param>
    /// <param name="smallSpriteNumberAddress">
    /// Address of small sprite number (useful when we have some hud sprites which are not update each frame)
    /// </param>
    /// <param name="oamSize">default OAM size (64px size not supported)</param>
    [NativeFunctionCall("oamInitDynamicSprite", Constants.HeaderFile)]
    public static void InitDynamicSprite(
        ushort largeGraphicsEntryAddress,
        ushort smallGraphicsEntryAddress,
        ushort largeSpriteNumberAddress,
        ushort smallSpriteNumberAddress,
        OamSize oamSize) { }

    /// <summary>
    /// Must be called at the end of the frame, initialize the dynamic sprite engine
    /// for the next frame.
    /// </summary>
    [NativeFunctionCall("oamInitDynamicSpriteEndFrame", Constants.HeaderFile)]
    public static void InitDynamicSpriteEndFrame() {}

    /// <summary>
    /// Update VRAM graphics for sprites 32x32, 16x16, and 8x8 (can but call in Vblank if needed)
    /// </summary>
    [NativeFunctionCall("oamVramQueueUpdate", Constants.HeaderFile)]
    public static void VramQueueUpdate() { }

    /// <summary>
    /// Sets an oam entry to the supplied values
    /// </summary>
    /// <param name="id">The oam number to be set [0 -127] * 4 because of oam structure</param>
    /// <param name="xLocation">X location of the sprite in pixels</param>
    /// <param name="yLocation">Y location of the sprite in pixels</param>
    /// <param name="priority">Sprite priority (0 to 3)</param>
    /// <param name="horizontallyFlip">Flip the sprite horizontally</param>
    /// <param name="verticallyFlip">Flip the sprite vertically</param>
    /// <param name="gfxOffset">Tile number graphic offset</param>
    /// <param name="paletteOffset">Palette default offset for sprite</param>
    [NativeFunctionCall("oamSet", Constants.HeaderFile)]
    public static void Set(
        ushort id,
        ushort xLocation,
        ushort yLocation,
        byte priority,
        byte horizontallyFlip,
        byte verticallyFlip,
        ushort gfxOffset,
        byte paletteOffset)
    {

    }

    /// <summary>
    /// Put the correct size and hide/show a sprite
    /// </summary>
    /// <param name="id">Oam number to be set [0 - 127] * 4 because of oam structure</param>
    /// <param name="size"></param>
    /// <param name="visibility">
    /// Visibility of the sprite. When hidden, the sprite is set at the coordinates x = 255, y = 240. Therefore,
    /// when a sprite is shown again a call to set its coordinates will be required.
    /// </param>
    [NativeFunctionCall("oamSetEx", Constants.HeaderFile)]
    public static void SetExtendedProperties(ushort id, SpriteSize size, OamVisibility visibility)
    {

    }

    /// <summary>
    /// Initializes a sprites Gfx and Loads the GFX into VRAM
    /// </summary>
    /// <param name="tileSource">Address of sprites graphic entry</param>
    /// <param name="tileSize">Size of sprites graphic</param>
    /// <param name="tilePalette">Address of sprites palette entry</param>
    /// <param name="paletteSize">Size of the palette</param>
    /// <param name="tilePaletteNumber">Palette number (0..8)</param>
    /// <param name="address">Address of sprite graphics (8K-word steps)</param>
    /// <param name="oamSize">Default OAM size</param>
    [NativeFunctionCall("oamInitGfxSet", Constants.HeaderFile)]
    public static void InitGfxSet(
        ref byte tileSource,
        ushort tileSize,
        ref byte tilePalette,
        ushort paletteSize,
        byte tilePaletteNumber,
        ushort address,
        OamSize oamSize)
    {

    }

    /// <summary>
    /// Add a 16x16 sprite on screen.oambuffer[id] needs to be populate before.
    /// </summary>
    /// <param name="id">d the oam number to be used  [0 - 127]</param>
    [NativeFunctionCall("oamDynamic16Draw", Constants.HeaderFile)]
    public static void Dynamic16Draw(ushort id)
    {

    }
}