namespace Hig.GameEngine.GameObjects
{
    using Hig.GameEngine.Graphics;

    public interface IGameObject
    {
        Position Position { get; set; }
        Animation Animation { get; set; }
    }
}
