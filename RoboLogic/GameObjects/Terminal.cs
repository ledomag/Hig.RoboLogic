namespace RoboLogic.GameObjects
{
    using System.IO;
    using Microsoft.Xna.Framework;

    using Hig.GameEngine;
    using Hig.GameEngine.GameObjects;
    using Hig.GameEngine.Graphics;
    using Hig.ScriptEngine;

    public class Terminal : StaticObject, IRoundData, IUsableObject
    {
        private bool _status = false;
        public bool Status
        {
            get { return _status; }

            set
            {
                _status = value;

                if (Animation != null)
                    Animation.SetFrame(y: (ushort)((value) ? 1 : 0));
            }
        }

        public IDataValue Value { get; set; }

        public Terminal(Animation animation, Position position)
        {
            Animation = animation;
            Position = position;
            Value = new DataValue(null);
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

                    bw.Write(Status);

                    byte[] buffer = Value.ToBytes(null);
                    bw.Write(buffer.Length);
                    bw.Write(buffer);
                }

                return ms.ToArray();
            }
        }

        public void FromBytes(byte[] buffer)
        {
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

                    Status = br.ReadBoolean();

                    Value.FromBytes(null, br.ReadBytes(br.ReadInt32()));
                }
            }
        }
    }
}
