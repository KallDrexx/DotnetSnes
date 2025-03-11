using Dntc.Attributes;
using DotnetSnes.Core.TranspilerSupport;

namespace DotnetSnes.Example.LikeMario;

public static class Globals
{
    [AssemblyLabel("SOUNDBANK__")]
    public static byte SoundBank;

    [AssemblyLabel("jumpsnd")]
    public static byte JumpSound;

    [AssemblyLabel("jumpsndend")]
    public static byte JumpSoundEnd;

    [AssemblyLabel("tileset")]
    public static byte TileSetStart;

    [AssemblyLabel("tilesetend")]
    public static byte TileSetEnd;

    [AssemblyLabel("tilepal")]
    public static byte TileSetPalette;

    [AssemblyLabel("tilesetdef")]
    public static byte TileSetDefinition;

    [AssemblyLabel("tilesetatt")]
    public static byte TileSetProperties;

    [AssemblyLabel("mapmario")]
    public static byte MarioMap;

    [AssemblyLabel("objmario")]
    public static byte MarioObject;

    [AssemblyLabel("mariogfx")]
    public static byte MarioGraphicsStart;

    [AssemblyLabel("mariogfx_end")]
    public static byte MarioGraphicsEnd;

    [AssemblyLabel("mariopal")]
    public static byte MarioPalette;

    [AssemblyLabel("snesfont")]
    public static byte SnesFont;

    [AssemblyLabel("snespal")]
    public static byte SnesPalette;

    [NativeGlobal("MOD_OVERWORLD", "soundbank.h")]
    public static byte OverworldMusic;
}