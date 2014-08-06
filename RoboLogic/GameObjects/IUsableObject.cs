namespace RoboLogic.GameObjects
{
    using Hig.ScriptEngine;

    public interface IUsableObject : IStatusObject
    {
        IDataValue Value { get; set; }
    }
}
