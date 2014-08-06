namespace Hig.Compiler.SyntacticAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Hig.Compiler.LexicalAnalysis;

    public class Parser
    {
        private struct Replacement
        {
            public bool IsCompleted;
            public int StartIndex;
            public int EndIndex;
            public Token[] Tokens;

            public Replacement(int startIndex, int endIndex, Token[] tokens, bool isCompleted)
            {
                StartIndex = startIndex;
                EndIndex = endIndex;
                Tokens = tokens;
                IsCompleted = isCompleted;
            }
        }

        protected IParserRule[] _rules;

        public Parser(IParserRule[] rules)
        {
            if (rules == null)
                throw new ArgumentNullException("rules");
            if (rules.Length == 0)
                throw new ArgumentException("Argument rules does not contain values.");

            _rules = rules;
        }

        protected virtual string TokensToString(List<Token> tokens, int startIndex, int endIndex)
        {
            string sequence = String.Empty;

            for (int i = startIndex; i < endIndex; i++)
                sequence += tokens[i].Name + " ";

            return sequence;
        }

        protected virtual string TokenAttributesToString(List<Token> tokens)
        {
            string sequence = String.Empty;

            for (int i = 0; i < tokens.Count; i++)
                sequence += tokens[i].Attribute + " ";

            return sequence.Remove(sequence.Length - 1, 1);
        }

        protected virtual void CollapseTokens(List<Token> tokens, IParserRule rule, string sequence, int startIndex, int endIndex)
        {
            // Create a sublist of tokens for CreateOperand method.
            List<Token> tmpTokens = new List<Token>();

            // Fill sublist of tokens.
            for (int i = startIndex; i < endIndex; i++)
                tmpTokens.Add(tokens[i]);

            ISyntaxNode node = rule.CreateNode(tmpTokens.ToArray());

            if (node != null)
            {
                // Remove tokens.
                for (int i = endIndex - 1; i >= startIndex; i--)
                    tokens.RemoveAt(i);

                // Insert new token.
                if (tmpTokens.Count > 0)
                    tokens.Insert(startIndex, new Token(tmpTokens[0].LineNumber, rule.Name, TokenAttributesToString(tmpTokens), node));
            }
        }

        protected virtual void LinearSearch(List<Token> tokens, ParserRule rule)
        {
            ushort stepCopy;
            ushort step = stepCopy = (rule.TokenNumber == 0) ? (ushort)1 : rule.TokenNumber;
            int index = 0;
            string sequence = String.Empty;

            while (index + step <= tokens.Count)
            {
                sequence = TokensToString(tokens, index, index + step);

                if (Regex.IsMatch(sequence, rule.Pattern))
                {
                    int count = tokens.Count;
                    CollapseTokens(tokens, rule, sequence, index, index + step);

                    if (count == tokens.Count)
                        index++;
                }
                else if (rule.TokenNumber == 0)
                {
                    step += stepCopy;
                }
                else
                {
                    index++;
                    step = stepCopy;
                }
            }
        }

        protected virtual List<Token> RecursiveSearch(List<Token> tokens, ParserRecursiveRule rule)
        {
            List<Replacement> replacements = new List<Replacement>();
            string sequence = TokensToString(tokens, 0, tokens.Count);
            Match match = Regex.Match(sequence, rule.Pattern);

            while (match.Success)
            {
                int startStrIndex = match.Index;
                int endStrIndex = startStrIndex + match.Length;

                int startTokensIndex = sequence.CountWords(0, startStrIndex);
                int tokensLength = sequence.CountWords(startStrIndex, endStrIndex) + 1;
                int endtokensIndex = startTokensIndex + tokensLength - 1;

                if (tokensLength > 0)
                {
                    Replacement r = new Replacement();
                    r.StartIndex = startTokensIndex;
                    r.EndIndex = endtokensIndex;

                    var subTokens = tokens.GetRange(r.StartIndex, tokensLength);    // выражение с обрамляющими символами. Например ( 5 ).
                    var tmpSubTokens = RecursiveSearch(subTokens.GetRange(1, subTokens.Count - 2), rule);   // выражение без обрамляющих символов. Например 5.

                    if (rule.Rules.Length != 0)
                        Process(tmpSubTokens, rule.Rules);

                    tmpSubTokens.Insert(0, subTokens[0]);
                    tmpSubTokens.Add(subTokens[subTokens.Count - 1]);

                    CollapseTokens(tmpSubTokens, rule, match.Value, 0, tmpSubTokens.Count);

                    // Если что-ир было свернуто-то добавляем replacement.
                    if (subTokens.Count > tmpSubTokens.Count)
                    {
                        r.Tokens = tmpSubTokens.ToArray();
                        replacements.Add(r);
                    }
                }

                match = match.NextMatch();
            }

            #region " Make replacements "

            if (replacements.Count > 0)
            {
                List<Token> tmpTokens = new List<Token>();

                for (int i = 0; i < tokens.Count; i++)
                {
                    bool add = true;

                    for (int j = replacements.Count - 1; j >= 0; j--)
                    {
                        if (i >= replacements[j].StartIndex && i <= replacements[j].EndIndex)
                        {
                            add = false;

                            if (!replacements[j].IsCompleted)
                            {
                                for (int k = 0; k < replacements[j].Tokens.Length; k++)
                                    tmpTokens.Add(replacements[j].Tokens[k]);

                                replacements[j] = new Replacement(replacements[j].StartIndex, replacements[j].EndIndex, replacements[j].Tokens, true);
                            }
                        }
                    }

                    if (add)
                        tmpTokens.Add(tokens[i]);
                }

                tokens.Clear();

                foreach (var item in tmpTokens)
                    tokens.Add(item);
            }

            #endregion

            return tokens;
        }

        protected virtual void Process(List<Token> tokens, IParserRule[] rules)
        {
            for (int i = 0; i < rules.Length; i++)
            {
                if (rules[i] is ParserRecursiveRule)
                    RecursiveSearch(tokens, (ParserRecursiveRule)rules[i]);
                else
                    LinearSearch(tokens, (ParserRule)rules[i]);
            }
        }

        public virtual ISyntaxNode[] Parse(Token[] tokens)
        {
            List<Token> tmpTokens = new List<Token>(tokens);

            Process(tmpTokens, _rules);

            ISyntaxNode[] operands = new ISyntaxNode[tmpTokens.Count];

            for (int i = 0; i < tmpTokens.Count; i++)
                operands[i] = tmpTokens[i].Node;

            return operands;
        }
    }
}
