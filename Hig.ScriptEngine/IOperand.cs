namespace Hig.ScriptEngine
{
    using System.IO;

    using Hig.Compiler;

    public interface IOperand : ISyntaxNode
    {
        bool IsCompleted { get; }
        IDataValue Value { get; }

        void Update();
        byte[] ToBytes(Script script);
        void FromBytes(Script script, byte[] buffer);
    }
}
