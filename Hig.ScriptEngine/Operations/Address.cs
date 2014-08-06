namespace Hig.ScriptEngine.Operations
{
    using Hig.Compiler;

    public class Address : Operation
    {
        protected override void Action(Operation operation, IDataValue[] values)
        {
            operation.Value = ((Function)GetParentFunction()).GetVariable((DataValue)values[0]);
            operation.IsCompleted = true;
        }
    }
}
