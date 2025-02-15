using Dntc.Attributes;
using DotnetSnes.Core;

namespace DotnetSnes.Example.HelloWorld;

public static class Game
{
    [CustomDeclaration("extern char tilfont", "tilfont", null)]
    public static byte TileFont;

    [CustomDeclaration("extern char palfont", "palfont", null)]
    public static byte PaletteFont;

    [CustomFunctionName("main")]
    public static int Main()
    {
        // Initialize text console with our font
        SnesConsole.SetTextVramBgAddress(0x6800);
        SnesConsole.SetTextVramAddress(0x3000);
        SnesConsole.SetTextOffset(0x0100);
        SnesConsole.InitText(0, 16 * 2, ref TileFont, ref PaletteFont);

        // Init background
        Background.SetGfxPointer(0, 0x2000);
        Background.SetMapPointer(0, 0x6800, MapSizes.Size32X32);

        // Now put in 16 color mode and disable bgs except current
        Video.SetMode(BackgroundMode.Mode1, 0);
        Background.Disable(1);
        Background.Disable(2);

        // Draw text
        SnesConsole.DrawText(10, 10, "Hello World!");
        SnesConsole.DrawText(6, 14, "From C#!");

        Video.SetScreenOn();

        while (true)
        {
            Interrupt.WaitForVBlank();
        }

#pragma warning disable CS0162 // Unreachable code detected
        return 0;
#pragma warning restore CS0162 // Unreachable code detected
    }
}