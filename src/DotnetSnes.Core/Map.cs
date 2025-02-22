using Dntc.Attributes;

namespace DotnetSnes.Core;

/// <summary>
/// Contains functions to manage / scroll large background MAP on SNES. This can
/// only be used for mode 1.  The engine scrolls layer 1 in x and y coordinates.
///
/// Layer address is : 6800 for Layer 1.
/// </summary>
public static class Map
{
    /// <summary>
    /// Current Y coordinates of the camera
    /// </summary>
    [NativeGlobal("y_pos", Constants.HeaderFile)]
    public static ushort CameraYPosition;

    /// <summary>
    /// Current X coordinates of the camera
    /// </summary>
    [NativeGlobal("x_pos", Constants.HeaderFile)]
    public static ushort CameraXPosition;

    /// <summary>
    /// Load map definition into memory. WARNING: Map engine must be used on background #0.
    /// </summary>
    /// <param name="layerMap">Address of the map with tiles</param>
    /// <param name="layerTiles">Address of tiles definnition</param>
    /// <param name="tilesProp">Address of tiles property definition (blocker, spikes, and so on)</param>
    [NativeFunctionCall("mapLoad", Constants.HeaderFile)]
    public static void Load(ref byte layerMap, ref byte layerTiles, ref byte tilesProp) { }

    /// <summary>
    /// Update map regarding current camera position (must be done once per frame)
    /// </summary>
    [NativeFunctionCall("mapUpdate", Constants.HeaderFile)]
    public static void Update() { }

    /// <summary>
    /// Display map regarding current buffer (must be done once per frame, near Vblank)
    /// </summary>
    [NativeFunctionCall("mapVblank", Constants.HeaderFile)]
    public static void VBlank() { }

    /// <summary>
    /// Update the map's camera (must be done once per frame
    /// </summary>
    /// <param name="xPosition">X coordinate of the object we want to focus on</param>
    /// <param name="yPosition">Y coordinate of the object we want to focus on</param>
    [NativeFunctionCall("mapUpdateCamera", Constants.HeaderFile)]
    public static void UpdateCamera(ushort xPosition, ushort yPosition) { }
}