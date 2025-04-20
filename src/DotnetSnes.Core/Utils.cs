using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class Utils
{
    [NativeFunctionCall("memcpy", "<string.h>")]
    public static unsafe void MemCopy<T, U>(T* source, U* destination, ushort count)
    {

    }
}