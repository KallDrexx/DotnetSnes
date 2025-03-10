using Dntc.Attributes;

namespace DotnetSnes.Core;

/// <summary>
/// Definition of a Pvsneslib object
/// </summary>
[NativeType("t_objs", Constants.HeaderFile)]
public struct ObjectDefinition
{
    /// <summary>
    /// Previous object in the linked list
    /// </summary>
    [CustomFieldName("pref")]
    public ushort Previous;

    /// <summary>
    /// Next object in the linked list
    /// </summary>
    [CustomFieldName("next")]
    public ushort Next;

    /// <summary>
    /// Type of the object, depends on the game
    /// </summary>
    [CustomFieldName("type")]
    public byte Type;

    /// <summary>
    /// Object id in the linked list
    /// </summary>
    [CustomFieldName("nID")]
    public byte Id;

    /// <summary>
    /// Sprite oam number of the object (multiple of 4)
    /// </summary>
    [CustomFieldName("sprnum")]
    public ushort SpriteNumber;

    /// <summary>
    /// Sprite OAM id in sprite OAM memory
    /// </summary>
    [CustomFieldName("sprid3216")]
    public ushort SpriteId;

    /// <summary>
    /// Sprite OAM address in sprites tiles memory
    /// </summary>
    [CustomFieldName("sprblk3216")]
    public ushort SpriteAddress;

    /// <summary>
    /// Sprite flip attribute
    /// </summary>
    [CustomFieldName("sprflip")]
    public byte SpriteFlip;

    /// <summary>
    /// Sprite palette
    /// </summary>
    [CustomFieldName("sprpal")]
    public byte SpritePalette;

    /// <summary>
    /// Current frame
    /// </summary>
    [CustomFieldName("sprframe")]
    public ushort SpriteFrame;

    /// <summary>
    /// X position on the screen (3 fixed point)
    /// </summary>
    [CustomFieldName("xpos")]
    [StaticallySizedArray(3, true)]
    public byte[] XPosition;

    /// <summary>
    /// Y position on the screen (3 fixed point)
    /// </summary>
    [CustomFieldName("ypos")]
    [StaticallySizedArray(3, true)]
    public byte[] YPosition;

    /// <summary>
    /// X offset of the object (from the square 16x16 or 32x32)
    /// </summary>
    [CustomFieldName("xofs")]
    public ushort XOffset;

    /// <summary>
    /// Y offset of the object (from the square 16x16 or 32x32)
    /// </summary>
    [CustomFieldName("yofs")]
    public ushort YOffset;

    /// <summary>
    /// Width of the object (from the square 16x16 or 32x32)
    /// </summary>
    [CustomFieldName("width")]
    public ushort Width;

    /// <summary>
    /// Height of the object (from the square 16x16 or 32x32)
    /// </summary>
    [CustomFieldName("height")]
    public ushort Height;

    /// <summary>
    /// Min x coordinate for action of object, depends on game (ex: revert direction)
    /// </summary>
    [CustomFieldName("xmin")]
    public ushort XMin;

    /// <summary>
    /// Max x coordinate for action of object, depends on game (ex: revert direction)
    /// </summary>
    [CustomFieldName("xmax")]
    public ushort XMax;

    /// <summary>
    /// X Velocity
    /// </summary>
    [CustomFieldName("xvel")]
    public short XVelocity;

    /// <summary>
    /// Y Velocity
    /// </summary>
    [CustomFieldName("yvel")]
    public short YVelocity;

    /// <summary>
    /// Tile number object is standing on
    /// </summary>
    [CustomFieldName("tilestand")]
    public ushort TileStanding;

    /// <summary>
    /// Tile number above object
    /// </summary>
    [CustomFieldName("tileabove")]
    public ushort TileAbove;

    /// <summary>
    /// Tile property stand on
    /// </summary>
    [CustomFieldName("tilesprop")]
    public ushort TileStandingProperty;

    /// <summary>
    /// Tile property on left/right side
    /// </summary>
    [CustomFieldName("tilebprop")]
    public ushort TileAdjacentProperty;

    /// <summary>
    /// Current action of the object, depends on the game.
    /// </summary>
    [CustomFieldName("action")]
    public ObjectAction CurrentObjectAction;

    /// <summary>
    /// Status of object regarding collision
    /// </summary>
    [CustomFieldName("status")]
    public byte Status;

    /// <summary>
    /// If object needs tempo
    /// </summary>
    [CustomFieldName("tempo")]
    public byte Tempo;

    /// <summary>
    /// If object needs a counter
    /// </summary>
    [CustomFieldName("count")]
    public byte Count;

    /// <summary>
    /// If object needs to manage direction
    /// </summary>
    [CustomFieldName("dir")]
    public byte Direction;

    /// <summary>
    /// Object id of parent (useful for projectiles)
    /// </summary>
    [CustomFieldName("parentID")]
    public ushort ParentId;

    /// <summary>
    /// Number of hit points of object
    /// </summary>
    [CustomFieldName("hitpoints")]
    public byte HitPoints;

    /// <summary>
    /// If object needs sprite to be refreshed
    /// </summary>
    [CustomFieldName("sprrefresh")]
    public byte SpriteRefresh;

    /// <summary>
    /// To know if object is on screen or not
    /// </summary>
    [CustomFieldName("onscreen;")]
    public byte OnScreen;

    /// <summary>
    /// Not used: OB_SIZE-55-1 for future use
    /// </summary>
    [CustomFieldName("objnotused")]
    [StaticallySizedArray(7, true)]
    public byte[] NotUsed;
}