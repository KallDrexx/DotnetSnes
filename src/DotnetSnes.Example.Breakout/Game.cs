using Dntc.Attributes;
using DotnetSnes.Core;

namespace DotnetSnes.Example.Breakout;

// Breakout game based on the PVSnesLib breakout example
public static class Game
{
    private const string PlayerReady = "DOTNET SNES\n\n READY";
    private const string GameOver = "GAME OVER";
    private const string Paused = "PAUSE";
    private const string Blank = "           ";
    private const ushort NumberTextureOffset = 0x426;
    private const ushort MessageTextureOffset = 0x3f6;
    private const ushort LevelNumberPosition = 0x2d6;
    private const ushort LivesPosition = 0x136;
    private const ushort MessageLine1Position = 0x248;
    private const ushort MessageLine2Position = 0x289;
    private const ushort GameOverPosition = 0x267; 
    private const ushort PausedPosition = 0x269;
    private const ushort ScorePosition = 0xf5;
    private const ushort HighScorePosition = 0x95;
    private const ushort TotalBlocks = 0x64;
    private const short BallStartX = 94;
    private const short BallStartY = 109;
    private const ushort PaddleStartX = 80;
    private const byte PaddleSpeed = 2;
    private const byte PaddleFastSpeed = 4;
    private const short PaddleWidth = 27;
    private const short LeftBoundary = 16;
    private const short RightBoundary = 171;
    private const short TopBoundary = 15;
    private const short BottomBoundary = 224;
    private const short PaddleTopBoundary = 195;
    private const short PaddleBottomBoundary = 203;
    private const ushort BackgroundSize = 0x400;
    private const byte MissingBrick = 8;

    [StaticallySizedArray(4, true)]
    [InitialGlobalValue("{{-2, -1}, {-1, -2}, {1, -2}, {2, -1}}")]
    public static Vector2[] Directions;

    [StaticallySizedArray(0x64, true)]
    [InitialGlobalValue($"{{{Constants.MapData}}};")]
    public static byte[] Map;

    [StaticallySizedArray(BackgroundSize, true)]
    public static ushort[] BlockMap;

    [StaticallySizedArray(BackgroundSize, true)]
    public static ushort[] BackMap;

    [StaticallySizedArray(0x100, true)]
    public static ushort[] Palette;

    [StaticallySizedArray(TotalBlocks, true)]
    public static byte[] Blocks;

    public static ushort BlockCount;
    public static ushort Score;
    public static ushort HighScore;
    public static ushort BackgroundColor;
    public static ushort CurrentLevel;
    public static ushort NumLives;

    public static KeypadBits Gamepad0;
    public static Vector2 BallVelocity;
    public static Vector2 BallPosition;
    public static Vector2 BallPositionInPixels; // In pixel space
    public static ushort PaddleXCoordinates;

