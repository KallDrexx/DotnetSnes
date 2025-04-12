using Dntc.Attributes;
using DotnetSnes.Core;

namespace DotnetSnes.Example.LikeMario;

public static unsafe class Mario
{
    private const short Acceleration = 0x0038;
    private const short MaxAcceleration = 0x0140;
    private const short JumpingAcceleration = 0x0394;
    private const short HighJumpingAcceleration = 0x0594;

    /// <summary>
    /// Pointer to the Mario object
    /// </summary>
    public static ObjectDefinition* MarioObject;

    /// <summary>
    /// Pointer to Mario's X coordinates with fixed point
    /// </summary>
    private static short* MarioXCoords;

    /// <summary>
    /// Pointer to Mario's Y coordinates with fixed point
    /// </summary>
    private static short* MarioYCoords;

    /// <summary>
    /// Pointer to Mario's X velocity with fixed point
    /// </summary>
    private static short* MarioXVelocity;

    /// <summary>
    /// Pointer to Mario's Y velocity with fixed point
    /// </summary>
    private static short* MarioYVelocity;

    /// <summary>
    /// Mario's X coordinates with map depth (not only screen)
    /// </summary>
    private static ushort MarioXMapDepth;

    /// <summary>
    /// Mario's Y coordinates with map depth (not only screen)
    /// </summary>
    private static ushort MarioYMapDepth;

    // To manage the sprite display
    public static byte MarioFidx;
    public static byte MarioFlip;
    public static byte Flip;

    public static void Initialize(ushort xPosition, ushort yPosition, ushort type, ushort minX, ushort maxX)
    {
        // Prepare new object
        if (SnesObject.New((byte)type, xPosition, yPosition) == 0)
        {
            // No more space, exit
            return;
        }

        SnesObject.GetPointer(SnesObject.CurrentObjectId);
        MarioObject = CUtils.AddressOf(SnesObject.ObjectBuffers[SnesObject.CurrentObjectPointer - 1]);
        MarioObject->Width = 16;
        MarioObject->Height = 16;

        MarioXCoords = (short*)CUtils.AddressOf(MarioObject->XPosition[1]);
        MarioYCoords = (short*)CUtils.AddressOf(MarioObject->YPosition[1]);
        MarioXVelocity = CUtils.AddressOf(MarioObject->XVelocity);
        MarioYVelocity = CUtils.AddressOf(MarioObject->YVelocity);

        MarioFidx = 0;
        MarioFlip = 0;

        MarioObject->CurrentObjectAction = ObjectAction.Stand;

        Sprite.Buffer[0].FrameId = 6;
        Sprite.Buffer[0].Refresh = 1;
        Sprite.Buffer[0].Attribute = 0x60; // palette 0 of sprite, sprite 16x16, priority 2, flip
        Sprite.Buffer[0].GraphicsPointer = CUtils.AddressOf(Globals.MarioGraphicsStart);

        SnesObject.SetPalette(ref Globals.MarioPalette, 128 + 0 * 16, 16 * 2);
    }

    public static void Update(byte index)
    {
        Game.Pad0 = (KeypadBits)Input.PadsCurrent(0);

        if ((Game.Pad0 & (KeypadBits.A | KeypadBits.Left | KeypadBits.Right)) > 0)
        {
            if ((Game.Pad0 & KeypadBits.Left) == KeypadBits.Left)
            {
                // Update animation (sprites 2-3)
                if (MarioFlip != 2)
                {
                    MarioFlip = 2;
                }

                // `& 0xFF` needed for the C# compiler not to complain that ~0x40 overflows a byte
                Sprite.Buffer[0].Attribute &= ~0x40 & 0xFF; // do not flip the sprite

                MarioObject->CurrentObjectAction = ObjectAction.Walk;
                *MarioXVelocity -= Acceleration;
                if (*MarioXVelocity <= -MaxAcceleration)
                {
                    *MarioXVelocity = -MaxAcceleration;
                }
            }

            if ((Game.Pad0 & KeypadBits.Right) == KeypadBits.Right)
            {
                // Update animation (sprites 2-3)
                if (MarioFlip != 2)
                {
                    MarioFlip = 2;
                }

                Sprite.Buffer[0].Attribute |= 0x40; // flip the sprite

                MarioObject->CurrentObjectAction = ObjectAction.Walk;
                *MarioXVelocity += Acceleration;
                if (*MarioXVelocity >= MaxAcceleration)
                {
                    *MarioXVelocity = MaxAcceleration;
                }
            }

            if ((Game.Pad0 & KeypadBits.A) == KeypadBits.A)
            {
                // We can jump only if we are on the ground
                if (MarioObject->TileStanding != 0)
                {
                    MarioObject->CurrentObjectAction = ObjectAction.Jump;

                    // If key is up, jump 2x more
                    if ((Game.Pad0 & KeypadBits.Up) == KeypadBits.Up)
                    {
                        *MarioYVelocity = -HighJumpingAcceleration;
                    }
                    else
                    {
                        *MarioYVelocity = -JumpingAcceleration;
                    }

                    Sound.PlaySound(0);
                }
            }
        }

        SnesObject.CollidMap(index);

        // Update the animation
        switch (MarioObject->CurrentObjectAction)
        {
            case ObjectAction.Walk:
                Walk(index);
                break;

            case ObjectAction.Fall:
                Fall(index);
                break;

            case ObjectAction.Jump:
                Jump(index);
                break;
        }

        SnesObject.UpdateXy(index);

        // Check boundaries
        if (*MarioXCoords <= 0)
        {
            *MarioXCoords = 0;
        }

        if (*MarioYCoords <= 0)
        {
            *MarioYCoords = 0;
        }

        MarioXMapDepth = (ushort)*MarioXCoords;
        MarioYMapDepth = (ushort)*MarioYCoords;
        Sprite.Buffer[0].XPosition = (short)(MarioXMapDepth - Map.CameraXPosition);
        Sprite.Buffer[0].YPosition = (short)(MarioYMapDepth - Map.CameraYPosition);

        Sprite.Dynamic16Draw(0);
        Map.UpdateCamera(MarioXMapDepth, MarioYMapDepth);
    }

    private static void Walk(byte index)
    {
        // Update animation
        Flip++;
        if ((Flip & 3) == 3)
        {
            MarioFidx++;
            MarioFidx = (byte)(MarioFidx & 1);
            Sprite.Buffer[0].FrameId = (ushort)(MarioFlip + MarioFidx);
            Sprite.Buffer[0].Refresh = 1;
        }

        // Check if we are still walking or not with the velocity property
        if (*MarioYVelocity != 0)
        {
            MarioObject->CurrentObjectAction = ObjectAction.Fall;
        }
        else if (*MarioXVelocity == 0 && (*MarioYVelocity == 0))
        {
            MarioObject->CurrentObjectAction = ObjectAction.Stand;
        }
    }

    private static void Fall(byte index)
    {
        if (*MarioYVelocity == 0)
        {
            MarioObject->CurrentObjectAction = ObjectAction.Stand;
            Sprite.Buffer[0].FrameId = 6;
            Sprite.Buffer[0].Refresh = 1;
        }
    }

    private static void Jump(byte index)
    {
        if (Sprite.Buffer[0].FrameId != 1)
        {
            Sprite.Buffer[0].FrameId = 1;
            Sprite.Buffer[0].Refresh = 1;
        }

        if (*MarioYVelocity >= 0)
        {
            MarioObject->CurrentObjectAction = ObjectAction.Fall;
        }
    }
}