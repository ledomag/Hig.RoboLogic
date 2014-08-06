namespace Hig.Compiler.SyntacticAnalysis
{
    using System;
    using Hig.Compiler.LexicalAnalysis;

    public class ParserRecursiveRule : IParserRule
    {
        public string Name { get; protected set; }
        public string Pattern { get; protected set; }
        public Func<Token[], ISyntaxNode> CreateNode { get; protected set; }
        public IParserRule[] Rules { get; protected set; }

        public ParserRecursiveRule(string name, string pattern, Func<Token[], ISyntaxNode> createNode, params IParserRule[] rules)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("name");
            if (String.IsNullOrEmpty(pattern))
                throw new ArgumentException("pattern");
            if (createNode == null)
                throw new ArgumentNullException("createOperand");
            if (rules == null)
                throw new ArgumentNullException("rules");

            Name = name;
            Pattern = pattern;
            CreateNode = createNode;
            Rules = rules;
        }
    }
}
