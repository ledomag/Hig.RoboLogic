namespace Hig.ScriptEngine.Operations
{
    using Hig.Compiler;

    public class DoWhile : Instruction
    {
        public DoWhile(IOperand instruction, IOperand expression)
        {
            Add(instruction);

            Instruction scope = new Instruction();
            Condition condition = new Condition(ConditionType.If);
            condition.Add(expression);
            condition.Add(scope);

            Add(condition);
        }
    }
}
