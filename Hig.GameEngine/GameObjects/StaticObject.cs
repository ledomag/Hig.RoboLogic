namespace Hig.GameEngine.GameObjects
{
    using Hig.GameEngine.Graphics;
    using System;

    public class StaticObject : IGameObject
    {
        public virtual Position Position { get; set; }
        public virtual Animation Animation { get; set; }

        protected short _angle = 0;
        public virtual short Angle
        {
            get { return _angle; }
            
            set 
            { 
                _angle = (short)(((value >= 0) ? 0 : 360) + value % 360);
                Animation.SetFrame(Animation.X ,(ushort)(_angle * Animation.FrameYCount / 360));
            }
        }
    }
}