    [CustomFunctionName("main")]
    public static void Main()
    {
        // Turn screen off to allow us to update vram
        Video.SetBrightness(0);
        Interrupt.WaitForVBlank();

        // Load tiles into vram
        Dma.CopyVram(ref AssemblyLabels.Tiles1, 0x1000, 0xf00);
        Dma.CopyVram(ref AssemblyLabels.Tiles2, 0x2000, 0x250);

        // Copy data files to ram var to be able to modify them
        unsafe
        {
            Utils.MemCopy(CUtils.AddressOf(BlockMap), CUtils.AddressOf(AssemblyLabels.Background1Map), 0x800);
            Utils.MemCopy(CUtils.AddressOf(BackMap), CUtils.AddressOf(AssemblyLabels.Background2Map), 0x800);
            Utils.MemCopy(CUtils.AddressOf(Blocks), CUtils.AddressOf(Map), 0x64);
            Utils.MemCopy(CUtils.AddressOf(Palette), CUtils.AddressOf(AssemblyLabels.Palette), 0x200);
        }

        // Init global variables
        BlockCount = 0;
        Score = 0;
        HighScore = 50000;
        BackgroundColor = 0;
        CurrentLevel = 0;
        NumLives = 4;
        PaddleXCoordinates = 80;
        BallVelocity.X = 2;
        BallVelocity.Y = 1;
        BallPosition.X = BallStartX;
        BallPosition.Y = BallStartY;
        BallPositionInPixels.X = (short)(BallPosition.X >> 4);
        BallPositionInPixels.Y = (short)(BallPosition.Y >> 3);

        // Init map with all the bricks
        byte index = 0;
        for (byte j = 0; j < 10; j++)
        {
            for (byte i = 0; i < 20; i += 2)
            {
                byte block = Blocks[index];
                index++;
                if (block < MissingBrick)
                {
                    ushort c = (ushort)((j << 5) + i);
                    BlockCount++;
                    BlockMap[0x62 + c] = (ushort)(13 + (block << 10));
                    BlockMap[0x63 + c] = (ushort)(14 + (block << 10));
                    BackMap[0x83 + c] += 0x400;
                    BackMap[0x84 + c] += 0x400;
                }
            }
        }

        WriteNumber(NumLives, LivesPosition, NumberTextureOffset);
        WriteString(PlayerReady, ref BlockMap, MessageLine1Position);
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

        for (byte i = 0; i < 10 * 4; i += 4)
        {
            Sprite.SetExtendedProperties(1, Sprite.SpriteSize.Small, OamVisibility.Show);
        }

        // Wait for key pressed
        while (Input.PadsCurrent(0) == 0)
        {
            Interrupt.WaitForVBlank();
        }

        // Remove text (wait for vblank to be sure of no glitches
        WriteString(Blank, ref BlockMap, MessageLine1Position);
        WriteString(Blank, ref BlockMap, MessageLine2Position);
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

    private static void WriteString(string stringToWrite, ref ushort[] map, ushort position)
    {
        var startPosition = position;
        for (byte i = 0; stringToWrite[i] != '\0'; i++)
        {
            if (stringToWrite[i] == '\n')
            {
                startPosition += 0x20;
                position = startPosition;
            }
            else
            {
                map[position] = (ushort)(stringToWrite[i] + MessageTextureOffset);
                position++;
            }
        }
    }

    private static void WriteNumber(ushort number, ushort position, ushort textureOffset)
    {
        byte length = 8;
        byte figure;
        position += (ushort)(length - 1);

        if (number == 0)
        {
            BlockMap[position] = textureOffset;
        }
        else
        {
            while (length > 0 && number > 0)
            {
                figure = (byte)(number % 10);
                if (figure > 0)
                {
                    BlockMap[position] = (ushort)(figure + textureOffset);
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

    private static void NewLevel()
    {
        // Update all variables regarding levels
        CurrentLevel++;
        BallPosition.X = BallStartX;
        BallPosition.Y = BallStartY;
        PaddleXCoordinates = PaddleStartX;
        WriteNumber((ushort)(CurrentLevel + 1), LevelNumberPosition, NumberTextureOffset);
        WriteString(PlayerReady, ref BlockMap, MessageLine1Position);

        // Change backgrounds
        unsafe
        {
            Utils.MemCopy(
                CUtils.AddressOf(BackMap),
                CUtils.AddressOf(AssemblyLabels.Background2Map,  0x800 * (CurrentLevel & 3)),
                0x800);

            Utils.MemCopy(
                CUtils.AddressOf(Blocks),
                CUtils.AddressOf(Map),
                TotalBlocks);
        }

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
        unsafe
        {
            Utils.MemCopy(
                CUtils.AddressOf(Palette, 16),
                CUtils.AddressOf(AssemblyLabels.Backgroundpalette, BackgroundColor, 16),
                0x10);
        }

        // Initialize the wall of bricks
        byte index = 0;
        for (byte j = 0; j < 10; j++)
        {
            for (byte i = 0; i < 20; i += 2)
            {
                var block = Blocks[index];
                if (block < MissingBrick)
                {
                    var c = (ushort)((j << 5) + i);
                    BlockCount++;
                    BlockMap[0x62 + c] = (ushort)(13 + (block << 10));
                    BlockMap[0x63 + c] = (ushort)(14 + (block << 10));
                    BackMap[0x83 + c] += 0x400;
                    BackMap[0x84 + c] += 0x400;
                }

                index++;
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
        WriteString(Blank, ref BlockMap, MessageLine1Position);
        WriteString(Blank, ref BlockMap, MessageLine2Position);
        Interrupt.WaitForVBlank();

        Dma.CopyVram(ref BlockMap, 0x000, 0x800);
    }

    private static void Die()
    {
        if (NumLives == 0)
        {
            WriteString(GameOver, ref BlockMap, GameOverPosition);
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

        WriteNumber(NumLives, LivesPosition, MessageTextureOffset);
        WriteString(PlayerReady, ref BlockMap, MessageLine1Position);
        Interrupt.WaitForVBlank();
        Dma.CopyVram(ref BlockMap, 0x000, 0x800);

        DrawScreen();

        // Wait until a key is pressed
        while (Input.PadsCurrent(0) == 0)
        {
            Interrupt.WaitForVBlank();
        }

        // Remove the message
        WriteString(Blank, ref BlockMap, MessageLine1Position);
        WriteString(Blank, ref BlockMap, MessageLine2Position);
        Interrupt.WaitForVBlank();
        Dma.CopyVram(ref BlockMap, 0x000, 0x800);
    }

    private static void HandlePause()
    {
        // If we pushed the pause button
        if ((Gamepad0 & KeypadBits.Start) > 0)
        {
            WriteString(Paused, ref BlockMap, PausedPosition);
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

            WriteString(Blank, ref BlockMap, PausedPosition);
            Interrupt.WaitForVBlank();
            Dma.CopyVram(ref BlockMap, 0x000, 0x800);
        }
    }

    private static void RunFrame()
    {
        Gamepad0 = Input.PadsCurrent(0);
        HandlePause();

        // If A is pressed, move faster
        if ((Gamepad0 & KeypadBits.A) > 0)
        {
            if ((Gamepad0 & KeypadBits.Right) > 0)
            {
                PaddleXCoordinates += PaddleFastSpeed;
            }

            if ((Gamepad0 & KeypadBits.Left) > 0)
            {
                PaddleXCoordinates -= PaddleFastSpeed;
            }
        }
        else
        {
            if ((Gamepad0 & KeypadBits.Right) > 0)
            {
                PaddleXCoordinates += PaddleSpeed;
            }

            if ((Gamepad0 & KeypadBits.Left) > 0)
            {
                PaddleXCoordinates -= PaddleSpeed;
            }
        }

        PaddleXCoordinates = Clamp(PaddleXCoordinates, (ushort)LeftBoundary, (RightBoundary - PaddleWidth));
        BallPosition.X += BallVelocity.X;
        BallPosition.Y += BallVelocity.Y;

        // React to walls
        if (BallPosition.X > RightBoundary)
        {
            BallVelocity.X = (short)-BallVelocity.X;
            BallPosition.X = RightBoundary;
        }
        else if (BallPosition.X < LeftBoundary)
        {
            BallVelocity.X = (short)-BallVelocity.X;
            BallPosition.X = LeftBoundary;
        }

        // Check the ball against bricks or the top/bottom of the screen
        if (BallPosition.Y < TopBoundary)
        {
            BallVelocity.Y = (short)-BallVelocity.Y;
        }
        else if (BallPosition.Y > PaddleTopBoundary)
        {
            // Are we colliding with the paddle?
            if (BallPosition.Y < PaddleBottomBoundary)
            {
                if ((BallPosition.X >= PaddleXCoordinates) && (BallPosition.X <= PaddleXCoordinates + PaddleWidth))
                {
                    var index = (byte)((BallPosition.X - PaddleXCoordinates) / 7);
                    BallVelocity.X = Directions[index].X;
                    BallVelocity.Y = Directions[index].Y;
                }
            }
            else if (BallPosition.Y > BottomBoundary)
            {
                Die();
            }
        }
        else if (BallPosition.Y < 112)
        {
            // Did the ball hit a block?
            var prevPosition = BallPositionInPixels;
            BallPositionInPixels.X = (short)((BallPosition.X - 14) >> 4);
            BallPositionInPixels.Y = (short)((BallPosition.Y - 14) >> 3);

            var brickIndex = (ushort)(BallPositionInPixels.X + (BallPositionInPixels.Y << 3) + (BallPositionInPixels.Y << 1) - 10);
            if (brickIndex < TotalBlocks)
            {
                // Is the brick still here?
                if (Blocks[brickIndex] != 8)
                {
                    BlockCount--;
                    for (byte i = 0; i <= CurrentLevel; i++)
                    {
                        Score += (ushort)(Blocks[brickIndex] + 1);
                    }

                    // Only adjust velocity if the ball changes positions since the last frame
                    if (prevPosition.Y != BallPositionInPixels.Y)
                    {
                        BallVelocity.Y = (short)-BallVelocity.Y;
                    }

                    if (prevPosition.X != BallPositionInPixels.X)
                    {
                        BallVelocity.X = (short)-BallVelocity.X;
                    }

                    // Remove the brick from the screen
                    Blocks[brickIndex] = MissingBrick;
                    brickIndex = (ushort)((BallPositionInPixels.Y << 5) + (BallPositionInPixels.X << 1));
                    BlockMap[0x42 + brickIndex] = 0;
                    BlockMap[0x43 + brickIndex] = 0;
                    BackMap[0x63 + brickIndex] -= BackgroundSize;
                    BackMap[0x64 + brickIndex] -= BackgroundSize;
                    WriteNumber(Score, ScorePosition, NumberTextureOffset);

                    if (Score > HighScore)
                    {
                        HighScore = Score;
                        WriteNumber(Score, HighScorePosition, NumberTextureOffset);
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
        }

        DrawScreen();
        Interrupt.WaitForVBlank();
    }
}