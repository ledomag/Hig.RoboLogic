namespace Hig.Compiler.LexicalAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public class Lexer
    {
        protected ILexerRule[] _lexems;
        protected string _newLine;
        protected string[] _splitterPatterns;
        protected ShieldSymbols[] _shieldSymbols;
        protected ShieldSymbols? _activeShieldSympol;

        public Lexer(ILexerRule[] lexems, string newLine, ShieldSymbols[] shieldSymbols, params string[] splitterPatterns)
        {
            if (lexems == null)
                throw new ArgumentNullException("lexem");
            if (String.IsNullOrEmpty(newLine))
                throw new ArgumentException("newLine");
            if (splitterPatterns == null)
                throw new ArgumentNullException("splitterPatterns");
            if (splitterPatterns.Length == 0)
                throw new ArgumentException("Argument splitterPatterns does not contain values.");

            _lexems = lexems;
            _newLine = newLine;
            _shieldSymbols = shieldSymbols;
            _splitterPatterns = splitterPatterns;
        }

        protected virtual Token? FindLexem(ushort lineNumber, string text)
        {
            for (int i = 0; i < _lexems.Length; i++)
                if (_lexems[i].Check(text))
                    return new Token(lineNumber, _lexems[i].Name, text);

            return null;
        }

        public virtual Token[] Analyze(string text)
        {
            int index = -1;
            ushort lineNumber = 1;
            StringBuilder sequence = new StringBuilder();
            _activeShieldSympol = null;
            List<Token> tokens = new List<Token>();

            while (++index < text.Length)
            {
                // Count number of lines;
                if (index + _newLine.Length < text.Length && text.Substring(index, _newLine.Length) == _newLine)
                    lineNumber++;

                sequence.Append(text[index]);

                // Check shield symbols.
                for (int i = 0; i < _shieldSymbols.Length; i++)
                {
                    if (_activeShieldSympol == null && _shieldSymbols[i].BeginSymbol == text[index])
                        _activeShieldSympol = _shieldSymbols[i];
                    else if (_shieldSymbols[i].EndSymbol == text[index])
                        _activeShieldSympol = null;
                }

                bool isSplitter = (_activeShieldSympol == null) ? Regex.IsMatch(text[index].ToString(), _splitterPatterns[0]) : false;

                // The end of the lexem was found.
                if (index == text.Length - 1 || isSplitter)
                {
                    Token? token = null;

                    // Remove the end symbol from the sequence if it is a splitter.
                    if (isSplitter)
                        sequence = sequence.Remove(sequence.Length - 1, 1);

                    // Check sequence.
                    if ((token = FindLexem(lineNumber, sequence.ToString())) != null)
                        tokens.Add((Token)token);

                    bool added = false;
                    int newIndex = index;

                    // Check end symbols.
                    for (int i = _splitterPatterns.Length - 1; i >= 0; i--)
                    {
                        if (index + i < text.Length)
                        {
                            string splitterText = text.Substring(index, i + 1);

                            if (Regex.IsMatch(splitterText, _splitterPatterns[i]) && (token = FindLexem(lineNumber, splitterText)) != null)
                            {
                                if (tokens.Count == 0 || !tokens.Last().Attribute.Contains(((Token)token).Attribute) && added || !added)
                                {
                                    tokens.Add((Token)token);
                                    added = true;

                                    if (index + i > newIndex)
                                        newIndex = index + i;
                                }
                            }
                        }
                    }

                    sequence.Clear();
                    index = newIndex;
                }
            }

            return tokens.ToArray();
        }
    }
}
