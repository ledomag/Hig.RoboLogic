namespace Hig.ScriptEngine
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    using Hig.Compiler.LexicalAnalysis;
    using Hig.Compiler.SyntacticAnalysis;
    using Hig.ScriptEngine.Operations;

    public sealed class Script : Function
    {
        public struct RegistredOperation
        {
            public CallAction Action;
            public bool HasOpernad;
        }

        private static Dictionary<string, RegistredOperation> _registeredOperations = new Dictionary<string, RegistredOperation>();

        private Lexer _lexer;
        private Parser _parser;
        List<string> _variableNames = new List<string>();   // Variables for compile time.
        private List<Error> _errors = new List<Error>();

        public Error[] Errors
        {
            get { return _errors.ToArray(); }
        }

        internal readonly static string AssemblyName;

        static Script()
        {
            AssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        }

        public Script()
        {
            #region " Init of lexer "

            ILexerRule[] lexerRules = 
            {
                new LexerRule("declare", "var"),
                new LexerRule("if"),
                new LexerRule("else"),
                new LexerRule("for"),
                new LexerRule("do"),
                new LexerRule("while"),
                new LexerRegexRule("string", "^\".*\"$"),
                new LexerRegexRule("digit", @"^\d+$|^\d*\.\d+$"),
                new LexerRule("boolean", "true", "false"),
                new LexerRegexRule("command1", GetCommandLexem(false)),
                new LexerRegexRule("command2", GetCommandLexem(true)),
                new LexerRule("assign", "="),
                new LexerRule("operation1", "*", "/", "%"),
                new LexerRule("operation2", "+", "-"),
                new LexerRule("bool_operation1", "&", "&&"),
                new LexerRule("bool_operation2", "|", "||"),
                new LexerRule("not", "!"),
                new LexerRule("inequality", "==", "!=", ">", "<", ">=", "<="),
                new LexerRule("{"),
                new LexerRule("}"),
                new LexerRule("("),
                new LexerRule(")"),
                new LexerRule("semicolon", ";"),
                new LexerRegexRule("literal", "^\\S+$")
            };

            _lexer = new Lexer(
                lexerRules,
                "\r\n",
                new[] { new ShieldSymbols('"') },
                @" |\t|\+|\-|/|%|\*|;|\(|\)|\{|\}|<|>|=|!|&|\||\n|\r",
                @"^&&$|^\|\|$|^>=$|^<=$|^==$|^!=$");

            #endregion

            #region " Init of parser "

            IParserRule[] parserRules = 
            {
                new ParserRule("#value", "(^digit|^string|^boolean)", 1, CreateValue),
                new ParserRule("#declare", @"(^declare) (\S+)", 2, CreateDeclare),
                new ParserRule("#address", "(#declare)", 1, CreateAddress),
                new ParserRule("#address", "(^literal)", 1, CreateAddress2),
                new ParserRule("#command", "(^command1)", 1, CreateCommand),

                // Math or bool expressions or value or address into ().
                new ParserRecursiveRule("#exp_br", @"\((?>[^\(\)]+|\((?<Br>)|\)(?<-Br>))*(?(Br)(?!))\)", DropParentheses,
                    new ParserRule("#value", "#value", 1, CreateValue),
                    new ParserRule("#address", "#address", 1, CreateAddress2),
                    new ParserRule("#bool_exp", @"(not) (\S+)", 2, CreateNot),
                    new ParserRule("#exp", @"(\S+) (operation1) (\S+)", 3, CreateMathExp),
                    new ParserRule("#bool_exp", @"(\S+) (bool_operation1) (\S+)", 3, CreateBoolExp),
                    new ParserRule("#exp", @"(\S+) (operation2) (\S+)", 3, CreateMathExp),
                    new ParserRule("#bool_exp", @"(\S+) (bool_operation2) (\S+)", 3, CreateBoolExp),
                    new ParserRule("#bool_exp", @"(\S+) (inequality) (\S+)", 3, CreateCompare)),

                // Math expressions without ().
                new ParserRule("#exp", @"(\S+) (operation1) (\S+)", 3, CreateMathExp),
                new ParserRule("#exp", @"(\S+) (operation2) (\S+)", 3, CreateMathExp),
                // Bool expressions without ().
                new ParserRule("#bool_exp", @"(not) (\S+)", 2, CreateNot),
                new ParserRule("#bool_exp", @"(\S+) (bool_operation1) (\S+)", 3, CreateBoolExp),
                new ParserRule("#bool_exp", @"(\S+) (bool_operation2) (\S+)", 3, CreateBoolExp),
                new ParserRule("#bool_exp", @"(\S+) (inequality) (\S+)", 3, CreateCompare),

                new ParserRule("#command", @"(command2) (\S+)", 2, CreateCommand2),
                new ParserRule("#assign", @"(\S+) (assign) (\S+)", 3, CreateAssign),
                //new ParserRule("#instruction", @"(\S+) (semicolon)", 2, CreateInstruction),

                // {} and conditions, instructions into {}.
                new ParserRecursiveRule("#scope", @"\{(?>[^\{\}]+|\{(?<Br>)|\}(?<-Br>))*(?(Br)(?!))\}", CreateScope,
                    new ParserRule("#loop", @"(do) (\S+) (while) (\S+) (\S+)", 5, CreateDoWhile),
                    //new ParserRule("#instruction", "#instruction", 1, CreateInstruction),
                    new ParserRule("#instruction", @"(\S+) (semicolon)", 2, CreateInstruction),
                    new ParserRule("#loop", @"(while) (\S+) (\S+)", 3, CreateWhile),
                    new ParserRule("#condition", @"(if) (\S+) (\S+)", 3, CreateConditionIf),
                    new ParserRule("#condition", @"(else) (\S+)", 2, CreateConditionElse))
            };

            _parser = new Parser(parserRules);

            #endregion
        }

        #region " Registration of users operations "

        public static RegistredOperation GetOperation(string name)
        {
            return _registeredOperations[name];
        }

        public static bool RegisterOperation(string name, CallAction action, bool hasOperand = false)
        {
            if (!_registeredOperations.ContainsKey(name))
            {
                _registeredOperations.Add(name, new RegistredOperation() { Action = action, HasOpernad = hasOperand });

                return true;
            }

            return false;
        }

        public static bool UnregisterOperation(string name)
        {
            if (!_registeredOperations.ContainsKey(name))
            {
                _registeredOperations.Remove(name);

                return true;
            }

            return false;
        }

        #endregion

        private string GetCommandLexem(bool hasOperand)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var item in _registeredOperations)
            {
                if (hasOperand && item.Value.HasOpernad || !hasOperand && !item.Value.HasOpernad)
                {
                    sb.Append("^");
                    sb.Append(item.Key);
                    sb.Append("$|");
                }
            }

            string str = sb.ToString();

            return str.Substring(0, str.Length - 2);
        }

        private void AddError(Error error)
        {
            if (!_errors.Contains(error))
                _errors.Add(error);
        }

        private void CheckTokenName(Token token, string message, params string[] values)
        {
            if (!values.Contains(token.Name))
            {
#if DEBUG
                AddError(new Error(token.LineNumber, String.Format("{0} \"{1}\".", message, token.Attribute)));
#elif !DEBUG
                AddError(new Error(token.LineNumber, message));
#endif
            }
        }

        #region " Create operand "

        private IOperand CreateConditionElse(Token[] tokens)
        {
            CheckTokenName(tokens[1], Error.ScopeExpectedMessage, "#scope", "#condition");

            Condition op;

            if (tokens[1].Name == "#condition")
            {
                op = new Condition(ConditionType.ElseIf);
                Condition condition = (Condition)tokens[1].Node;

                op.Add(condition.Operands[0]);
                op.Add(condition.Operands[1]);
            }
            else
            {
                op = new Condition(ConditionType.Else);
                op.Add((IOperand)tokens[1].Node);
            }

            return op;
        }

        private IOperand CreateConditionIf(Token[] tokens)
        {
            CheckTokenName(tokens[1], Error.InvalidIfMessage, "#exp_br");
            CheckTokenName(tokens[2], Error.ScopeExpectedMessage, "#scope");

            Condition op = new Condition(ConditionType.If);
            op.Add((IOperand)tokens[1].Node);
            op.Add((IOperand)tokens[2].Node);

            return op;
        }

        private IOperand CreateDoWhile(Token[] tokens)
        {
            CheckTokenName(tokens[1], Error.ScopeExpectedMessage, "#scope");
            CheckTokenName(tokens[3], Error.InvalidWhileMessage, "#exp_br");
            CheckTokenName(tokens[4], Error.SemicolonExpectedMessage, "semicolon");

            return new DoWhile((IOperand)tokens[1].Node, (IOperand)tokens[3].Node);
        }

        private IOperand CreateWhile(Token[] tokens)
        {
            CheckTokenName(tokens[1], Error.InvalidWhileMessage, "#exp_br");
            CheckTokenName(tokens[2], Error.ScopeExpectedMessage, "#scope");

            While op = new While();
            op.Add((IOperand)tokens[1].Node);
            op.Add((IOperand)tokens[2].Node);

            return op;
        }

        private IOperand CreateValue(Token[] tokens)
        {
            bool b;
            object obj = null;
            double d;
            string str = tokens[0].Attribute;

            if (Double.TryParse(str, out d))
                obj = d;
            else if (Boolean.TryParse(str, out b))
                obj = b;
            else
                obj = str.Replace("\"", String.Empty);  // Remove double quotes.

            return new DataValue(obj);
        }

        private IOperand CreateAddress(Token[] tokens)
        {
            Address address = new Address();
            address.Add((IOperand)tokens[0].Node);

            return address;
        }

        private IOperand CreateAddress2(Token[] tokens)
        {
            Address address = new Address();
            address.Add(new DataValue(tokens[0].Attribute));

            return address;
        }

        private IOperand CreateCommand(Token[] tokens)
        {
            if (!_registeredOperations.ContainsKey(tokens[0].Attribute))
                return null;

            return new OuterOperation(tokens[0].Attribute) { Parent = this };
        }

        private IOperand CreateCommand2(Token[] tokens)
        {
            if (!_registeredOperations.ContainsKey(tokens[0].Attribute))
                return null;

            CheckTokenName(tokens[1], Error.InvalidCommandOperandMessage, "#value", "#command", "#exp", "#exp_br", "#address");

            Operation op = new OuterOperation(tokens[0].Attribute);
            op.Parent = this;
            op.Add((IOperand)tokens[1].Node);

            return op;
        }

        private IOperand DropParentheses(Token[] tokens)
        {
            if (tokens.Length == 1)
                return (IOperand)tokens[0].Node;
            else if (tokens.Length == 3)
                return (IOperand)tokens[1].Node;

            return null;
        }

        private IOperand CreateMathExp(Token[] tokens)
        {
            string[] values = { "#value", "#address", "#exp", "#exp_br" };
            CheckTokenName(tokens[0], Error.InvalidMathOperandMessage, values);
            CheckTokenName(tokens[2], Error.InvalidMathOperandMessage, values);

            MathOperationType type = MathOperationType.None;

            switch (tokens[1].Attribute)
            {
                case "+":
                    type = MathOperationType.Plus;
                    break;
                case "-":
                    type = MathOperationType.Minus;
                    break;
                case "*":
                    type = MathOperationType.Multiply;
                    break;
                case "/":
                    type = MathOperationType.Divide;
                    break;
                case "%":
                    type = MathOperationType.Mod;
                    break;
            }

            MathOperation op = new MathOperation(type);
            op.Add((IOperand)tokens[0].Node);
            op.Add((IOperand)tokens[2].Node);

            return op;
        }

        private IOperand CreateBoolExp(Token[] tokens)
        {
            string[] values = { "#bool_exp", "#bool_exp_br", "#value", "#address", "#exp", "#exp_br" };
            CheckTokenName(tokens[0], Error.InvalidBoolOperandMessage, values);
            CheckTokenName(tokens[2], Error.InvalidBoolOperandMessage, values);

            BooleanOperationType type = BooleanOperationType.None;

            string strType = tokens[1].Attribute;

            if (strType == "&" || strType == "&&")
                type = BooleanOperationType.And;
            else if (strType == "|" || strType == "||")
                type = BooleanOperationType.Or;

            BooleanOperation op = new BooleanOperation(type);
            op.Add((IOperand)tokens[0].Node);
            op.Add((IOperand)tokens[2].Node);

            return op;
        }

        private IOperand CreateNot(Token[] tokens)
        {
            CheckTokenName(tokens[1], Error.InvalidBoolOperandMessage, "#value", "#address", "#exp", "#exp_br", "#bool_exp", "#bool_exp_br");

            Not op = new Not();
            op.Add((IOperand)tokens[1].Node);

            return op;
        }

        private IOperand CreateCompare(Token[] tokens)
        {
            string[] values = { "#bool_exp", "#bool_exp_br", "#value", "#address", "#exp", "#exp_br" };
            CheckTokenName(tokens[0], Error.InvalidComparisonOperandMessage, values);
            CheckTokenName(tokens[2], Error.InvalidComparisonOperandMessage, values);

            CompareOperationType type = CompareOperationType.None;

            switch (tokens[1].Attribute)
            {
                case "==":
                    type = CompareOperationType.Equal;
                    break;
                case "!=":
                    type = type = CompareOperationType.NotEqual;
                    break;
                case "<":
                    type = type = CompareOperationType.Less;
                    break;
                case "<=":
                    type = type = CompareOperationType.LessOrEqual;
                    break;
                case ">":
                    type = type = CompareOperationType.More;
                    break;
                case ">=":
                    type = type = CompareOperationType.MoreOrEqual;
                    break;
            }

            CompareOperation op = new CompareOperation(type);
            op.Add((IOperand)tokens[0].Node);
            op.Add((IOperand)tokens[2].Node);

            return op;
        }

        private IOperand CreateAssign(Token[] tokens)
        {
            CheckTokenName(tokens[0], Error.InvalidAssignLeftMessage, "#address", "#declare");
            CheckTokenName(tokens[2], Error.InvalidAssignRightMessage, "#value", "#address", "#exp", "#exp_br", "#bool_exp", "#bool_exp_br", "#command");

            Assign op = new Assign();
            op.Add((IOperand)tokens[0].Node);
            op.Add((IOperand)tokens[2].Node);

            return op;
        }

        private IOperand CreateDeclare(Token[] tokens)
        {
            CheckTokenName(tokens[1], Error.InvalidDeclarationNameMessage, "literal");
            string nameVariable = tokens[1].Attribute;

            if (nameVariable.Length == 0 || !Regex.IsMatch(nameVariable[0].ToString(), "^[a-zA-Z_]$"))
            {
                AddError(new Error(tokens[1].LineNumber, String.Format(
                    "{0} \"{1}\". Variable name must begin with a letter or the symbol \"_\".",
                    Error.InvalidVariableNameMessage,
                    nameVariable)));
            }
            else if (!Regex.IsMatch(nameVariable, "^[a-zA-Z_0-9]+$"))
            {
                AddError(new Error(tokens[1].LineNumber, String.Format(
                    "{0} \"{1}\". Variable name can consist from letters, numbers and the symbol \"_\".",
                    Error.InvalidVariableNameMessage,
                    nameVariable)));
            }

            Declare op = new Declare();
            op.Add(new DataValue(nameVariable));

            return op;
        }

        private IOperand CreateInstruction(Token[] tokens)
        {
            CheckTokenName(tokens[0], Error.UnknownInstructionMessage, "#address", "#assign", "#condition", "#command", "#instruction", "#do_while");

            if (tokens[0].Name == "#instruction")
                return (IOperand)tokens[0].Node;

            Instruction op = new Instruction() { LineNumber = tokens[0].LineNumber };
            op.Add((IOperand)tokens[0].Node);

            return op;
        }

        private IOperand CreateScope(Token[] tokens)
        {
            ushort lineNumber = 0;

            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].Node != null)
                {
                    lineNumber = tokens[i].LineNumber;
                    break;
                }
            }

            Instruction scope = new Instruction() { LineNumber = lineNumber };

            for (int i = 1; i < tokens.Length - 1; i++)
            {
                IOperand operand = (IOperand)tokens[i].Node;

                if (operand == null || !(operand is Instruction || operand is Condition))
                    AddError(new Error(tokens[i].LineNumber, Error.SemicolonExpectedMessage));

                scope.Add(operand);
            }

            return scope;
        }

        #endregion

        private bool PreProcess(Token[] tokens, int startIndex, int endIndex)
        {
            bool result = true;

            #region " Check brackets "

            ushort[] lines = new ushort[2];
            int[] balances = new int[lines.Length];

            for (int i = startIndex; i < endIndex; i++)
            {
                if (tokens[i].Name == "(")
                    balances[0]++;
                else if (tokens[i].Name == ")")
                    balances[0]--;
                else if (tokens[i].Name == "{")
                    balances[1]++;
                else if (tokens[i].Name == "}")
                    balances[1]--;

                for (int j = 0; j < balances.Length; j++)
                    if (balances[j] < 0 && lines[j] == 0)
                        lines[j] = tokens[i].LineNumber;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i] != 0)
                {
                    result = false;
                    AddError(new Error(lines[i], String.Format("{0} \"{1}\".", Error.InvalidBracketMessage, tokens[lines[i]].Name)));
                }
            }

            if (balances[0] != 0)
            {
                result = false;
                AddError(new Error(0, String.Format("{0} \"()\".", Error.InvalidBracketsCountMessage)));
            }
            if (balances[1] != 0)
            {
                result = false;
                AddError(new Error(0, String.Format("{0} \"{1}\".", Error.InvalidBracketsCountMessage, "{}")));
            }

            #endregion

            return result;
        }

        private void PostProcess(ushort lineNumber, Operation operation)
        {
            ushort instructionIndex = 0;
            ushort[] goToAddress = null;
            ConditionType conditionType = ConditionType.None;
            IOperand[] operands = operation.Operands;

            // Get line number.
            if (operation is Instruction)
                lineNumber = ((Instruction)operation).LineNumber;

            for (int i = 0; i < operands.Length; i++)
            {
                IOperand operand = operands[i];

                if ((!(operand is Condition) || ((Condition)operand).Type == ConditionType.If) && goToAddress != null)
                {
                    goToAddress = null;
                    operand = new Nop();
                    operation.Insert(i, operand);
                    operands = operation.Operands;
                }

                #region " Addressing "

                Instruction instruction = operand as Instruction;

                if (instruction != null)
                {
                    ushort[] address = instruction.Parent.GetParentInstruction().Address;
                    instruction.Address = new ushort[address.Length + 1];
                    Array.Copy(address, instruction.Address, address.Length);
                    instruction.Address[instruction.Address.Length - 1] = instructionIndex++;
                }

                #endregion

                #region " Check "

                if (operand is Declare)
                {
                    Declare op = (Declare)operand;

                    if (op.Operands.Length > 0)
                    {
                        string name = (string)(DataValue)op.Operands[0];

                        if (!_variableNames.Contains(name))
                            _variableNames.Add(name);
                        else
                            AddError(new Error(lineNumber, String.Format("{0} \"{1}\".", Error.InvalidVariableIsDefinedMessage, name)));
                    }
                    else
                        AddError(new Error(lineNumber, Error.InvalidDeclarationNameMessage));
                }
                else if (operand is Address)
                {
                    Address op = (Address)operand;

                    if (op.Operands.Length == 0)
                    {
                        AddError(new Error(lineNumber, Error.UnknownVariableMessage));
                    }
                    else if (op.Operands[0] is DataValue)
                    {
                        DataValue value = (DataValue)op.Operands[0];

                        if (!_variableNames.Contains(value))
                        {
                            string message = (op.Parent is Assign || op.Parent is MathOperation || op.Parent is BooleanOperation)
                                ? Error.UnknownVariableMessage
                                : Error.UnknownInstructionMessage;

                            AddError(new Error(lineNumber, String.Format("{0} \"{1}\".", message, (string)value)));
                        }
                    }
                }
                else if (operand is DoWhile)
                {
                    DoWhile op = (DoWhile)operand;
                    ((Instruction)((Condition)op.Operands[1]).Operands[1]).Add(new GoTo(this, op.Address));
                }
                else if (operand is While)
                {
                    While op = (While)operand;

                    Operation body = op.Operands[1] as Operation;

                    if (body != null)
                        body.Add(new GoTo(this, op.Address));
                    else
                        AddError(new Error(op.LineNumber, Error.InvalidWhileMessage));
                }
                else if (operand is Condition)
                {
                    Condition op = (Condition)operand;

                    if ((op.Type == ConditionType.ElseIf || op.Type == ConditionType.Else) &&
                        (conditionType == ConditionType.Else || conditionType == ConditionType.None))
                    {
                        AddError(new Error(lineNumber, Error.InvalidIfMessage));
                    }

                    if (goToAddress == null)
                    {
                        int j;

                        for (j = i + 1; j < operands.Length; j++)
                        {
                            Condition tmpOp = operands[j] as Condition;

                            if (tmpOp == null || tmpOp.Type != ConditionType.ElseIf && tmpOp.Type != ConditionType.Else)
                                break;
                        }

                        ushort[] address = new ushort[op.Address.Length];
                        Array.Copy(op.Address, address, op.Address.Length);
                        address[address.Length - 1] = (ushort)j;

                        goToAddress = address;
                    }

                    Instruction body = null;
                    int chaildIndex = (op.Type == ConditionType.Else) ? 0 : 1;

                    if (op.Operands.Length > chaildIndex && (body = op.Operands[chaildIndex] as Instruction) != null)
                        body.Add(new GoTo(this, goToAddress));

                    conditionType = op.Type;
                }

                #endregion

                if (operand is Operation)
                    PostProcess(lineNumber, (Operation)operand);
            }
        }

        public void Compile(string text)
        {
#if DEBUG
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
#endif

            _operands.Clear();
            _errors.Clear();
            _variableNames.Clear();

            // Lexical analysis.
            Token[] tokens = _lexer.Analyze("{" + text + "}");

#if DEBUG
            stopwatch.Stop();
            Console.WriteLine("lexem number: " + tokens.Length);
            Console.WriteLine("lexer msec: " + stopwatch.ElapsedMilliseconds);
            stopwatch.Restart();
#endif

            if (PreProcess(tokens, 1, tokens.Length - 1))
            {
                // Syntactic analysis.
                var operands = _parser.Parse(tokens);

                Instruction instruction = null;

                if (operands.Length == 1 && (instruction = operands[0] as Instruction) != null)
                {
                    instruction.Address = new[] { (ushort)0 };

                    PostProcess(1, instruction);
                    Add(instruction);
                }
                else if (_errors.Count == 0)
                {
                    AddError(new Error(0, Error.UnknownErrorMessage));
                }
            }

            _isInit = true;
            IsCompleted = (_errors.Count > 0) ? true : false;

#if DEBUG
            stopwatch.Stop();
            Console.WriteLine("parser msec: " + stopwatch.ElapsedMilliseconds);
#endif
        }

        public void Stop()
        {
            IsCompleted = true;
            _operands.Clear();
        }

        public override void Update()
        {
            // Return if script has errors or if it is completed.
            if (_errors.Count > 0 || IsCompleted)
                return;

            base.Update();
        }

        public override void FromBytes(Script script, byte[] buffer)
        {
            IsCompleted = true;
            _operands.Clear();

            base.FromBytes(script, buffer);

            IsCompleted = false;
        }

        public void FromBytes(byte[] buffer)
        {
            FromBytes(this, buffer);
        }

        public byte[] ToBytes()
        {
            return base.ToBytes(this);
        }
    }
}
