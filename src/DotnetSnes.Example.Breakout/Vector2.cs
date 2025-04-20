using Dntc.Attributes;

namespace DotnetSnes.Example.Breakout;

public struct Vector2
{
    [CustomFieldName("x")]
    public short X;

    [CustomFieldName("y")]
    public short Y;
}