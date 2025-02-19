namespace DotnetSnes.Core;

public enum KeypadBits : ushort
{
    A = 1 << 7,
    B = 1 << 15,
    Select = 1 << 13,
    Start = 1 << 12,
    Right = 1 << 8,
    Left = 1 << 9,
    Down = 1 << 10,
    Up = 1 << 11,
    R = 1 << 4,
    L = 1 << 5,
    X = 1 << 6,
    Y = 1 << 14,
}