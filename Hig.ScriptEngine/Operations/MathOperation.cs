namespace Hig.ScriptEngine.Operations
{
    using System;
    using System.IO;

    using Hig.Compiler;

    public class MathOperation : Operation
    {
        protected MathOperationType _type;

        public MathOperation(MathOperationType type = MathOperationType.None)
        {
            _type = type;
        }

        private DataValue Calc(Operation operation, Func<double, double, double> mathOperation, DataValue value1, DataValue value2)
        {
            return new DataValue(mathOperation((double)value1, (double)value2));
        }

        protected override void Action(Operation operation, IDataValue[] values)
        {
            DataValue result = new DataValue(null); // NaN.
            DataValue value1 = (DataValue)values[0];
            DataValue value2 = (DataValue)values[1];

            if (value2.Type == DataType.Number && value1.Type == DataType.Boolean)
                value1.Set(((bool)value1) ? 1d : 0d);
            if (value1.Type == DataType.Number && value2.Type == DataType.Boolean)
                value2.Set(((bool)value2) ? 1d : 0d);

            if (value1.Type == DataType.Number && value2.Type == DataType.Number)
            {
                switch (_type)
                {
                    case MathOperationType.Plus:
                        result = Calc(operation, (v1, v2) => v1 + v2, value1, value2);
                        break;
                    case MathOperationType.Minus:
                        result = Calc(operation, (v1, v2) => v1 - v2, value1, value2);
                        break;
                    case MathOperationType.Multiply:
                        result = Calc(operation, (v1, v2) => v1 * v2, value1, value2);
                        break;
                    case MathOperationType.Divide:
                        result = Calc(operation, (v1, v2) => v1 / v2, value1, value2);
                        break;
                    case MathOperationType.Mod:
                        result = Calc(operation, (v1, v2) => v1 % v2, value1, value2);
                        break;
                }
            }
            else if ((value1.Type == DataType.String || value2.Type == DataType.String) && _type == MathOperationType.Plus)
            {
                result = new DataValue(value1.ToString() + value2.ToString());
            }

            operation.Value = result;
            operation.IsCompleted = true;
        }

        protected override void Load(Script script, BinaryReader reader)
        {
            _type = (MathOperationType)reader.ReadByte();

            base.Load(script, reader);
        }

        protected override void Save(Script script, BinaryWriter writer)
        {
            writer.Write((byte)_type);

            base.Save(script, writer);
        }
    }
}
