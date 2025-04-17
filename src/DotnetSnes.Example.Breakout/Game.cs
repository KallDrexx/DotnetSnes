using Dntc.Attributes;
using DotnetSnes.Core;

namespace DotnetSnes.Example.Breakout;

public static class Game
{
    public const string PlayerReady = "PLAYER 1\n\n READY";
    public const string GameOver = "GAME OVER";
    public const string Paused = "PAUSE";
    public const string Blank = "        ";

    [StaticallySizedArray(4, true)]
    [InitialGlobalValue("{{-2, -1}, {-1, -2}, {1, -2}, {2, -1}}")]
    public static Vector2[] Directions;

    [StaticallySizedArray(0x64, true)]
    [InitialGlobalValue($"{{{Constants.MapData}}};")]
    public static byte[] Map;

    [StaticallySizedArray(0x400, true)]
    public static ushort[] BlockMap;

    [StaticallySizedArray(0x400, true)]
    public static ushort[] BackMap;

    [StaticallySizedArray(0x100, true)]
    public static ushort[] Palette;

    [StaticallySizedArray(0x64, true)]
    public static byte[] Blocks;

    public static byte LoopI, LoopJ, LoopK;
    public static ushort BrickA, BrickB, BrickC;
    public static ushort BlockCount;
    public static ushort BallX, BallY;
    public static ushort BrickTestX, BrickTestY;
    public static ushort Score, HighScore;
    public static ushort MaxLevel;
    public static ushort BackgroundColor, CurrentLevel, NumLives;
    public static KeypadBits Gamepad0;
    public static Vector2 BallVelocity, BallPosition;
    public static ushort PaddleXCoordinates;

    [CustomFunctionName("main")]
    public static unsafe void Main()
    {
        // Turn screen off to allow us to update vram
        Video.SetBrightness(0);
        Interrupt.WaitForVBlank();

        // Load tiles into vram
        Dma.CopyVram(ref AssemblyLabels.Tiles1, 0x1000, 0xf00);
        Dma.CopyVram(ref AssemblyLabels.Tiles2, 0x2000, 0x250);

        // Copy data files to ram var to be able to modify them
        Utils.MemCopy(CUtils.AddressOf(BlockMap), CUtils.AddressOf(AssemblyLabels.Background1Map), 0x800);
        Utils.MemCopy(CUtils.AddressOf(BackMap), CUtils.AddressOf(AssemblyLabels.Background2Map), 0x800);
        Utils.MemCopy(CUtils.AddressOf(Blocks), CUtils.AddressOf(Map), 0x64);
        Utils.MemCopy(CUtils.AddressOf(AssemblyLabels.Palette), CUtils.AddressOf(Palette), 0x200);

        // Init global variables
        BlockCount = 0;
        BallX = 5;
        BallY = 11;
        Score = 0;
        HighScore = 50000;
        MaxLevel = 1;
        BackgroundColor = 0;
        CurrentLevel = 0;
        NumLives = 4;
        PaddleXCoordinates = 80;
        BallVelocity.X = 22;
        BallVelocity.Y = 1;
        BallPosition.X = 94;
        BallPosition.Y = 109;

        // Init map with all the bricks
        BrickB = 0;
        for (LoopJ = 0; LoopJ < 10; LoopJ++)
        {
            for (LoopI = 0; LoopI < 20; LoopI += 2)
            {
                BrickA = Blocks[BrickB++];
                if (BrickA < 8)
                {
                    BrickC = (ushort)((LoopJ << 5) + LoopI);
                    BlockCount++;
                    BlockMap[0x62 + BrickC] = (ushort)(13 + (BrickA << 10));
                    BlockMap[0x63 + BrickC] = (ushort)(14 + (BrickA << 10));
                    BackMap[0x83 + BrickC] += 0x400;
                    BackMap[0x84 + BrickC] += 0x400;
                }
            }
        }

        WriteNumber(NumLives, 8, ref BlockMap, 0x136, 0x426);
        WriteString(PlayerReady, ref BlockMap, 0x248, 0x3f6);
        Interrupt.WaitForVBlank(); // Wait to avoid glitches

        // Init map bg0 and bg2 address and put data inside them
        Background.InitMapSet(0, ref BlockMap, 0x800, MapSizes.Size32X32, 0x000);
        Background.InitMapSet(2, ref BackMap, 0x800, MapSizes.Size32X32, 0x400);
        Dma.CopyCGram(ref Palette, 0, 256 * 2);

        // Init graphics pointer for each background
        Background.SetGfxPointer(0, 0x1000);
        Background.SetGfxPointer(2, 0x2000);

        // Put it in 16-bit color mode and disable bg2
        Video.SetMode(BackgroundMode.Mode1, 0);
        Background.Disable(1);

        Video.SetScreenOn();
        DrawScreen();

        for (LoopI = 0; LoopI < 10 * 4; LoopI += 4)
        {
            Sprite.SetExtendedProperties(1, Sprite.SpriteSize.Small, OamVisibility.Show);
        }

        // Wait for key pressed
        while (Input.PadsCurrent(0) == 0)
        {
            Interrupt.WaitForVBlank();
        }

        // Remove text (wait for vblank to be sure of no glitches
        WriteString(Blank, ref BlockMap, 0x248, 0x3f6);
        WriteString(Blank, ref BlockMap, 0x289, 0x3f6);
        Interrupt.WaitForVBlank();
        Dma.CopyVram(ref BlockMap, 0x000, 0x800);

        while (true)
        {
            RunFrame();
        }
    }

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
            CUtils.AddressOf(Palette, 16),
            CUtils.AddressOf(AssemblyLabels.Backgroundpalette, BackgroundColor, 16),
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
        Dma.CopyCGram(ref AssemblyLabels.Palette, 0, 256 * 2);
        Dma.CopyVram(ref BlockMap, 0x0000, 0x800);
        Dma.CopyVram(ref BackMap, 0x0400, 0x800);

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

