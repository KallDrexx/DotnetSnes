using Dntc.Attributes;

namespace DotnetSnes.Example.Breakout;

public static class Globals
{
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
}