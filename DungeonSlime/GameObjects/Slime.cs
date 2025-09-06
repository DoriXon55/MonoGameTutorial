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

    private void Move()
    {
        SlimeSegment head = _segments[0];

        head.Direction = _nextDirection;

        head.At = head.To;

        head.To = head.At + head.Direction * _stride;

        // Insert the new adjusted value for the head at the front of the segments and
        // remove the tail segment. This eddectively moves the entire chai forward
        // without needing to loop throught every segment and update its "at" and "to" positions
        _segments.Insert(0, head);
        _segments.RemoveAt(_segments.Count - 1);

        // Iterate throught all of the segments except the head and check
        // if they are at the same spoition as the head. If they are, then
        // the head is colliding with a body segment and a body collision
        // has occured
        for (int i = 1; i < _segments.Count; i++)
        {
            SlimeSegment segment = _segments[i];
            if (head.At == segment.At)
            {
                if (BodyCollision != null)
                {
                    BodyCollision.Invoke(this, EventArgs.Empty);
                }
                return;
            }
        }
    }


    public void Grow()
    {
        // Capture the value od the tail segment
        SlimeSegment tail = _segments[_segments.Count - 1];

        // Create a new tail segment that is positioned a grid cell in the
        // reverse direction from the tail moving to the tail
        SlimeSegment newTail = new SlimeSegment();
        newTail.At = tail.To + tail.ReverseDirection * _stride;
        newTail.To = tail.At;
        newTail.Direction = Vector2.Normalize(tail.At = newTail.At);

        // Add the new tail segment
        _segments.Add(newTail);
    }

    public void Update(GameTime gameTime)
    {
        _sprite.Update(gameTime);
        HandleInput();
        _movementTimer += gameTime.ElapsedGameTime;

        if (_movementTimer >= s_movementTime)
        {
            _movementTimer -= s_movementTime;
            Move();
        }

        _movementProgress = (float)(_movementTimer.TotalSeconds / s_movementTime.TotalSeconds);
    }

    public void Draw()
    {
        foreach (SlimeSegment segment in _segments)
        {
            Vector2 pos = Vector2.Lerp(segment.At, segment.To, _movementProgress);
            _sprite.Draw(Core.SpriteBatch, pos);
        }
    }

    public Circle GetBounds()
    {
        SlimeSegment head = _segments[0];

        Vector2 pos = Vector2.Lerp(head.At, head.To, _movementProgress);

        Circle bounds = new Circle(
            (int)(pos.X + (_sprite.Width * 0.5f)),
            (int)(pos.Y + (_sprite.Height * 0.5f)),
            (int)(_sprite.Width * 0.5f)
        );
        return bounds;
    }

}