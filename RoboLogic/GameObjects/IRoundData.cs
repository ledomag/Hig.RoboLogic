namespace RoboLogic.GameObjects
{
    using Hig.GameEngine;

    public interface IRoundData
    {
        Position Position { get; set; }

        byte[] ToBytes();
        void FromBytes(byte[] buffer);
    }
}
