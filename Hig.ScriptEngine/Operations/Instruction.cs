namespace Hig.ScriptEngine.Operations
{
    using System.IO;

    using Hig.Compiler;

    public class Instruction : Operation
    {
        public ushort LineNumber { get; set; }
        public ushort[] Address { get; set; }

        public override Instruction GetParentInstruction()
        {
            return this;
        }

        protected override void Action(Operation operation, IDataValue[] values)
        {
            Value = (values.Length == 0 || values.Length > 1) ? new DataValue(null) : values[0];
            IsCompleted = true;
        }

        protected override void Load(Script script, BinaryReader reader)
        {
            LineNumber = reader.ReadUInt16();
            Address = new ushort[reader.ReadUInt16()];

            for (ushort i = 0; i < Address.Length; i++)
                Address[i] = reader.ReadUInt16();

            base.Load(script, reader);
        }

        protected override void Save(Script script, BinaryWriter writer)
        {
            writer.Write(LineNumber);
            writer.Write((ushort)Address.Length);

            for (int i = 0; i < Address.Length; i++)
                writer.Write(Address[i]);

            base.Save(script, writer);
        }
    }
}
