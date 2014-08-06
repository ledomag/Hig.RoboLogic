namespace RoboLogic.GameObjects
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Microsoft.Xna.Framework;

    using Hig.GameEngine;
    using Hig.GameEngine.GameObjects;
    using Hig.GameEngine.Graphics;
    using Hig.ScriptEngine;

    public class Button : StaticObject, IRoundData, IStatusObject
    {
        protected Map _map;

        public bool Status { get; set; }
        public List<Point> LinkedPositions { get; protected set; }

        public IDataValue Value
        {
            get { return new DataValue(Status); }
        }

        public Button(Animation animation, Position position, Map map)
        {
            if (map == null)
                throw new ArgumentNullException("map");

            Animation = animation;
            Position = position;

            _map = map;
            LinkedPositions = new List<Point>();
        }

        private void ChangeStatusOfObjects()
        {
            foreach (var pos in LinkedPositions)
                foreach (var obj in _map[pos.X, pos.Y].GameObjects)
                    if (obj is IStatusObject)
                        ((IStatusObject)obj).Status = Status;
        }

        public void UpdateMap()
        {
            if (Animation != null)
            {
                foreach (var pos in LinkedPositions)
                {
                    var ground = _map[pos.X, pos.Y].Ground;

                    if (ground != null)
                        ground.Color = Animation.Color;
                }
            }
        }

        public virtual void Press()
        {
            Status = true;
            Animation.SetFrame(y: 1);
            ChangeStatusOfObjects();
        }

        public virtual void Release()
        {
            Status = false;
            Animation.SetFrame(y: 0);
            ChangeStatusOfObjects();
        }

        public byte[] ToBytes()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    Color color = (Animation != null) ? Animation.Color : Color.White;

                    bw.Write(color.A);
                    bw.Write(color.R);
                    bw.Write(color.G);
                    bw.Write(color.B);

                    bw.Write(LinkedPositions.Count);

                    for (int i = 0; i < LinkedPositions.Count; i++)
                    {
                        bw.Write(LinkedPositions[i].X);
                        bw.Write(LinkedPositions[i].Y);
                    }
                }

                return ms.ToArray();
            }
        }

        public void FromBytes(byte[] buffer)
        {
            LinkedPositions.Clear();

            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    Color color = new Color();

                    color.A = br.ReadByte();
                    color.R = br.ReadByte();
                    color.G = br.ReadByte();
                    color.B = br.ReadByte();

                    if (Animation != null)
                        Animation.Color = color;

                    int count = br.ReadInt32();

                    for (int i = 0; i < count; i++)
                        LinkedPositions.Add(new Point(br.ReadInt32(), br.ReadInt32()));
                }
            }

            UpdateMap();
        }
    }
}
