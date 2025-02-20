using DotnetSnes.Core.TranspilerSupport;

namespace DotnetSnes.Example.LikeMario;

public static class Globals
{
    [AssemblyLabel("SOUNDBANK__")]
    public static byte SoundBank;

    [AssemblyLabel("jumpsnd")]
    public static byte JumpSound;

    [AssemblyLabel("jmpsndend")]
    public static byte JumpSoundEnd;


    // extern char tileset, tilesetend, tilepal;
    // extern char tilesetdef, tilesetatt; // for map & tileset of map
    //
    // extern char mapmario, objmario;
    //
    // extern char mariogfx, mariogfx_end;
    // extern char mariopal;
    //
    // extern char snesfont, snespal;
}