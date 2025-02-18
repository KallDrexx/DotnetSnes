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
    public static void InitDynamicSprite(
        ushort largeGraphicsEntryAddress,
        ushort smallGraphicsEntryAddress,
        ushort largeSpriteNumberAddress,
        ushort smallSpriteNumberAddress,
        OamSize oamSize) { }
}