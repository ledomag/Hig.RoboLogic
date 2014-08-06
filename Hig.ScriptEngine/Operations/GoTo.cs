namespace Hig.ScriptEngine.Operations
{
    using System;
    using Hig.Compiler;
    using System.IO;

    public class GoTo : Instruction
    {
        protected Script _script;
        public ushort[] ToAddress { get; set; }

        public GoTo(Script script, ushort[] address = null)
        {
            if (script == null)
                throw new ArgumentNullException("script");

            _script = script;
            ToAddress = address;
        }

        public GoTo() { }

        private int FindDifferenceIndex(ushort[] address1, ushort[] address2)
        {
            int length = (address1.Length > address2.Length) ? address1.Length : address2.Length;

            for (int i = 0; i < length; i++)
                if (i >= address1.Length || i >= address2.Length || address1[i] != address2[i])
                    return i;

            return -1;
        }

        private Instruction GetInstructionByAddress(IOperand operand, ushort[] address)
        {
            Instruction instruction = operand as Instruction;

            if (instruction != null && FindDifferenceIndex(instruction.Address, address) == -1)
                return instruction;

            Operation operation = operand as Operation;

            if (operation != null)
                for (int i = 0; i < operation.Operands.Length; i++)
                    if ((instruction = GetInstructionByAddress(operation.Operands[i], address)) != null)
                        return instruction;

            return null;
        }

        private bool IsBetween(ushort[] address, ushort[] address1, ushort[] address2)
        {
            int index1 = FindDifferenceIndex(address, address1);
            int index2 = FindDifferenceIndex(address, address2);

            if (index1 > address.Length - 1)
                index1 = address.Length - 1;
            else if (index1 > address1.Length - 1)
                index1 = address1.Length - 1;

            if (index2 > address.Length - 1)
                index2 = address.Length - 1;
            else if (index2 > address2.Length - 1)
                index2 = address2.Length - 1;

            return (index1 == -1 || address[index1] >= address1[index1]) && (index2 == -1 || address[index2] <= address2[index2]);
        }

        private void SetStatus(IOperand operand, bool status, ushort[] address1, ushort[] address2, Func<int?, Operation, int?> iterator)
        {
            Operation operation = operand as Operation;

            if (operation != null)
            {
                if (IsBetween(operation.GetParentInstruction().Address, address1, address2))
                {
                    if (!status || operation != _commonInstruction)
                        operation.IsCompleted = status;

#if DEBUG

                    string str = String.Empty;
                    var adr = operation.GetParentInstruction().Address;

                    for (int i = 0; i < adr.Length; i++)
                        str += adr[i] + " ";

                    Console.WriteLine(str);

#endif

                    int? index = null;

                    while ((index = iterator(index, operation)) != null)
                        SetStatus(operation.Operands[(int)index], status, address1, address2, iterator);
                }
            }
        }

        private Instruction _commonInstruction = null;

        protected override void Action(Operation operation, IDataValue[] values)
        {
            bool status = true;
            _commonInstruction = null;

            if (ToAddress != null)
            {
                ushort[] currentAddress = GetParentInstruction().Address;

                // Find parents addresses. It will start points.
                int index = FindDifferenceIndex(currentAddress, ToAddress);

#if DEBUG

                string str = String.Empty;

                for (int i = 0; i < currentAddress.Length; i++)
                    str += currentAddress[i] + " ";

                str += ":";

                for (int i = 0; i < ToAddress.Length; i++)
                    str += ToAddress[i] + " ";

                Console.WriteLine(str);

#endif

                if (index >= 0)
                {
                    ushort[] commonAddress = new ushort[index];
                    Array.Copy(currentAddress, commonAddress, commonAddress.Length);

                    _commonInstruction = GetInstructionByAddress(_script, commonAddress);

                    if (_commonInstruction != null)
                    {
                        ushort[] fromAddress = currentAddress;
                        ushort[] toAddress = ToAddress;
                        Func<int?, Operation, int?> iterator;

                        #region " Change places of addresses "

                        // If fromAddress > toAddress or toAddress is the parent of fromAddress
                        if (index < fromAddress.Length && index < toAddress.Length && fromAddress[index] > toAddress[index] ||
                            toAddress.Length < fromAddress.Length && fromAddress[toAddress.Length - 1] == toAddress[toAddress.Length - 1])
                        {
                            status = false;

                            ushort[] tmpAddress = fromAddress;
                            fromAddress = toAddress;
                            toAddress = tmpAddress;

                            iterator = (i, op) =>
                                {
                                    if (i == null && op.Operands.Length > 0)
                                        i = op.Operands.Length - 1;
                                    else if (i > 0)
                                        i--;
                                    else
                                        return null;

                                    return i;
                                };
                        }
                        else
                        {
                            iterator = (i, op) =>
                                {
                                    if (i == null && op.Operands.Length > 0)
                                        i = 0;
                                    else if (i < op.Operands.Length - 1)
                                        i++;
                                    else
                                        return null;

                                    return i;
                                };
                        }

                        #endregion

                        SetStatus(_commonInstruction, status, fromAddress, toAddress, iterator);
                    }
                }
            }

            operation.Value = new DataValue(null);
            operation.IsCompleted = status;
        }

        protected override void Load(Script script, BinaryReader reader)
        {
            _script = script;

            ToAddress = new ushort[reader.ReadUInt16()];

            for (ushort i = 0; i < ToAddress.Length; i++)
                ToAddress[i] = reader.ReadUInt16();

            base.Load(script, reader);
        }

        protected override void Save(Script script, BinaryWriter writer)
        {
            writer.Write((ushort)ToAddress.Length);

            for (int i = 0; i < ToAddress.Length; i++)
                writer.Write(ToAddress[i]);

            base.Save(script, writer);
        }
    }
}
