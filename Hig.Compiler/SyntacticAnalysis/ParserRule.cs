namespace Hig.Compiler.SyntacticAnalysis
{
    using System;
    using Hig.Compiler.LexicalAnalysis;

    public class ParserRule : IParserRule
    {
        public string Name { get; protected set; }
        public string Pattern { get; protected set; }
        public ushort TokenNumber { get; protected set; }
        public Func<Token[], ISyntaxNode> CreateNode { get; protected set; }

        public ParserRule(string name, string pattern, ushort tokenNumber, Func<Token[], ISyntaxNode> createNode)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("name");
            if (String.IsNullOrEmpty(pattern))
                throw new ArgumentException("pattern");
            if (createNode == null)
                throw new ArgumentNullException("createOperand");

            Name = name;
            Pattern = pattern;
            TokenNumber = tokenNumber;
            CreateNode = createNode;
        }

        public ParserRule(string name, string pattern, Func<Token[], ISyntaxNode> createOperand)
            : this(name, pattern, 0, createOperand) { }
    }
}
