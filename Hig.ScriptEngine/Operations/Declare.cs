namespace Hig.ScriptEngine.Operations
{
    using Hig.Compiler;

    public class Declare : Operation
    {
        protected override void Action(Operation operation, IDataValue[] values)
        {
            DataValue value = (DataValue)values[0];

            ((Function)GetParentFunction()).AddVariable(value, new DataValue(null));
            operation.Value = value;
            operation.IsCompleted = true;
        }
    }
}
