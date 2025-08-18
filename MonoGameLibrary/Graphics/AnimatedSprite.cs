using System;
using Microsoft.Xna.Framework;

namespace MonoGameLibrary.Graphics;

public class AnimatedSprite : Sprite
{
    private int _currtenFrame;
    private TimeSpan _elapsed;
    private Animation _animation;

    public Animation Animation
    {
        get => _animation;
        set
        {
            _animation = value;
            Region = _animation.Frames[0];
        }
    }

    public AnimatedSprite() { }

    public AnimatedSprite(Animation animation)
    {
        Animation = animation;
    }

    public void Update(GameTime gameTime)
    {
        _elapsed += gameTime.ElapsedGameTime;
        if (_elapsed >= _animation.Delay)
        {
            _elapsed -= _animation.Delay;
            _currtenFrame++;

            if (_currtenFrame >= _animation.Frames.Count)
            {
                _currtenFrame = 0;
            }
            Region = _animation.Frames[_currtenFrame];
        }
    }

}