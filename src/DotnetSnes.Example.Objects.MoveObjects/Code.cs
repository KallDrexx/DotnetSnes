using Dntc.Attributes;
using DotnetSnes.Core;
using DotnetSnes.Core.TranspilerSupport;

namespace DotnetSnes.Example.Objects.MoveObjects;

public static unsafe class Code
{
    [AssemblyLabel("snesfont")]
    private static byte SnesFont;

    [AssemblyLabel("gfxpsrite")]
    private static byte GfxSprite;

    [AssemblyLabel("gfxpsrite_end")]
    private static byte GfxSpriteEnd;

    [AssemblyLabel("palsprite")]
    private static byte PalSprite;

    [AssemblyLabel("palsprite_end")]
    private static byte PalSpriteEnd;

    private static ushort NumSprites;
    private static ushort I;

    private static ushort Pad0;
    private static short CurrentXPosition, CurrentYPosition;

    private static ObjectDefinition* SnesObj;
    private static short* SnesObjectX;
    private static short* SnesObjectY;

    [InitialGlobalValue(MoveObjects.ObjectTable.InitialValues)]
    [CustomDeclaration($"const u16 tabobjects[] = {MoveObjects.ObjectTable.InitialValues}", "tabobjects", null)]
    private static byte ObjectTable;

    private static void TestInit(ushort xPosition, ushort yPosition, ushort type, ushort minX, ushort maxX)
    {
        if (SnesObject.New((byte)type, xPosition, yPosition) == 0)
        {
            return; // No more space
        }

        SnesObject.GetPointer(SnesObject.CurrentObjectId);
        SnesObj = CUtils.AddressOf(SnesObject.ObjectBuffers[SnesObject.CurrentObjectPointer - 1]);

        // Put sprites at coordinates, with maximum priority 3 from tile entry 0, pallet 0
        SnesObj->SpriteNumber = (ushort)(NumSprites * 4);
        Sprite.Set(SnesObj->SpriteNumber, xPosition, yPosition, 3, 0, 0, 0, 0);
        Sprite.SetExtendedProperties(SnesObj->SpriteNumber, Sprite.SpriteSize.Small, OamVisibility.Show);

        NumSprites++;
    }

    private static void TestUpdate(byte index)
    {
        SnesObj = CUtils.AddressOf(SnesObject.ObjectBuffers[index]);

        // Grab the coordinate pointers
        SnesObjectX = (short*)CUtils.AddressOf(SnesObj->XPosition[1]);
        SnesObjectY = (short*)CUtils.AddressOf(SnesObj->YPosition[1]);
        CurrentXPosition = *SnesObjectX;
        CurrentYPosition = *SnesObjectY;

        // Change sprite coordinates randomly
        I = (ushort)(Misc.Random() & 0xF);
        if (I < 7)
        {
            CurrentXPosition++;
            if (CurrentXPosition > 255)
            {
                CurrentXPosition--;
            }
        }
        else
        {
            CurrentXPosition--;
            if (CurrentXPosition < 1)
            {
                CurrentXPosition++;
            }
        }

        I = (ushort)(Misc.Random() & 0xF);
        if (I < 7)
        {
            CurrentYPosition++;
            if (CurrentYPosition > 223)
            {
                CurrentYPosition--;
            }
        }
        else
        {
            CurrentYPosition--;
            if (CurrentYPosition < 1)
            {
                CurrentYPosition++;
            }
        }

        // Change sprite display
        Sprite.Set(SnesObj->SpriteNumber, (ushort)CurrentXPosition, (ushort)CurrentYPosition, 3, 0, 0, 0, 0);

        // Update variables for the sprite
        *SnesObjectX = CurrentXPosition;
        *SnesObjectY = CurrentYPosition;
    }

    [CustomFunctionName("main")]
    public static void Main()
    {
        // Init sprites gfx and palette with default size of 32x32
        var gfxSpriteBytes = CUtils.BytesBetweenAddress(ref GfxSpriteEnd, ref GfxSprite);
        var palSpriteBytes = CUtils.BytesBetweenAddress(ref PalSpriteEnd, ref PalSprite);
        Sprite.InitGfxSet(
            ref GfxSprite,
            gfxSpriteBytes,
            ref PalSprite,
            palSpriteBytes,
            0,
            0x000,
            OamSize.Size32L64);

        // Now put it in 16 color mode
        Video.SetMode(BackgroundMode.Mode1, 0);
        Background.Disable(1);
        Background.Disable(2);

        Video.SetScreenOn();
        SnesObject.InitEngine();
        SnesObject.InitFunctions(0, &TestInit, &TestUpdate, null);

        // load all objects into memory
        NumSprites = 0;
        SnesObject.LoadObjects((byte *)CUtils.AddressOf(ObjectTable));

        // Need to init the map, even if not present to allow update functions to work
        Map.CameraXPosition = 0;
        Map.CameraYPosition = 0;

        while (true)
        {
            SnesObject.UpdateAll();
            Interrupt.WaitForVBlank();
        }
    }
}
