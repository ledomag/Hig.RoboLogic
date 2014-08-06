namespace Hig.ScriptEngine
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Hig.ScriptEngine.Operations;

    public abstract class Operation : IOperand
    {
        protected List<IOperand> _operands = new List<IOperand>();
        public virtual IOperand[] Operands
        {
            get { return _operands.ToArray(); }
        }

        public virtual bool IsCompleted { get; set; }
        public virtual IDataValue Value { get; set; }
        public virtual Operation Parent { get; set; }

        protected abstract void Action(Operation operation, IDataValue[] values);

        private void SetOperandParent(IOperand operand, Operation parent)
        {
            Operation operation = operand as Operation;

            if (operation != null)
                operation.Parent = parent;
        }

        public virtual Function GetParentFunction()
        {
            return Parent.GetParentFunction();
        }

        public virtual Instruction GetParentInstruction()
        {
            return Parent.GetParentInstruction();
        }

        public virtual void Add(IOperand operand)
        {
            SetOperandParent(operand, this);
            _operands.Add(operand);
        }

        public virtual void Insert(int index, IOperand operand)
        {
            SetOperandParent(operand, this);
            _operands.Insert(index, operand);
        }

        public virtual bool Remove(IOperand operand)
        {
            SetOperandParent(operand, null);

            return _operands.Remove(operand);
        }

        public virtual void RemoveAt(int index)
        {
            SetOperandParent(_operands[index], this);
            _operands.RemoveAt(index);
        }

        public virtual void Update()
        {
            bool isEnd = true;

            for (int i = 0; i < _operands.Count; i++)
            {
                IOperand operand = _operands[i];

                if (!operand.IsCompleted)
                {
                    operand.Update();

                    if (!operand.IsCompleted)
                    {
                        isEnd = false;
                        break;
                    }
                }
            }

            if (isEnd && !IsCompleted)
            {
                IDataValue[] values = new IDataValue[_operands.Count];

                for (int i = 0; i < _operands.Count; i++)
                    values[i] = _operands[i].Value;

                Action(this, values);
            }
        }

        protected virtual void Load(Script script, BinaryReader reader) { }

        protected virtual void Save(Script script, BinaryWriter writer) { }

        public virtual void FromBytes(Script script, byte[] buffer)
        {
            using (MemoryStream ms = new MemoryStream(buffer))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    Load(script, br);

                    ushort count = br.ReadUInt16();

                    for (int i = 0; i < count; i++)
                    {
                        string typeName = br.ReadString();
                        ushort size = br.ReadUInt16();

                        IOperand operand = (IOperand)Activator.CreateInstance(Script.AssemblyName, typeName).Unwrap();
                        operand.FromBytes(script, br.ReadBytes(size));
                        Add(operand);
                    }
                }
            }
        }

        public virtual byte[] ToBytes(Script script)
        {
            using (MemoryStream ms  = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(ms))
                {
                    Save(script, bw);

                    bw.Write((ushort)_operands.Count);

                    for (int i = 0; i < _operands.Count; i++)
                    {
                        bw.Write(_operands[i].GetType().ToString());

                        byte[] buffer = _operands[i].ToBytes(script);
                        bw.Write((ushort)buffer.Length);
                        bw.Write(buffer);
                    }

                    return ms.ToArray();
                }
            }
        }
    }
}
