namespace Hig.GameEngine.GameObjects
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;

    using Hig.GameEngine.Graphics;

    public sealed class Map
    {
        private readonly Cell[][] _cells;
        private readonly Dictionary<IGameObject, Point> _points = new Dictionary<IGameObject, Point>();
        private readonly Dictionary<Point, IGameObject> _objects = new Dictionary<Point, IGameObject>();

        public ushort Width { get; private set; }
        public ushort Height { get; private set; }

        public Animation Animation { get; private set; }

        public Cell this[int x, int y]
        {
            get { return _cells[x][y]; }
            set { _cells[x][y] = value; }
        }

        public Cell this[Point point]
        {
            get { return _cells[point.X][point.Y]; }
            set { _cells[point.X][point.Y] = value; }
        }

        public Map(ushort width, ushort height, Animation animation)
        {
            Width = width;
            Height = height;
            Animation = animation;

            _cells = new Cell[width][];

            for (int x = 0; x < Width; x++)
            {
                _cells[x] = new Cell[Height];

                for (int y = 0; y < Height; y++)
                    _cells[x][y] = new Cell() { Ground = animation.Copy(), Position = new Point(x, y) };
            }
        }

        public Point? GetMapPosition(IGameObject gameObject)
        {
            if (_points.ContainsKey(gameObject))
                return _points[gameObject];

            return null;
        }

        public Point? CalcMapPosition(Position position)
        {
            if (position != null)
            {
                int mapX = position.X / Cell.Width / 2;
                int mapY = position.Y / Cell.Height;

                if (mapX >= 0 && mapX < Width && mapY >= 0 && mapY < Height)
                    return new Point(mapX, mapY);
            }

            return null;
        }

        public Position CalcPosition(int x, int y)
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height)
                return new Position(x * Cell.Width * 2, y * Cell.Height);

            return null;
        }

        public IGameObject[] GetGameObjects()
        {
            List<IGameObject> objects = new List<IGameObject>();

            foreach (var obj in _points)
                objects.Add(obj.Key);

            return objects.ToArray();

        }

        public bool RegistrObject(IGameObject gameObject)
        {
            if (gameObject != null)
            {
                Point? mapPos = CalcMapPosition(gameObject.Position);

                if (mapPos != null)
                {
                    UnregistrObject(gameObject);
                    _points.Add(gameObject, (Point)mapPos);
                    this[(Point)mapPos].GameObjects.Add(gameObject);

                    return true;
                }
            }

            return false;
        }

        public bool UnregistrObject(IGameObject gameObject)
        {
            if (_points.ContainsKey(gameObject))
            {
                this[_points[gameObject]].GameObjects.Remove(gameObject);
                _points.Remove(gameObject);

                return true;
            }

            return false;
        }
    }
}
