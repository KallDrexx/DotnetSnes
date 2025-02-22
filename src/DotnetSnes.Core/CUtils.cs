using DotnetSnes.Core.TranspilerSupport;

namespace DotnetSnes.Core;

public static class CUtils
{
    /// <summary>
    /// Gets the count of bytes between two addresses
    /// </summary>
    [SimpleMacro("bytesBetween", "#define bytesBetween(a,b) ((a) - (b))")]
    public static byte BytesBetweenAddress(ref byte first, ref byte second)
    {
        return 0;
    }

    /// <summary>
    /// Gets a pointer to the specified object.
    ///
    /// Needed because C# requires the `fixed` keyword, and that compiles to extra MSIL. So this
    /// allows us to compile it down directly to a simple macro
    /// </summary>
    [SimpleMacro("pointerTo", "#define pointerTo(a) (&(a))")]
    public static unsafe T* AddressOf<T>(T obj)
    {
        return null;
    }

    /// <summary>
    /// Gets a `void *` pointer to referenced variable.
    /// </summary>
    [SimpleMacro("voidPointer", "#define voidPointer(a) ((char *)(&(a)))")]
    public static unsafe char* GetCharPointer<T>(T variableName)
    {
        return null;
    }
}