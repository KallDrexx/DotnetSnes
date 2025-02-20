namespace DotnetSnes.Core;

public static class CUtils
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <returns></returns>
    public static byte BytesBetweenAddress(ref byte first, ref byte second)
    {
        return (byte)(first - second);
    }
}