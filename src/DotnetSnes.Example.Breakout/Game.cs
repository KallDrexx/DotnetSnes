using Dntc.Attributes;
using DotnetSnes.Core;

namespace DotnetSnes.Example.Breakout;

public static class Game
{
    public const string PlayerReady = "PLAYER 1\n\n READY";
    public const string GameOver = "GAME OVER";
    public const string Paused = "PAUSE";
    public const string Blank = "        ";

    [StaticallySizedArray(4)]
    [InitialGlobalValue("{{-2, -1}, {-1, -2}, {1, -2}, {2, -1}}")]
    public static Vector2[] Directions;

    [StaticallySizedArray(0x64)]
    [InitialGlobalValue($"{{{Constants.MapData}}};")]
    public static byte[] Map;

    [StaticallySizedArray(0x400)]
    public static ushort[] BlockMap;

    [StaticallySizedArray(0x400)]
    public static ushort[] BackMap;

    [StaticallySizedArray(0x100)]
    public static ushort[] Palette;

    [StaticallySizedArray(0x64)]
    public static byte[] Blocks;

    public static byte LoopI, LoopJ, LoopK;
    public static ushort BrickA, BrickB, BrickC;
    public static ushort BlockCount;
    public static ushort BallX, BallY;
    public static ushort BrickTestX, BrickTestY;
    public static ushort Score, HighScore;
    public static ushort MaxLevel;
    public static ushort BackgroundColor, CurrentLevel, NumLives;
    public static ushort Gamepad0;
    public static Vector2 BallVelocity, BallPosition;
    public static ushort PaddleXCoordinates;

    private static ushort Clamp(ushort value, ushort min, ushort max)
    {
        if (value < min)
        {
            value = min;
        }

        if (value > max)
        {
            value = max;
        }

        return value;
    }

    private static void WriteString(string stringToWrite, ref ushort[] map, ushort position, ushort offset)
    {
        var startPosition = position;
        for (LoopI = 0; stringToWrite[LoopI] != '\0'; LoopI++)
        {
            if (stringToWrite[LoopI] == '\n')
            {
                startPosition += 0x20;
                position = startPosition;
            }
            else
            {
                map[position] = (ushort)(stringToWrite[LoopI] + offset);
                position++;
            }
        }
    }

    private static void WriteNumber(ushort number, byte length, ref ushort[] map, ushort position, ushort offset)
    {
        byte figure;
        position += (ushort)(length - 1);

        if (number == 0)
        {
            map[position] = offset;
        }
        else
        {
            while (length > 0 && number > 0)
            {
                figure = (byte)(number % 10);
                if (figure > 0)
                {
                    map[position] = (ushort)(figure + offset);
                }

                number /= 10;
                position--;
                length--;
            }
        }
    }

    private static void DrawScreen()
    {
        // main sprites (ball & paddle) (sprites are automatically update in VBlank function of PVSneslib)
        // id (multiple of 4),  xspr, yspr, priority, hflip, vflip, gfxoffset, paletteoffset
        Sprite.Set(0, (ushort)BallPosition.X, (ushort)BallPosition.Y, 3, 0, 0, 20 | (1 << 8), 0);
        Sprite.Set(1 * 4, (ushort)(PaddleXCoordinates + 0), 200, 3, 0, 0, 15 | (1 << 8), 0);
        Sprite.Set(2 * 4, (ushort)(PaddleXCoordinates + 8), 200, 3, 0, 0, 16 | (1 << 8), 0);
        Sprite.Set(3 * 4, (ushort)(PaddleXCoordinates + 16), 200, 3, 1, 0, 16 | (1 << 8), 0);
        Sprite.Set(4 * 4, (ushort)(PaddleXCoordinates + 24), 200, 3, 0, 0, 17 | (1 << 8), 0);

        // shadow sprites
        Sprite.Set(5 * 4, (ushort)(BallPosition.X + 3), (ushort)(BallPosition.Y + 3), 1, 0, 0, 21 | (1 << 8), 0);
        Sprite.Set(6 * 4, (ushort)(PaddleXCoordinates + 4), 204, 1, 0, 0, 18 | (1 << 8), 0);
        Sprite.Set(7 * 4, (ushort)(PaddleXCoordinates + 12), 204, 1, 0, 0, 19 | (1 << 8), 0);
        Sprite.Set(8 * 4, (ushort)(PaddleXCoordinates + 20), 204, 1, 1, 0, 19 | (1 << 8), 0);
        Sprite.Set(9 * 4, (ushort)(PaddleXCoordinates + 28), 204, 1, 0, 0, 18 | (1 << 8), 0);
    }

    private static unsafe void NewLevel()
    {
        // Update all variables regarding levels
        CurrentLevel++;
        MaxLevel++;
        BallPosition.X = 94;
        BallPosition.Y = 109;
        PaddleXCoordinates = 80;
        WriteNumber(MaxLevel, 8, ref BlockMap, 0x2d6, 0x426);
        WriteString(PlayerReady, ref BlockMap, 0x248, 0x3f6);

        // Change backgrounds
        Utils.MemCopy(
            CUtils.AddressOf(BackMap),
            CUtils.AddressOf(AssemblyLabels.Background2Map) + 0x800 * (CurrentLevel & 3),
            0x800);

        Utils.MemCopy(
            CUtils.AddressOf(Blocks),
            CUtils.AddressOf(Map),
            0x64);

        // Manage color of the background
        if (BackgroundColor < 6)
        {
            BackgroundColor++;
        }
        else
        {
            BackgroundColor = 0;
        }

        // Change the background color
        Utils.MemCopy(
            CUtils.AddressOf(Palette) + 16,
            CUtils.AddressOf(AssemblyLabels.Backgroundpalette) + BackgroundColor * 16,
            0x10);

        // Initialize the wall of bricks
        BrickB = 0;
        for (LoopJ = 0; LoopJ < 10; LoopJ++)
        {
            for (LoopI = 0; LoopI < 20; LoopI += 2)
            {
                BrickA = Blocks[BrickB];
                if (BrickA < 8)
                {
                    BrickC = (ushort)((LoopJ << 5) + LoopI);
                    BlockCount++;
                    BlockMap[0x62 + BrickC] = (ushort)(13 + (BrickA << 10));
                    BlockMap[0x63 + BrickC] = (ushort)(14 + (BrickA << 10));
                    BackMap[0x83 + BrickC] += 0x400;
                    BackMap[0x84 + BrickC] += 0x400;
                }

                BrickB++;
            }
        }

        // Reinit palette and backgrounds
        Interrupt.WaitForVBlank();
        Dma.CopyCGram(CUtils.AddressOf(AssemblyLabels.Palette), 0, 256 * 2);
        Dma.CopyVram(CUtils.AddressOf(BlockMap), 0x0000, 0x800);
        Dma.CopyVram(CUtils.AddressOf(BackMap), 0x0400, 0x800);

        DrawScreen();

        // Wait until a key is pressed
        while (Input.PadsCurrent(0) == 0)
        {
            Interrupt.WaitForVBlank();
        }

        // Remove message on the screen
        WriteString(Blank, ref BlockMap, 0x248, 0x3f6);
        WriteString(Blank, ref BlockMap, 0x289, 0x3f6);
        Interrupt.WaitForVBlank();

        Dma.CopyVram(CUtils.AddressOf(BlockMap), 0x000, 0x800);
    }
}