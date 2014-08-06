namespace Hig.Compiler.LexicalAnalysis
{
    public interface ILexerRule
    {
        string Name { get; }

        bool Check(string text);
    }
}
