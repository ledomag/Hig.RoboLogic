namespace Hig.ScriptEngine.Operations
{
    using System;
    using System.IO;

    using Hig.Compiler;

    public class CompareOperation : Operation
    {
        protected CompareOperationType _type;

        public CompareOperation(CompareOperationType type = CompareOperationType.None)
        {
            _type = type;
        }

        private void CompareBoolean(Operation operation, Func<bool, bool, bool> compareOperation, DataValue value1, DataValue value2)
        {
            operation.Value = new DataValue(compareOperation(value1, value2));
            operation.IsCompleted = true;
        }

        private void CompareString(Operation operation, Func<string, string, bool> compareOperation, DataValue value1, DataValue value2)
        {
            operation.Value = new DataValue(compareOperation(value1, value2));
            operation.IsCompleted = true;
        }

        private void CompareNumeric(Operation operation, Func<double, double, bool> compareOperation, DataValue value1, DataValue value2)
        {
            operation.Value = new DataValue(compareOperation(Math.Round((double)value1, 4), Math.Round((double)value2, 4)));
            operation.IsCompleted = true;
        }

        private void SetFalse(Operation operation)
        {
            operation.Value = new DataValue(false);
            operation.IsCompleted = true;
        }

        protected override void Action(Operation operation, IDataValue[] values)
        {
            DataValue value1 = (DataValue)values[0];
            DataValue value2 = (DataValue)values[1];

            if (value1.Type == DataType.NaN || value2.Type == DataType.NaN)
            {
                operation.Value = new DataValue(null);
                operation.IsCompleted = true;
            }
            else if (value1.Type == DataType.Boolean)
            {
                switch (_type)
                {
                    case CompareOperationType.Equal:
                        CompareBoolean(operation, (v1, v2) => v1 == v2, value1, value2);
                        break;
                    case CompareOperationType.NotEqual:
                        CompareBoolean(operation, (v1, v2) => v1 != v2, value1, value2);
                        break;
                    default:
                        SetFalse(operation);
                        break;
                }
            }
            else if (value1.Type == DataType.Number)
            {
                switch (_type)
                {
                    case CompareOperationType.More:
                        CompareNumeric(operation, (v1, v2) => v1 > v2, value1, value2);
                        break;
                    case CompareOperationType.Less:
                        CompareNumeric(operation, (v1, v2) => v1 < v2, value1, value2);
                        break;
                    case CompareOperationType.Equal:
                        CompareNumeric(operation, (v1, v2) => v1 == v2, value1, value2);
                        break;
                    case CompareOperationType.NotEqual:
                        CompareNumeric(operation, (v1, v2) => v1 != v2, value1, value2);
                        break;
                    case CompareOperationType.MoreOrEqual:
                        CompareNumeric(operation, (v1, v2) => v1 >= v2, value1, value2);
                        break;
                    case CompareOperationType.LessOrEqual:
                        CompareNumeric(operation, (v1, v2) => v1 <= v2, value1, value2);
                        break;
                    default:
                        SetFalse(operation);
                        break;
                }
            }
            else if (value1.Type == DataType.String)
            {
                switch (_type)
                {
                    case CompareOperationType.Equal:
                        CompareString(operation, (v1, v2) => v1 == v2, value1, value2);
                        break;
                    case CompareOperationType.NotEqual:
                        CompareString(operation, (v1, v2) => v1 != v2, value1, value2);
                        break;
                    default:
                        SetFalse(operation);
                        break;
                }
            }
        }

        protected override void Load(Script script, BinaryReader reader)
        {
            _type = (CompareOperationType)reader.ReadByte();

            base.Load(script, reader);
        }

        protected override void Save(Script script, BinaryWriter writer)
        {
            writer.Write((byte)_type);

            base.Save(script, writer);
        }
    }
}
