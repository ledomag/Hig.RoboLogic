using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hig.Compiler;
using System.IO;
using System.Reflection;

namespace Hig.ScriptEngine
{
    public class DataValue : IDataValue
    {
        public DataType Type { get; protected set; }
        protected object _value;

        public virtual bool IsCompleted
        {
            get { return true; }
        }

        public virtual IDataValue Value
        {
            get { return this; }
        }

        public virtual Operation Parent { get; set; }

        public DataValue(object value)
        {
            Set(value);
        }

        public DataValue()
            : this(null) { }

        public virtual void Update() { }
        public virtual void SetStatus(bool isFinished) { }

        public virtual bool Check(out string message)
        {
            message = string.Empty;

            return true;
        }

        public virtual void Set(object value)
        {
            if (value != null)
            {
                DataValue tmpValue = value as DataValue;

                if (tmpValue != null)
                {
                    Set(tmpValue._value);

                    return;
                }

                TypeCode typeCode = System.Type.GetTypeCode(value.GetType());

                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        Type = DataType.Boolean;
                        _value = (bool)value;
                        break;
                    case TypeCode.Byte:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.SByte:
                    case TypeCode.Single:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        Type = DataType.Number;
                        _value = Convert.ToDouble(value);
                        break;
                    case TypeCode.Char:
                    case TypeCode.String:
                        Type = DataType.String;
                        _value = (string)value;
                        break;
                }

                return;
            }

            Type = DataType.NaN;
            _value = null;
        }

        public override bool Equals(object obj)
        {
            if(obj is DataValue)
                return _value.Equals(((DataValue)obj)._value);

            return _value.Equals(obj); 
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override string ToString()
        {
            if (Type == DataType.NaN)
                return DataType.NaN.ToString();

            return _value.ToString();
        }

        public static implicit operator bool(DataValue value)
        {
            if (value.Type == DataType.String)
                return value.ToString() != String.Empty;

            return Convert.ToBoolean(value._value);
        }

        public static implicit operator double(DataValue value)
        {
            return Convert.ToDouble(value._value);
        }

        public static implicit operator string(DataValue value)
        {
            return value._value.ToString();
        }

        public virtual void FromBytes(Script script, byte[] buffer)
        {
            using (MemoryStream ms  = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    string assemblyName = Assembly.GetEntryAssembly().GetName().FullName;
                    Type = (DataType)br.ReadByte();

                    switch (Type)
                    {
                        case DataType.NaN:
                            _value = null;
                            break;
                        case DataType.Boolean:
                            _value = br.ReadBoolean();
                            break;
                        case DataType.Number:
                            _value = br.ReadDouble();
                            break;
                        case DataType.String:
                            _value = br.ReadString();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public byte[] ToBytes(Script script)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    bw.Write((byte)Type);

                    switch (Type)
                    {
                        case DataType.Boolean:
                            bw.Write((bool)_value);
                            break;
                        case DataType.Number:
                            bw.Write((double)_value);
                            break;
                        case DataType.String:
                            bw.Write((string)_value);
                            break;
                        default:
                            break;
                    }

                    return ms.ToArray();
                }
            }
        }
    }
}