        Dma.CopyVram(ref BlockMap, 0x000, 0x800);
    }

    private static unsafe void Die()
    {
        if (NumLives == 0)
        {
            WriteString(GameOver, ref BlockMap, 0x267, 0x3f6);
            Interrupt.WaitForVBlank();
            Dma.CopyVram(ref BlockMap, 0x000, 0x800);
            while (true)
            {
                // Require a reset to reset
            }
        }

        NumLives--;
        BallPosition.X = 94;
        BallPosition.Y = 109;
        PaddleXCoordinates = 80;

        WriteNumber(NumLives, 8, ref BlockMap, 0x267, 0x3f6);
        WriteString(PlayerReady, ref BlockMap, 0x248, 0x3f6);
        Interrupt.WaitForVBlank();
        Dma.CopyVram(ref BlockMap, 0x000, 0x800);

        DrawScreen();

        // Wait until a key is pressed
        while (Input.PadsCurrent(0) == 0)
        {
            Interrupt.WaitForVBlank();
        }

        // Remove the message
        WriteString(Blank, ref BlockMap, 0x248, 0x3f6);
        WriteString(Blank, ref BlockMap, 0x289, 0x3f6);
        Interrupt.WaitForVBlank();
        Dma.CopyVram(ref BlockMap, 0x000, 0x800);
    }

    private static unsafe void HandlePause()
    {
        // If we pushed the pause button
        if ((Gamepad0 & KeypadBits.Start) > 0)
        {
            WriteString(Paused, ref BlockMap, 0x269, 0x3f6);
            Interrupt.WaitForVBlank();
            Dma.CopyVram(ref BlockMap, 0x000, 0x800);

            // Wait for start to be released
            while (Input.PadsCurrent(0) != 0)
            {
                Interrupt.WaitForVBlank();
            }

            // Wait for start to be pressed again
            while ((Input.PadsCurrent(0) & KeypadBits.Start) == 0)
            {
                Interrupt.WaitForVBlank();
            }

            // Wait for start to be released again
            while ((Input.PadsCurrent(0) & KeypadBits.Start) > 0)
            {
                Interrupt.WaitForVBlank();
            }

            WriteString(Blank, ref BlockMap, 0x269, 0x3f6);
            Interrupt.WaitForVBlank();
            Dma.CopyVram(ref BlockMap, 0x000, 0x800);
        }
    }

    private static unsafe void RunFrame()
    {
        Gamepad0 = Input.PadsCurrent(0);
        HandlePause();

        // If A is pressed, do a fast move
        if ((Gamepad0 & KeypadBits.A) > 0)
        {
            if ((Gamepad0 & KeypadBits.Right) > 0)
            {
                PaddleXCoordinates += 4;
            }

            if ((Gamepad0 & KeypadBits.Left) > 0)
            {
                PaddleXCoordinates -= 4;
            }
        }
        else
        {
            if ((Gamepad0 & KeypadBits.Right) > 0)
            {
                PaddleXCoordinates += 4;
            }

            if ((Gamepad0 & KeypadBits.Left) > 0)
            {
                PaddleXCoordinates -= 4;
            }
        }

        PaddleXCoordinates = Clamp(PaddleXCoordinates, 16, 144);
        BallPosition.X += BallVelocity.X;
        BallPosition.Y += BallVelocity.Y;

        // React to walls
        if (BallPosition.X > 171)
        {
            BallVelocity.X = (short)-BallVelocity.X;
            BallPosition.X = 171;
        }
        else if (BallPosition.X < 16)
        {
            BallVelocity.X = (short)-BallVelocity.X;
            BallPosition.X = 16;
        }

        // Check the ball against bricks or the top/bottom of the screen
        if (BallPosition.Y < 15)
        {
            // Top of the screen
            BallVelocity.Y = (short)-BallVelocity.Y;
        }
        else if (BallPosition.Y > 195)
        {
            // Are we colliding with the paddle?
            if (BallPosition.Y < 203)
            {
                if ((BallPosition.X >= PaddleXCoordinates) && (BallPosition.X <= PaddleXCoordinates + 27))
                {
                    LoopK = (byte)((BallPosition.X - PaddleXCoordinates) / 7);
                    BallVelocity.X = Directions[LoopK].X;
                    BallVelocity.Y = Directions[LoopK].Y;
                }
            }
            else if (BallPosition.Y > 224)
            {
                Die();
            }
        }
        else if (BallPosition.Y < 224)
        {
            BrickTestX = BallX;
            BrickTestY = BallY;
            BallX = (ushort)((BallPosition.X - 14) >> 4);
            BallY = (ushort)((BallPosition.Y - 14) >> 3);
            BrickB = (ushort)(BallX + (BallY << 3) + (BallY << 1) - 10);

            if ((BrickB >= 0) && (BrickB < 100))
            {
                // Is the brick still here?
                if (Blocks[BrickB] != 8)
                {
                    BlockCount--;
                }

                for (LoopI = 0; LoopI <= CurrentLevel; LoopI++)
                {
                    Score += (ushort)(Blocks[BrickB] + 1);
                }

                if (BrickTestY != BallY)
                {
                    BallVelocity.Y = (short)-BallVelocity.Y;
                }

                if (BrickTestX != BallX)
                {
                    BallVelocity.X = (short)-BallVelocity.X;
                }

                // Remove the brick from the screen
                Blocks[BrickB] = 8;
                BrickB = (ushort)((BallY << 5) + (BallX << 1));
                BlockMap[0x42 + BrickB] = 0;
                BlockMap[0x43 + BrickB] = 0;
                BackMap[0x63 + BrickB] -= 0x400;
                BackMap[0x64 + BrickB] -= 0x400;
                WriteNumber(Score, 8, ref BlockMap, 0xf5, 0x426);

                if (Score > HighScore)
                {
                    HighScore = Score;
                    WriteNumber(Score, 8, ref BlockMap, 0x95, 0x426);
                }

                Interrupt.WaitForVBlank();
                Dma.CopyVram(ref BlockMap, 0x000, 0x800);
                Dma.CopyVram(ref BackMap, 0x400, 0x800);

                // If no more bricks, start a new level
                if (BlockCount == 0)
                {
                    NewLevel();
                }
            }
        }

        DrawScreen();
        Interrupt.WaitForVBlank();
    }
}