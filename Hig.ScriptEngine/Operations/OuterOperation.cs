namespace Hig.ScriptEngine.Operations
{
    using System;
    using System.IO;

    using Hig.Compiler;

    public class OuterOperation : Operation
    {
        protected string _actionName = String.Empty;
        protected CallAction _action = null;

        public OuterOperation(string name)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException(name);

            _actionName = name;
            _action = Script.GetOperation(name).Action;
        }

        public OuterOperation() { }

        protected override void Action(Operation operation, IDataValue[] values)
        {
            _action(operation, values);
        }

        protected override void Load(Script script, BinaryReader reader)
        {
            _actionName = reader.ReadString();
            _action = Script.GetOperation(_actionName).Action;

            base.Load(script, reader);
        }

        protected override void Save(Script script, BinaryWriter writer)
        {
            writer.Write(_actionName);

            base.Save(script, writer);
        }
    }

    public delegate void CallAction(Operation operation, IDataValue[] values);
}
