namespace RoboLogic.GameObjects
{
    using System;
    using System.IO;

    using Hig.GameEngine;
    using Hig.GameEngine.GameObjects;
    using Hig.GameEngine.Graphics;
    using Hig.ScriptEngine;

    public sealed class Robot : Unit, IRoundData
    {
        private Direction _direction;
        private Position _wayPoint = null;

        public Operation MoveOperation { get; set; }

        public Direction Direction
        {
            get { return _direction; }

            set
            {
                _direction = value;
                Angle = (short)_direction;
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
                if (value != 0 && value != 90 && value != 180 && value != 270)
                    throw new ArgumentOutOfRangeException("Angle can be 0 or 90 or 180 or 270 degree.");

                base.Angle = value;
                _direction = (Direction)value;
            }
        }

        private readonly Script _script;

        public Script Script
        {
            get { return _script; }
        }

        public bool IsMoving { get; private set; }

        public Robot(Animation animation, Position position, float speed)
            :base(animation, position, speed)
        {
            _script = new Script();
            Direction = Direction.Right;
            IsMoving = false;
        }

        public Position CalcNextWayPoint()
        {
            int x = Position.X;
            int y = Position.Y;

            if (Direction == Direction.Left)
                x -= Cell.Width * 2;    // Multiply by 2 because width in isometric.
            else if (Direction == Direction.Right)
                x += Cell.Width * 2;
            else if (Direction == Direction.Down)
                y += Cell.Height;
            else if (Direction == Direction.Up)
                y -= Cell.Height;

            return new Position(x, y);
        }

        public void StartMove()
        {
            IsMoving = true;
            _wayPoint = CalcNextWayPoint();
        }

        public override void Move(uint milliseconds)
        {
            if (IsMoving)
            {
                base.Move(milliseconds);
                Animation.Play(milliseconds);

                if (Direction == Direction.Left && Position.X <= _wayPoint.X ||
                    Direction == Direction.Right && Position.X >= _wayPoint.X ||
                    Direction == Direction.Down && Position.Y >= _wayPoint.Y ||
                    Direction == Direction.Up && Position.Y <= _wayPoint.Y)
                {
                    IsMoving = false;
                    Position.X = _wayPoint.X;
                    Position.Y = _wayPoint.Y;
                    Animation.Reset();
                }
            }
        }

        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                    bw.Write((ushort)Direction);

                return ms.ToArray();
            }
        }

        public void FromBytes(byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
                using (BinaryReader br =new BinaryReader(ms))
                    Direction = (Direction)br.ReadUInt16();
        }
    }
}
