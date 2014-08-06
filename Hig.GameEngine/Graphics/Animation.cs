namespace Hig.GameEngine.Graphics
{
    using System.Collections.Generic;
    using System;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// The anomation.
    /// This class plays frames that are sets in the horizontal line.
    /// </summary>
    public class Animation : Sprite
    {
        private readonly Counter _counter;

        public bool IsLoop { get; set; }

        private float _speed;
        /// <summary>
        /// Gets or sets the speed of the animation (a frame per sec.).
        /// </summary>
        public float Speed
        {
            get { return _speed; }

            set
            {
                _speed = value;
                _counter.Interval = 1000 / _speed;
            }
        }

        #region " Ctors "

        public Animation(Texture2D texture, int frameWidth, int frameHeight, float speed, Point drawOffset, Color color, bool isLoop = false)
            :base(texture, frameWidth, frameHeight, drawOffset, color)
        {
            _counter = new Counter(Next, 0);
            Speed = speed;
            IsLoop = isLoop;
        }

        public Animation(Texture2D texture, int frameWidth, int frameHeight, float speed, Point drawOffset, bool isLoop = false)
            : this(texture, frameWidth, frameHeight, speed, drawOffset, Color.White, isLoop) { }

        public Animation(Texture2D texture, int frameWidth, int frameHeight, float speed, Color color, bool isLoop = false)
            : this(texture, frameWidth, frameHeight, speed, new Point(), color, isLoop) { }

        public Animation(Texture2D texture, int frameWidth, int frameHeight, float speed, bool isLoop = false)
            : this(texture, frameWidth, frameHeight, speed, new Point(), Color.White, isLoop) { }

        #endregion

        /// <summary>
        /// Goes to the next frame.
        /// </summary>
        public void Next()
        {
            ushort x = X;

            if (x + 1 >= FrameXCount)
            {
                if (IsLoop)
                    x = 0;
            }
            else
            {
                x++;
            }

            SetFrame(x, Y);
        }

        /// <summary>
        /// Plays the animation.
        /// </summary>
        /// <param name="milliseconds">The elapsed time in milliseconds.</param>
        public void Play(uint milliseconds)
        {
            _counter.Update(milliseconds);
        }

        /// <summary>
        /// Goes to the initial state of the animation.
        /// </summary>
        public void Reset()
        {
            SetFrame(0, Y);
        }

        /// <summary>
        /// Creates a new copy of the animation.
        /// </summary>
        /// <returns>New copy of the animation.</returns>
        public Animation Copy()
        {
            return new Animation(Texture, FrameWidth, FrameHeight, Speed, DrawOffset, Color, IsLoop);
        }
    }
}
