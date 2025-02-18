namespace DotnetSnes.Core;

public static class SnesObject
{
    /// <summary>
    /// Initialize object engine, need to be called once
    /// </summary>
    public static void InitObject() {}

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
    public static void LoadObjects(ref byte source) {}

    /// <summary>
    /// Initialize a new object in game, objgetid will have the id of the object
    /// </summary>
    /// <param name="type">The type of object depending on the game</param>
    /// <param name="x">The X coordinate of object on map or screen</param>
    /// <param name="y">The Y coordinate of object on map or screen</param>
    /// <returns>id of the object in object id table</returns>
    public static ushort New(byte type, ushort x, ushort y)
    {
        return 0;
    }
}