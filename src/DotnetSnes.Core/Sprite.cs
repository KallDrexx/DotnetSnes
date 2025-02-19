using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Sprite
{
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
}