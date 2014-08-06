namespace Hig.Compiler.SyntacticAnalysis
{
    using System;
    using Hig.Compiler.LexicalAnalysis;

    public interface IParserRule
    {
        string Name { get; }
        string Pattern { get; }
        Func<Token[], ISyntaxNode> CreateNode { get; }
    }
}
