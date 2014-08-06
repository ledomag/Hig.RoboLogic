namespace Hig.GameEngine.GameObjects
{
    using System;
    using Hig.GameEngine.Graphics;

    public class Unit : StaticObject
    {
        private const byte stepLength = 2;
        private Counter _counter;
        private double _angleInRadian;

        private float _speed;

        public float Speed
        {
            get { return _speed; }

            set
            {
                _speed = value;
                _counter.Interval = 1000 / _speed;
            }
        }

        public override short Angle
        {
            get
            {
                return base.Angle;
            }
            set
            {
                base.Angle = value;
                _angleInRadian = base.Angle * Math.PI / 180;
            }
        }

        public Unit(Animation animation, Position position, float speed)
        {
            _counter = new Counter(Move, 0);
            Animation = animation;
            Position = position;
            Speed = speed;
        }

        private void Move()
        {
            Position.X += (int)Math.Round(Math.Cos(_angleInRadian) * stepLength);
            Position.Y += (int)Math.Round(Math.Sin(_angleInRadian) * stepLength);
        }

        public virtual void Move(uint milliseconds)
        {
            _counter.Update(milliseconds);
        }
    }
}
