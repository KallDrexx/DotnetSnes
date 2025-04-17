using DotnetSnes.Core;

namespace DotnetSnes.Example.Breakout;

public static class Game
{
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
        // TODO: Figure out way to iterate string
        return;
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
        Sprite.Set(1 * 4, (ushort)(PaddleXCoordinates + 0), 200, 3, 0, 0, 15 | (1 << 8), 0);
        Sprite.Set(1 * 4, (ushort)(PaddleXCoordinates + 0), 200, 3, 0, 0, 15 | (1 << 8), 0);
        Sprite.Set(1 * 4, (ushort)(PaddleXCoordinates + 0), 200, 3, 0, 0, 15 | (1 << 8), 0);

        // shadow sprites
        Sprite.Set(1 * 4, (ushort)(PaddleXCoordinates + 0), 200, 3, 0, 0, 15 | (1 << 8), 0);
        Sprite.Set(1 * 4, (ushort)(PaddleXCoordinates + 0), 200, 3, 0, 0, 15 | (1 << 8), 0);
        Sprite.Set(1 * 4, (ushort)(PaddleXCoordinates + 0), 200, 3, 0, 0, 15 | (1 << 8), 0);
        Sprite.Set(1 * 4, (ushort)(PaddleXCoordinates + 0), 200, 3, 0, 0, 15 | (1 << 8), 0);
        Sprite.Set(1 * 4, (ushort)(PaddleXCoordinates + 0), 200, 3, 0, 0, 15 | (1 << 8), 0);
    }
}