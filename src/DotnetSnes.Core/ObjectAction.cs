namespace DotnetSnes.Core;

public enum ObjectAction : ushort
{
    Stand = 0x0,
    Walk = 0x1,
    Jump = 0x2,
    Fall = 0x4,
    Climb = 0x8,
    Die = 0x10,
    Burn = 0x20,
}