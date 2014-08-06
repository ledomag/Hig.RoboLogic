namespace RoboLogic
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.Xna.Framework;

    using Hig.GameEngine;
    using Hig.GameEngine.GameObjects;
    using Hig.ScriptEngine;
    using RoboLogic.GameObjects;

    public sealed class Round
    {
        public struct RoundData
        {
            public string TypeName;
            public Position Position;
            public byte[] Buffer;
        }

        public List<string> ImagePaths { get; set; }
        public UniversalColor Player1Color { get; set; }
        public UniversalColor Player2Color { get; set; }
        public Point MapSize { get; set; }
        public List<IGameObject> GameObjects { get; private set; }
        public Dictionary<int, IDataValue> WinConditions { get; private set; }

        public RoundData[] Data { get; private set; }

        public Round()
        {
            ImagePaths = new List<string>();
            MapSize = new Point();
            GameObjects = new List<IGameObject>();
            WinConditions = new Dictionary<int, IDataValue>();
        }

        public void Save(string path)
        {
            IRoundData[] objects = GameObjects.Where(o => o is IRoundData).Cast<IRoundData>().ToArray();

            using (Stream stream = File.OpenWrite(path))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(ImagePaths.Count);

                    for (int i = 0; i < ImagePaths.Count; i++)
                        bw.Write(ImagePaths[i]);

                    bw.Write(MapSize.X);
                    bw.Write(MapSize.Y);
                    bw.Write(objects.Length);

                    for (int i = 0; i < objects.Length; i++)
                    {
                        bw.Write(objects[i].Position.X);
                        bw.Write(objects[i].Position.Y);
                        bw.Write(objects[i].GetType().ToString());

                        byte[] buffer = objects[i].ToBytes();

                        bw.Write(buffer.Length);
                        bw.Write(buffer);
                    }

                    bw.Write(WinConditions.Count);

                    foreach (var condition in WinConditions)
                    {
                        bw.Write(condition.Key);

                        byte[] buffer = condition.Value.ToBytes(null);

                        bw.Write(buffer.Length);
                        bw.Write(buffer);
                    }
                }
            }
        }

        public static Round Load(string path)
        {
            Round round = new Round();

            using (Stream stream = File.OpenRead(path))
            {
                using (BinaryReader br = new BinaryReader(stream))
                {
                    int count = br.ReadInt32();

                    for (int i = 0; i < count; i++)
                        round.ImagePaths.Add(br.ReadString());

                    round.MapSize = new Point(br.ReadInt32(), br.ReadInt32());
                    round.Data = new RoundData[br.ReadInt32()];

                    for (int i = 0; i < round.Data.Length; i++)
                    {
                        RoundData data = new RoundData();
                        data.Position = new Position(br.ReadInt32(), br.ReadInt32());
                        data.TypeName = br.ReadString();
                        data.Buffer = br.ReadBytes(br.ReadInt32());

                        round.Data[i] = data;
                    }

                    count = br.ReadInt32();

                    for (int i = 0; i < count; i++)
                    {
                        int index = br.ReadInt32();

                        DataValue value = new DataValue();
                        value.FromBytes(null, br.ReadBytes(br.ReadInt32()));

                        round.WinConditions.Add(index, value);
                    }
                }
            }

            return round;
        }
    }
}
