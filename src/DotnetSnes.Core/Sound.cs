using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Sound
{
    /// <summary>
    /// Boots the spc700 with sm-spc.  Call once at startup
    /// </summary>
    [NativeFunctionCall("spc700", Constants.HeaderFile)]
    public static void Boot() { }

    /// <summary>
    /// Set the soundbank origin. Sound bank must have dedicated banks
    /// </summary>
    /// <param name="bank">Bank address</param>
    [NativeFunctionCall("spcSetBank", Constants.HeaderFile)]
    public static void SetBank(ref byte bank) { }

    /// <summary>
    /// Set the size of the sound region. This must be big enough to hold your
    /// longest/largest sound. This function will STOP module playback too
    /// </summary>
    /// <param name="size">Size of the sound region (size*256) bytes</param>
    public static void AllocateSoundRegion(byte size) { }

    /// <summary>
    /// Load module into sm-spc. This function may take some time to execute
    /// </summary>
    /// <param name="index">module_id</param>
    public static void Load(ushort index) { }

    /// <summary>
    /// Set the values and address of the sound table for a sound entry
    /// </summary>
    /// <param name="volume">Volume (0..15)</param>
    /// <param name="panning">Panning (0..15)</param>
    /// <param name="pitch">Pitch (1..6) (hz = PITCH * 2000)</param>
    /// <param name="length">Length of the brr sample</param>
    /// <param name="sampleAddress">Address of the brr sample</param>
    /// <param name="samplePointer">Pointer to the variable where sound values will be stored</param>
    public static void SetSoundEntry(
        byte volume,
        byte panning,
        byte pitch,
        ushort length,
        ref byte sampleAddress,
        ref BrrSamples samplePointer) { }
}