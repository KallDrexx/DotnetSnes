using DotnetSnes.Core.TranspilerSupport;

namespace DotnetSnes.Example.Breakout;

public static class AssemblyLabels
{
    [AssemblyLabel("tiles1")] public static byte Tiles1;
    [AssemblyLabel("tiles2")] public static byte Tiles2;
    [AssemblyLabel("bg1map")] public static byte Background1Map;
    [AssemblyLabel("bg2map")] public static byte Background2Map;
    [AssemblyLabel("palette")] public static byte Palette;
    [AssemblyLabel("backpal")] public static byte Backgroundpalette;
}