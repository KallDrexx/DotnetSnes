using Dntc.Attributes;
using DotnetSnes.Core;

namespace DotnetSnes.Example.LikeMario;

public static unsafe class Game
{
    public static BrrSamples JumpSound;
    public static ushort Pad0;

    /// <summary>
    /// Pointer to the Mario object
    /// </summary>
    public static ObjectDefinition* MarioObject;

    /// <summary>
    /// Pointer to Mario's X coordinates with fixed point
    /// </summary>
    public static short* MarioXCoords;

    /// <summary>
    /// Pointer to Mario's Y coordinates with fixed point
    /// </summary>
    public static short* MarioYCoords;

    /// <summary>
    /// Pointer to Mario's X velocity with fixed point
    /// </summary>
    public static short* MarioXVelocity;

    /// <summary>
    /// Pointer to Mario's Y velocity with fixed point
    /// </summary>
    public static short* MarioYVelocity;

    /// <summary>
    /// Mario's X coordinates with map depth (not only screen)
    /// </summary>
    public static ushort MarioXMapDepth;

    /// <summary>
    /// Mario's Y coordinates with map depth (not only screen)
    /// </summary>
    public static ushort MarioYMapDepth;

    // To manage the sprite display
    public static byte MarioFidx;
    public static byte MarioFlip;
    public static byte Flip;

    [CustomFunctionName("main")]
    public static int Main()
    {
        Sound.Boot(); // Initialize the sound engine

        // Initialize text console with our font
        SnesConsole.SetTextVramBgAddress(0x6000);
        SnesConsole.SetTextVramAddress(0x3000);
        SnesConsole.InitText(1, 16 * 2, ref Globals.SnesFont, ref Globals.SnesPalette);

        Sound.SetBank(ref Globals.SoundBank);
        Sound.AllocateSoundRegion(39); // Allocate around 10k of sound ram (39 256-byte blocks)
        Sound.Load(Globals.OverworldMusic);

        // Load jump sound sample
        var jumpSoundSize = CUtils.BytesBetweenAddress(ref Globals.JumpSoundEnd, ref Globals.JumpSound);
        Sound.SetSoundEntry(15, 8, 6, jumpSoundSize, ref Globals.JumpSound, ref JumpSound);

        // Init layer with tiles and init map length 0x6000 is mandatory for map engine
        var tileSize = CUtils.BytesBetweenAddress(ref Globals.TileSetEnd, ref Globals.TileSetStart);
        Background.SetGfxPointer(1, 0x3000);
        Background.SetMapPointer(1, 0x6000, MapSizes.Size32X32);
        Background.InitTileSet(
            0,
            ref Globals.TileSetStart,
            ref Globals.TileSetPalette,
            0,
            tileSize,
            16 * 2,
            16, // 16 color mode
            0x2000);

        // Init sprite engine (0x000 for large, 0x1000 for small)
        Sprite.InitDynamicSprite(0x000, 0x1000, 0, 0, OamSize.Size8L16);

        SnesObject.InitEngine();
        // TODO: objInitFunctions
        // TODO: objLoadObjects

        // Load map in memory and update it regarding current location of the sprite
        Map.Load(ref Globals.MarioMap, ref Globals.TileSetDefinition, ref Globals.TileSetProperties);

        Video.SetMode(BackgroundMode.Mode1, 0);
        Background.Disable(2);

        SnesConsole.DrawText(6, 16, "Mariox00 WORLD TIME");
        SnesConsole.DrawText(6, 17, " 00000 ox00 1-1  000");
        Video.SetScreenOn();

        Sound.Play(0);
        Sound.SetModuleVolume(100);
        Interrupt.WaitForVBlank();

        while (true)
        {
            Map.Update();
            SnesObject.UpdateAll();

            // Prepare next frame
            Sprite.InitDynamicSpriteEndFrame();
            Sound.Process();
            Interrupt.WaitForVBlank();
            Map.VBlank();
            Sprite.VramQueueUpdate();
        }

        return 0;
    }

    public static void InitializeMario(ushort xPosition, ushort yPosition, ushort type, ushort minX, ushort maxX)
    {
        // Prepare new object
        if (SnesObject.New((byte)type, xPosition, yPosition) == 0)
        {
            // No more space, exit
            return;
        }

        SnesObject.GetPointer(SnesObject.CurrentObjectId);
        MarioObject = &SnesObject.ObjectBuffers[SnesObject.CurrentObjectPointer - 1];
        MarioObject->Width = 16;
        MarioObject->Height = 16;
    }
}