using Dntc.Attributes;

namespace DotnetSnes.Core;

/// <summary>
/// BRR Sample sound header
/// </summary>
[NativeType("brrsamples", Constants.HeaderFile)]
public struct BrrSamples
{
    /// <summary>
    /// Default pitch (1..6) (hz = PITCH * 2000)
    /// </summary>
    [CustomFieldName("pitch")]
    public byte Pitch;

    /// <summary>
    /// Default panning (0..15)
    /// </summary>
    [CustomFieldName("panning")]
    public byte Panning;

    /// <summary>
    /// Default volume (0..15)
    /// </summary>
    [CustomFieldName("volume")]
    public byte Volume;

    /// <summary>
    /// Number of BRR chunks (BYTELEN/9) (max 4kbs??) low
    /// </summary>
    [CustomFieldName("length1")]
    public byte Length1;

    /// <summary>
    /// Number of BRR chunks (BYTELEN/9) (max 4kbs??) high
    /// </summary>
    [CustomFieldName("length2")]
    public byte Length2;

    /// <summary>
    /// Address of the BRR data low
    /// </summary>
    [CustomFieldName("addr1")]
    public byte Address1;

    /// <summary>
    /// Address of BRR data high
    /// </summary>
    [CustomFieldName("addr2")]
    public byte Address2;

    /// <summary>
    /// Bank of BRR data
    /// </summary>
    [CustomFieldName("bank")]
    public byte Bank;
}