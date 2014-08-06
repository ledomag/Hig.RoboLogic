namespace Hig.Compiler.LexicalAnalysis
{
    public struct ShieldSymbols
    {
        public char BeginSymbol;
        public char EndSymbol;

        public ShieldSymbols(char beginSymbol, char endSymbol)
        {
            BeginSymbol = beginSymbol;
            EndSymbol = endSymbol;
        }

        public ShieldSymbols(char symbol)
            : this(symbol, symbol) { }
    }
}
