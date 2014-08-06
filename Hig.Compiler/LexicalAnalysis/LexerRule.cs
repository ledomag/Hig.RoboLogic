namespace Hig.Compiler.LexicalAnalysis
{
    using System;
    using System.Linq;

    public class LexerRule : ILexerRule
    {
        protected string[] _lexems;

        public string Name { get; protected set; }

        public LexerRule(string name, params string[] lexems)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("name");
            if (lexems == null)
                throw new ArgumentNullException("lexems");

            Name = name;
            _lexems = lexems;
        }

        public LexerRule(string lexem)
        {
            if (String.IsNullOrEmpty(lexem))
                throw new ArgumentException("lexem");

            Name = lexem;
            _lexems = new[] { lexem };
        }

        public bool Check(string text)
        {
            return _lexems.Contains(text);
        }
    }
}
