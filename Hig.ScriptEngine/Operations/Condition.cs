namespace Hig.ScriptEngine.Operations
{
    using System.IO;

    using Hig.Compiler;

    public class Condition : Instruction
    {
        public ConditionType Type { get; protected set; }

        public Condition(ConditionType type = ConditionType.None)
        {
            Type = type;
        }

        protected override void Action(Operation operation, IDataValue[] values)
        {
            operation.Value = new DataValue(null);
            operation.IsCompleted = true;
        }

        public override void Update()
        {
            if (Operands.Length >= 2)
            {
                if (Operands[0].IsCompleted)
                {
                    DataValue value = (DataValue)Operands[0].Value;
                    Instruction op = (Instruction)Operands[1];

                    if (!(bool)value)
                    {
                        op.IsCompleted = true;
                        op.Value = new DataValue(null);
                    }

                    base.Update();
                }
                else
                {
                    Operands[0].Update();
                }
            }
            else
            {
                base.Update();
            }
        }

        protected override void Load(Script script, BinaryReader reader)
        {
            Type = (ConditionType)reader.ReadByte();

            base.Load(script, reader);
        }

        protected override void Save(Script script, BinaryWriter writer)
        {
            writer.Write((byte)Type);

            base.Save(script, writer);
        }
    }
}
