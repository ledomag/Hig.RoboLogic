namespace Hig.Compiler.LexicalAnalysis
{
    using System;
    using System.Text.RegularExpressions;

    public class LexerRegexRule : ILexerRule
    {
        protected string _pattern;

        public string Name { get; protected set; }

        public LexerRegexRule(string name, string pattern)
        {
            if (String.IsNullOrEmpty(name))
                throw new ArgumentException("name");
            if (String.IsNullOrEmpty(pattern))
                throw new ArgumentException("pattern");

            Name = name;
            _pattern = pattern;
        }

        public bool Check(string text)
        {
            return Regex.IsMatch(text, _pattern);
        }
    }
}
