namespace Hig.GameEngine.GameObjects
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    using Hig.GameEngine.Graphics;

    public class Cell
    {
        public const ushort Width = 32;
        public const ushort Height = 32;

        public Point Position { get; set; }
        public Animation Ground { get; set; }
        public List<IGameObject> GameObjects { get; set; }

        public Cell()
        {
            GameObjects = new List<IGameObject>();
        }
    }
}
