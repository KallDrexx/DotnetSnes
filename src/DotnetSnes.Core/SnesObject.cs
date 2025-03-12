using Dntc.Attributes;

namespace DotnetSnes.Core;

public static class SnesObject
{
    /// <summary>
    /// Id of the current object (useful when creating it)
    /// </summary>
    [NativeGlobal("objgetid", Constants.HeaderFile)]
    public static ushort CurrentObjectId;

    /// <summary>
    /// Pointer to the current object
    /// </summary>
    [NativeGlobal("objptr", Constants.HeaderFile)]
    public static ushort CurrentObjectPointer;

    /// <summary>
    /// Object buffer with all objects
    /// </summary>
    [NativeGlobal("objbuffers", Constants.HeaderFile)]
    [StaticallySizedArray(64, true)]
    public static ObjectDefinition[] ObjectBuffers;

    /// <summary>
    /// Initialize object engine, need to be called once
    /// </summary>
    [NativeFunctionCall("objInitEngine", Constants.HeaderFile)]
    public static void InitEngine() {}

    /// <summary>
    /// Initialize an object of a certain type with the specified init and update functionality
    /// </summary>
    [NativeFunctionCall("objInitFunctions", Constants.HeaderFile)]
    public static unsafe void InitFunctions(
        byte objectType,
        delegate*<ushort, ushort, ushort, ushort, ushort, void> initFunction,
        delegate*<byte, void> updateFunction,
        delegate*<byte, void> refreshFunction) { }

    /// <summary>
    /// Load all objects for a specific table in memory.
    ///
    /// Call, after loading, each init function of the type of objects for each object
    /// The table has an entry with x,y,type,minx,maxx for each object:
    ///     - x,y are coordinates of object,
    ///     - type if the type of the object (maximum 32 types)
    ///     - minx,maxx are the coordinates of minimum & maxinmum possible on x
    ///
    /// The last four parameters are useful to do some actions where minimum or maximum is reached.
    /// The table needs to finish with FFFF to indicate that no more objects are availables
    /// </summary>
    /// <param name="source"></param>
    [NativeFunctionCall("objLoadObjects", Constants.HeaderFile)]
    public static unsafe void LoadObjects(byte* source) {}

    /// <summary>
    /// Initialize a new object in game, objgetid will have the id of the object
    /// </summary>
    /// <param name="type">The type of object depending on the game</param>
    /// <param name="x">The X coordinate of object on map or screen</param>
    /// <param name="y">The Y coordinate of object on map or screen</param>
    /// <returns>id of the object in object id table</returns>
    [NativeFunctionCall("objNew", Constants.HeaderFile)]
    public static ushort New(byte type, ushort x, ushort y)
    {
        return 0;
    }

    /// <summary>
    /// Call update function for all objects currently active (if they are in
    /// "virtual screen" coordinates). "Virtual Screen" coordinates are -64<x<320 and -64<y<288.
    /// </summary>
    [NativeFunctionCall("objUpdateAll", Constants.HeaderFile)]
    public static void UpdateAll() { }

    /// <summary>
    /// Change a palette in CGRAM
    /// </summary>
    /// <param name="palette">Address of palette</param>
    /// <param name="paletteEntry">Palette entry (0..16 for 16 colors mode) of the beginning of each color</param>
    /// <param name="paletteSize">Size of palette</param>
    [NativeFunctionCall("setPalette", Constants.HeaderFile)]
    public static void SetPalette(ref byte palette, ushort paletteEntry, ushort paletteSize) { }

    /// <summary>
    /// Check if an object collides with the map
    /// </summary>
    /// <param name="handle">Handle for the object to check</param>
    [NativeFunctionCall("objCollidMap", Constants.HeaderFile)]
    public static void CollidMap(ushort handle) { }

    /// <summary>
    /// Update X & Y coordinates of object regarding its own velocities.
    ///
    /// It uses xvel and yvel to do the computation.
    /// </summary>
    /// <param name="handle">Handle for the object to update</param>
    [NativeFunctionCall("objUpdateXY", Constants.HeaderFile)]
    public static void UpdateXy(ushort handle) { }

    /// <summary>
    /// Get the pointer to an object from its handle (need to do -1 to have offset after).
    /// Sets `CurrentObjectPointer` to the object's pointer.
    /// </summary>
    /// <param name="objectHandle"></param>
    [NativeFunctionCall("objGetPointer", Constants.HeaderFile)]
    public static void GetPointer(ushort objectHandle) { }
}