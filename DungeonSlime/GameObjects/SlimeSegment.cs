using Microsoft.Xna.Framework;

namespace DungeonSlime.GameObjects;

public struct SlimeSegment
{
    // The position this slime segment is at before the movement cycle occurs 
    public Vector2 At;

    // The position this slime segment should move to during the next movement cycle
    public Vector2 To;

    // The direction this slime segment is moving
    public Vector2 Direction;

    // The opposite direction this slime segment is moving
    public Vector2 ReverseDirection => new Vector2(-Direction.X, -Direction.Y);
}