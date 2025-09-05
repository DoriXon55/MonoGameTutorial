using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;

namespace DungeonSlime.GameObjects;

public class Slime
{
    private static readonly TimeSpan s_movementTime = TimeSpan.FromMilliseconds(200);
    private TimeSpan _movementTimer;
    private float _movementProgress;
    private Vector2 _nextDirection;
    private float _stride;
    private List<SlimeSegment> _segments;
    private AnimatedSprite _sprite;


    // Event that is raisedif it is detected that the head segment of the slime 
    // has collided with a body segment
    public event EventHandler BodyCollision;


    // Creates a new Slime using the specified animated sprite
    public Slime(AnimatedSprite sprite)
    {
        _sprite = sprite;
    }


    public void Initialize(Vector2 startingPosition, float stride)
    {
        _segments = new List<SlimeSegment>();

        _stride = stride;

        SlimeSegment head = new SlimeSegment();
        head.At = startingPosition;
        head.To = startingPosition + new Vector2(_stride, 0);
        _segments.Add(head);

        _nextDirection = head.Direction;
        _movementTimer = TimeSpan.Zero;
    }

    private void HandleInput()
    {
        Vector2 potentialNextDirection = _nextDirection;
        if (GameController.MoveUp())
        {
            potentialNextDirection = -Vector2.UnitY;
        }
        else if (GameController.MoveDown())
        {
            potentialNextDirection = Vector2.UnitY;
        }
        else if (GameController.MoveLeft())
        {
            potentialNextDirection = -Vector2.UnitX;
        }
        else if (GameController.MoveRight())
        {
            potentialNextDirection = Vector2.UnitX;
        }


        // Only allow direction change if it is not reversing the current direction
        // This prevents the slime from backing into itself
        float dot = Vector2.Dot(potentialNextDirection, _segments[0].Direction);
        if (dot >= 0)
        {
            _nextDirection = potentialNextDirection;
        }


    }



}