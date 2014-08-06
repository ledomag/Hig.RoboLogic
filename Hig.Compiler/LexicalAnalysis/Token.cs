namespace Hig.Compiler.LexicalAnalysis
{
    using System;

    public struct Token
    {
        public ushort LineNumber;
        public string Name;
        public string Attribute;
        public ISyntaxNode Node;

        public Token(ushort lineNumber, string name = null, string attribute = null, ISyntaxNode node = null)
        {
            LineNumber = lineNumber;
            Name = name;
            Attribute = attribute;
            Node = node;
        }
    }
}
