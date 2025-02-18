namespace DotnetSnes.Core;

public enum OamSize : byte
{
    /// <summary>
    /// Default OAM size 8x8 (SM) and 16x16 (LG) pix for OBJSEL register
    /// </summary>
    Size8L16 = 0,

    /// <summary>
    /// Default OAM size 8x8 (SM) and 32x32 (LG) pix for OBJSEL register
    /// </summary>
    Size8L32 = 1 << 5,

    /// <summary>
    /// Default OAM size 8x8 (SM) and 64x64 (LG) pix for OBJSEL register
    /// </summary>
    Size8L64 = 2 << 5,

    /// <summary>
    /// Default OAM size 16x16 (SM) and 32x32 (LG) pix for OBJSEL register
    /// </summary>
    Size16L32 = 3 << 5,

    /// <summary>
    /// Default OAM size 16x16 (SM) and 64x64 (LG) pix for OBJSEL register
    /// </summary>
    Size16L64 = 4 << 5,

    /// <summary>
    /// Default OAM size 32x32 (SM) and 64x64 (LG) pix for OBJSEL register
    /// </summary>
    Size32L64 = 5 << 5,
}