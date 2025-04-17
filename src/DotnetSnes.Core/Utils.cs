using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Utils
{
    [NativeFunctionCall("memcpy", Constants.HeaderFile)]
    public static unsafe void MemCopy<T, U>(T* source, U* destination, ushort count)
    {

    }
}