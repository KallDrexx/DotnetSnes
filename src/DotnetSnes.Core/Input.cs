using Dntc.Attributes;

namespace DotnetSnes.Core;

/// <summary>
/// Input support. Inputs are automatically read by the VBlank-ISR on non-lag frames
/// </summary>
public static class Input
{
    /// <summary>
    /// Return the current value of the selected pad
    /// </summary>
    /// <param name="padIndex">Pad index to use (0-1 or 0-4 if multipayer 5 connected)</param>
    /// <returns>Value of the specified pad</returns>
    [NativeFunctionCall("padsCurrent", Constants.HeaderFile)]
    public static KeypadBits PadsCurrent(ushort padIndex)
    {
        return 0;
    }
}