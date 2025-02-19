using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Sound
{
    /// <summary>
    /// Boots the spc700 with sm-spc.  Call once at startup
    /// </summary>
    [NativeFunctionCall("spcBoot", Constants.HeaderFile)]
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
    [NativeFunctionCall("spcAllocateSoundRegion", Constants.HeaderFile)]
    public static void AllocateSoundRegion(byte size) { }

    /// <summary>
    /// Load module into sm-spc. This function may take some time to execute
    /// </summary>
    /// <param name="index">module_id</param>
    [NativeFunctionCall("spcLoad", Constants.HeaderFile)]
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
    [NativeFunctionCall("spcSetSoundEntry", Constants.HeaderFile)]
    public static void SetSoundEntry(
        byte volume,
        byte panning,
        byte pitch,
        ushort length,
        ref byte sampleAddress,
        ref BrrSamples samplePointer) { }

    /// <summary>
    /// Play module.
    /// NOTE: this simply queues a message, use flush if you want to wait until the message
    /// is processed.
    ///
    /// Another note: There may be significant startup time from after the message is processed to
    /// when the song starts playing... To sync the program with the song start use Flush() and then
    /// wait until SPC_P of the status register is set.
    /// </summary>
    /// <param name="startPosition"></param>
    [NativeFunctionCall("spcPlay", Constants.HeaderFile)]
    public static void Play(byte startPosition) { }

    /// <summary>
    /// Set the module playback volume
    /// </summary>
    /// <param name="volume">Volume (0..255)</param>
    [NativeFunctionCall("spcSetModuleVolume", Constants.HeaderFile)]
    public static void SetModuleVolume(byte volume) { }

    /// <summary>
    /// Process sound messages. This function will try to give messages to the spc until a few
    /// scan lines pass. This function MUST be called every frame if you are using streamed sounds.
    /// </summary>
    [NativeFunctionCall("spcProcess", Constants.HeaderFile)]
    public static void Process() { }

    /// <summary>
    /// Play sound from memory (using default arguments).
    ///
    /// Be careful: the index 0 corresponds with the LAST sound loaded. Index 1 is
    /// the penultimate sound loaded, and so on...
    /// </summary>
    /// <param name="soundIndex">Index in the sound table</param>
    [NativeFunctionCall("spcPlaySound", Constants.HeaderFile)]
    public static void PlaySound(byte soundIndex) { }
}