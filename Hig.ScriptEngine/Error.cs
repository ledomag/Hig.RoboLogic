namespace Hig.ScriptEngine
{
    public struct Error
    {
        #region " Error messages "

        public const string InvalidIfMessage = "Invalid \"if\" instruction.";
        public const string InvalidWhileMessage = "Invalid \"while\" instruction.";
        public const string InvalidCommandOperandMessage = "Invalid operand of command.";
        public const string InvalidMathOperandMessage = "Invalid operand of math expression.";
        public const string InvalidBoolOperandMessage = "Invalid operand of boolean expression.";
        public const string InvalidComparisonOperandMessage = "Invalid operand of comparison.";
        public const string InvalidAssignLeftMessage = "Invalid left side of assign.";
        public const string InvalidAssignRightMessage = "Invalid right side of assign.";
        public const string InvalidVariableIsDefinedMessage = "Variable is already defined.";
        public const string InvalidDeclarationNameMessage = "Invalid name of declaration.";
        public const string InvalidVariableNameMessage = "Invalid name of variable.";
        public const string UnknownInstructionMessage = "Unknown instruction.";
        public const string UnknownVariableMessage = "Unknown variable.";
        public const string SemicolonExpectedMessage = "; expected.";
        public const string ScopeExpectedMessage = "{} expected.";
        public const string UnknownErrorMessage = "Unknown error.";
        public const string InvalidBracketsCountMessage = "Uneven brackets number.";
        public const string InvalidBracketMessage = "Invalid bracket.";

        #endregion

        private ushort _lineNumber;
        public ushort LineNumber
        {
            get { return _lineNumber; }
            set { _lineNumber = value; }
        }

        private string _text;
        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public Error(ushort lineNumber, string text)
        {
            _lineNumber = lineNumber;
            _text = text;
        }
    }
}
