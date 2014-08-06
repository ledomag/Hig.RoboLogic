namespace Hig.ScriptEngine.Operations
{
    using Hig.Compiler;

    public class Assign : Operation
    {
        protected override void Action(Operation operation, IDataValue[] values)
        {
            values[0].Set(values[1]);
            operation.Value = values[0];
            operation.IsCompleted = true;
        }
    }
}
