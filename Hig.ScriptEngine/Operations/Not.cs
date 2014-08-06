namespace Hig.ScriptEngine.Operations
{
    using Hig.Compiler;

    public class Not : Operation
    {
        protected override void Action(Operation operation, IDataValue[] values)
        {
            operation.Value = new DataValue(!(DataValue)values[0]);
            operation.IsCompleted = true;
        }
    }
}
