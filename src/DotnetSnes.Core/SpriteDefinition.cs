using Dntc.Attributes;

namespace DotnetSnes.Core;

[NativeType("t_sprites", Constants.HeaderFile)]
public unsafe struct SpriteDefinition
{
    /// <summary>
    /// X position on the screen
    /// </summary>
    [CustomFieldName("oamx")]
    public short XPosition;

    /// <summary>
    /// Y position on the screen
    /// </summary>
    [CustomFieldName("oamy")]
    public short YPosition;

    /// <summary>
    /// Frame index in graphic file of the sprite
    /// </summary>
    [CustomFieldName("oamframeid")]
    public ushort FrameId;

    /// <summary>
    /// Sprite attribute value
    /// (vhoopppc v : vertical flip h: horizontal flip o: priority bits p: palette num c : last byte of tile num)
    /// </summary>
    [CustomFieldName("oamattribute")]
    public byte Attribute;

    /// <summary>
    /// 1 if we need to load graphics from the graphic file
    /// </summary>
    [CustomFieldName("oamrefresh")]
    public byte Refresh;

    /// <summary>
    /// Pointer to the graphic file
    /// </summary>
    [CustomFieldName("oamgraphics")]
    public byte* GraphicsPointer;
}