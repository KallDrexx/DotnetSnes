using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Dma
{
    /// <summary>
    /// Copy data from source to destination using channel 0 of DMA available channels in half words
    /// </summary>
    /// <param name="source">The source to copy from</param>
    /// <param name="address">CGram (palette storage) address to copy</param>
    /// <param name="size">Size in bytes of the data to copy</param>
    [NativeFunctionCall("dmaCopyCGram", Constants.HeaderFile)]
    public static void CopyCGram<T>(ref T source, ushort address, ushort size)
    {
    }

    /// <summary>
    /// Copy data from source to destination using channel 0 of DMA available channels in half words
    /// </summary>
    /// <param name="source">Source to copy from</param>
    /// <param name="address">VRAM address to copy</param>
    /// <param name="size">Size in bytes of the data to copy</param>
    [NativeFunctionCall("dmaCopyVram", Constants.HeaderFile)]
    public static void CopyVram<T>(ref T source, ushort address, ushort size)
    {

    }
}