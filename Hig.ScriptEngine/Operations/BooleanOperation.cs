namespace Hig.ScriptEngine.Operations
{
    using System;
    using System.IO;

    using Hig.Compiler;

    public class BooleanOperation : Operation
    {
        protected BooleanOperationType _type;

        public BooleanOperation(BooleanOperationType type = BooleanOperationType.None)
        {
            _type = type;
        }

        private DataValue Calc(Operation operation, Func<bool, bool, bool> mathOperation, DataValue value1, DataValue value2)
        {
            return new DataValue(mathOperation((bool)value1, (bool)value2));
        }

        protected override void Action(Operation operation, IDataValue[] values)
        {
            DataValue result = new DataValue(null); // NaN.
            DataValue value1 = (DataValue)values[0];
            DataValue value2 = (DataValue)values[1];

            if (value1.Type != DataType.String && value1.Type != DataType.NaN && value2.Type != DataType.String && value2.Type != DataType.NaN)
            {
                switch (_type)
                {
                    case BooleanOperationType.Or:
                        result = Calc(operation, (v1, v2) => v1 | v2, value1, value2);
                        break;
                    case BooleanOperationType.And:
                        result = Calc(operation, (v1, v2) => v1 & v2, value1, value2);
                        break;
                }
            }

            operation.Value = result;
            operation.IsCompleted = true;
        }

        protected override void Load(Script script, BinaryReader reader)
        {
            _type = (BooleanOperationType)reader.ReadByte();

            base.Load(script, reader);
        }

        protected override void Save(Script script, BinaryWriter writer)
        {
            writer.Write((byte)_type);

            base.Save(script, writer);
        }
    }
}
