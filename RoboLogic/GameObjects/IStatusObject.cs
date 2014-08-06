namespace RoboLogic.GameObjects
{
    using Hig.ScriptEngine;

    public interface IStatusObject
    {
        bool Status { get; set; }
        IDataValue Value { get; }
    }
}
