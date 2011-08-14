using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FireEngine.FireMLEngine.Expr;

namespace FireEngine.FireMLEngine.Compiler
{
    /// <summary>
    /// 用来对单一表达式字串进行解析的类
    /// </summary>
    class ExpressionParserKernel
    {
        private enum OpLevel
        {
            Assign = 0,         //  =
            AndOr = 1,          //  &&, ||
            Equal = 2,          //  ==, !=
            Comparer = 3,       //  >, >=, <, <=
            AddMinus = 4,       //  +, -
            MulDiv = 5,         //  *, /
            Power = 6,          //  ^
            Not = 7,            //  !
            Null                // 
        }

        private enum TokenType
        {
            Null = 0,
            Error = 1,
            Operator,
            Int,
            Float,
            String,
            Bool,
            Variable
        }

        private CompilerKernel kernel;
        private string expr;
        private Location loc;
        private static readonly Regex intPattern = new Regex(@"^([0-9]+)", RegexOptions.Compiled);
        private static readonly Regex varPattern = new Regex(@"^\$(\w+[\w\d]*)", RegexOptions.Compiled);     //TODO: 不能以数字开头，有BUG！
        private static readonly Regex floatPattern = new Regex(@"^([0-9]*\.[0-9]+)", RegexOptions.Compiled);
        private static readonly Regex boolPattern = new Regex(@"^((true)|(false))", RegexOptions.Compiled);
        //private static Regex strPattern = new Regex("^\"(.*)\"", RegexOptions.Compiled);

        internal ExpressionParserKernel(CompilerKernel kernel, string expr, Location loc)
        {
            this.kernel = kernel;
            this.expr = expr;
            this.loc = loc;
        }

        internal Expression Parse()
        {
            return parse(0, expr.Length);
        }

        /// <summary>
        /// end表示字符串最后一个字符的后一个位置
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        private Expression parse(int begin, int end)
        {
            if (begin >= end)
            {
                kernel.IssueError(ErrorType.ExpressionSyntaxError, loc.Offset(begin));
                return null;
            }

            while (isWhiteSpace(expr[begin]))
                begin++;
            while (isWhiteSpace(expr[end - 1]))
                end--;

            int currentParenLevel;
            bool hasSideParren = true;
            while (expr[begin] == '(' && hasSideParren)
            {
                currentParenLevel = 0;
                int currentPos;
                for (currentPos = begin; currentPos < end; currentPos++)
                {
                    if (expr[currentPos] == '(') currentParenLevel++;
                    else if (expr[currentPos] == ')')
                    {
                        currentParenLevel--;
                        if (currentParenLevel == 0)
                        {
                            if (currentPos == end - 1)
                            {
                                begin++;
                                end--;
                            }
                            else
                                hasSideParren = false;
                            break;
                        }
                    }
                }

                if (currentPos == end && currentParenLevel > 0)
                {
                    kernel.IssueError(ErrorType.SingleParen, loc.Offset(begin));
                    return null;
                }
            }

            currentParenLevel = 0;
            OpLevel currentLevel = OpLevel.Null;
            OpLevel minLevel = OpLevel.Null;
            int minPos = -1;
            int minOpLength = 0;
            bool findOp = false;
            string op = null;
            MatchTokenResult matchTokenResult;

            //找到当前应处理的操作符
            for (int currentPos = begin; currentPos < end; currentPos = matchTokenResult.EndPos)
            {
                //if (expr[currentPos] == ' ' || expr[currentPos] == '\t' || expr[currentPos] == '\n'
                //    || Environment.NewLine.IndexOf(expr[currentPos]) != -1)
                //{
                //    op = null;
                //    continue;
                //}

                matchTokenResult = MatchToken(currentPos, end);
                if (matchTokenResult.TokenType == TokenType.Error)
                    return null;

                if (matchTokenResult.TokenType != TokenType.Operator)
                    continue;

                op = matchTokenResult.Matched;

                if (op != "(" && op != ")") findOp = true;
                if (op == "(")
                {
                    currentParenLevel++;
                    continue;
                }
                else if (op == ")")
                {
                    currentParenLevel--;
                    continue;
                }
                else if (currentParenLevel > 0)
                {
                    continue;
                }
                else if (currentParenLevel < 0)
                {
                    kernel.IssueError(ErrorType.SingleParen, loc.Offset(currentPos));
                    return null;
                }

                if (currentParenLevel == 0
                    &&(
                    (int)(currentLevel = getOpLevel(op)) < (int)minLevel)
                    || ((int)currentLevel == (int)minLevel && op != "=")    //=为右结合
                    )
                {
                    minLevel = currentLevel;
                    minPos = matchTokenResult.BeginPos;
                    minOpLength = op.Length;
                }
            }

            if (currentParenLevel != 0)
            {
                kernel.IssueError(ErrorType.SingleParen, loc.Offset(begin));
                return null;
            }

            if (!findOp)    //单个数据
            {
                string str = expr.Substring(begin, end - begin);
                int currentPos = begin;
                matchTokenResult = MatchToken(currentPos, end);

                switch (matchTokenResult.TokenType)
                {
                    case TokenType.Int:
                        RightValueExpr intResult = new RightValueExpr();
                        IntConst intConst = new IntConst();
                        intConst.Value = int.Parse(matchTokenResult.Matched);
                        intResult.DataType = DataType.Int;
                        intResult.RightValue = intConst;
                        intResult.Location = loc.Offset(begin);
                        return intResult;

                    case TokenType.Float:
                        RightValueExpr floatResult = new RightValueExpr();
                        FloatConst floatConst = new FloatConst();
                        floatConst.Value = float.Parse(matchTokenResult.Matched);
                        floatResult.DataType = DataType.Int;
                        floatResult.RightValue = floatConst;
                        floatResult.Location = loc.Offset(begin);
                        return floatResult;

                    case TokenType.String:
                        RightValueExpr strResult = new RightValueExpr();
                        StringConst strConst = new StringConst();
                        strConst.Value = matchTokenResult.Matched;
                        strResult.DataType = DataType.String;
                        strResult.RightValue = strConst;
                        strResult.Location = loc.Offset(begin);
                        return strResult;

                    case TokenType.Bool:
                        RightValueExpr boolResult = new RightValueExpr();
                        BoolConst boolConst = new BoolConst();
                        boolConst.Value = bool.Parse(matchTokenResult.Matched);
                        boolResult.DataType = DataType.Bool;
                        boolResult.RightValue = boolConst;
                        boolResult.Location = loc.Offset(begin);
                        return boolResult;

                    case TokenType.Variable:
                        VarRef varRef = new VarRef();
                        varRef.VarName = matchTokenResult.Matched;
                        LeftValueExpr leftValueResult = new LeftValueExpr();
                        leftValueResult.DataType = DataType.Unknown;
                        leftValueResult.Location = loc.Offset(begin);
                        leftValueResult.LeftValue = varRef;
                        return leftValueResult;

                    default:
                        kernel.IssueError(ErrorType.ExpressionSyntaxError, loc.Offset(begin));
                        return null;
                }
            }
            
            Expression left;
            Expression right;
            
            matchTokenResult = MatchToken(minPos, end);
            op = matchTokenResult.Matched;

            left = (begin != minPos) ? parse(begin, minPos) : null; //null表示单目运算符
            right = parse(matchTokenResult.EndPos, end);
            Location currentLoc = loc.Offset(begin);

            if (right == null)
                return null;

            
            switch (op)
            {
                case "=":
                    if (!(left is LeftValueExpr))
                    {
                        kernel.IssueError(ErrorType.NotLeftValue, currentLoc);
                        return null;
                    }
                    AssignExpr assignExpr = new AssignExpr();
                    assignExpr.LeftExpr = left as LeftValueExpr;
                    assignExpr.RightExpr = right;
                    assignExpr.DataType = right.DataType;
                    assignExpr.Location = currentLoc;
                    return assignExpr;

                case "&&":
                    return processBinaryLogicExpr(
                        new AndExpr(),
                        left, right, currentLoc);
                    

                case "||":
                    return processBinaryLogicExpr(
                        new OrExpr(),
                        left, right, currentLoc);

                case "==":
                    return processBinaryCmpExpr(new EquExpr(), left, right, currentLoc);

                case "!=":
                    return processBinaryCmpExpr(new NeqExpr(), left, right, currentLoc);

                case ">":
                    return processBinaryCmpExpr(new GreatExpr(), left, right, currentLoc);

                case ">=":
                    return processBinaryCmpExpr(new GreatEquExpr(), left, right, currentLoc);

                case "<":
                    return processBinaryCmpExpr(new LessExpr(), left, right, currentLoc);

                case "<=":
                    return processBinaryCmpExpr(new LessEquExpr(), left, right, currentLoc);

                case "+":
                    return processBinaryAlgoExpr(new AddExpr(), left, right, currentLoc);

                case "-":
                    if (left == null)
                    {
                        NegativeExpr negExpr = new NegativeExpr();
                        if (right.DataType == DataType.Bool || right.DataType == DataType.String)
                        {
                            kernel.IssueError(ErrorType.OprandTypeError, currentLoc);
                            return null;
                        }
                        else if (right.DataType == DataType.Int)
                        {
                            negExpr.DataType = DataType.Int;
                        }
                        else if (right.DataType == DataType.Float)
                        {
                            negExpr.DataType = DataType.Float;
                        }
                        else
                        {
                            negExpr.DataType = DataType.Unknown;
                        }

                        
                        negExpr.Op = right;
                        negExpr.Location = currentLoc;
                        return negExpr;
                    }
                    else
                    {
                        return processBinaryAlgoExpr(new SubExpr(), left, right, currentLoc);
                    }

                case "*":
                    return processBinaryAlgoExpr(new MulExpr(), left, right, currentLoc);

                case "/":
                    return processBinaryAlgoExpr(new DivExpr(), left, right, currentLoc);

                case "^":
                    return processBinaryAlgoExpr(new PowExpr(), left, right, currentLoc);

                case "!":
                    if (left != null)
                    {
                        kernel.IssueError(ErrorType.ExpressionSyntaxError, currentLoc);
                        return null;
                    }
                    else
                    {
                        NotExpr notExpr = new NotExpr();
                        notExpr.DataType = DataType.Bool;
                        notExpr.Op = right;
                        notExpr.Location = currentLoc;
                        return notExpr;
                    }
            }

            return null;
             
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentPos">结束后指向该Token的下一个字符处</param>
        /// <param name="end"></param>
        /// <param name="matched">
        /// string: 已处理转义符
        /// var: 不含$符，即matched就是变量名
        /// int, float, operator: 不变
        /// </param>
        /// <returns></returns>
        private MatchTokenResult MatchToken(int currentPos, int end)
        {
            //string matched = null;
            MatchTokenResult result = new MatchTokenResult();
            result.BeginPos = currentPos;

            for (; isWhiteSpace(expr[result.BeginPos]) && result.BeginPos < end; result.BeginPos++) ;
            if (result.BeginPos >= end)
            {
                result.EndPos = result.BeginPos;
                result.TokenType = TokenType.Null;
                return result;
            }
            result.EndPos = result.BeginPos;

            char c = expr[result.EndPos];
            switch (c)
            {
                case '+':
                case '-':
                case '*':
                case '/':
                case '^':
                case '(':
                case ')':
                    result.Matched = c.ToString();
                    result.EndPos++;
                    result.TokenType = TokenType.Operator;
                    return result;

                case '=':
                    if (result.EndPos + 1 < end && expr[result.EndPos + 1] == '=')
                    {
                        result.Matched = "==";
                        result.EndPos += 2;
                    }
                    else
                    {
                        result.Matched = "=";
                        result.EndPos++;
                    }
                    result.TokenType = TokenType.Operator;
                    return result;

                case '!':
                    if (result.EndPos + 1 < end && expr[result.EndPos + 1] == '=')
                    {
                        result.Matched = "!=";
                        result.EndPos += 2;
                    }
                    else
                    {
                        result.Matched = "!";
                        result.EndPos++;
                    }
                    result.TokenType = TokenType.Operator;
                    return result;

                case '&':
                    if (result.EndPos + 1 < end && expr[result.EndPos + 1] == '&')
                    {
                        result.Matched = "&&";
                        result.EndPos += 2;
                        result.TokenType = TokenType.Operator;
                        return result;
                    }
                    else
                    {
                        kernel.IssueError(ErrorType.UnknownToken, loc.Offset(result.EndPos));
                        result.TokenType = TokenType.Error;
                        return result;
                    }

                case '|':
                    if (result.EndPos + 1 < end && expr[result.EndPos + 1] == '|')
                    {
                        result.Matched = "||";
                        result.EndPos += 2;
                        result.TokenType = TokenType.Operator;
                        return result;
                    }
                    else
                    {
                        kernel.IssueError(ErrorType.UnknownToken, loc.Offset(result.EndPos));
                        result.TokenType = TokenType.Error;
                        return result;
                    }

                case '>':
                    if (result.EndPos + 1 < end && expr[result.EndPos + 1] == '=')
                    {
                        result.Matched = ">=";
                        result.EndPos += 2;
                    }
                    else
                    {
                        result.Matched = ">";
                        result.EndPos ++;
                    }
                    result.TokenType = TokenType.Operator;
                    return result;

                case '<':
                    if (result.EndPos + 1 < end && expr[result.EndPos + 1] == '=')
                    {
                        result.EndPos += 2;
                        result.Matched = "<=";
                    }
                    else
                    {
                        result.EndPos++;
                        result.Matched = "<";
                    }
                    result.TokenType = TokenType.Operator;
                    return result;

                case '"':
                    result.EndPos++;
                    if (result.EndPos >= end)
                    {
                        kernel.IssueError(ErrorType.SingleQuote, loc.Offset(result.EndPos));
                        result.TokenType = TokenType.Error;
                        return result;
                    }

                    StringBuilder matchedBuilder = new StringBuilder();
                    bool escape = false;

                    while (result.EndPos < end)
                    {
                        if (expr[result.EndPos] == '\\')
                        {
                            if(escape)
                            {
                                matchedBuilder.Append('\\');
                                escape = false;
                            }
                            else
                            {
                                escape = true;
                            }
                        }
                        else if (expr[result.EndPos] == '"')
                        {
                            if (escape)
                            {
                                matchedBuilder.Append('"');
                                escape = false;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (escape)
                            {
                                switch (expr[result.EndPos])
                                {
                                    case 'n':
                                        matchedBuilder.Append('\n');
                                        break;

                                    case 't':
                                        matchedBuilder.Append('\t');
                                        break;

                                    case 'r':
                                        matchedBuilder.Append('\r');
                                        break;

                                    default:
                                        kernel.IssueError(ErrorType.UnknownEscapeChar, loc.Offset(result.EndPos), expr[result.EndPos].ToString());
                                        result.TokenType = TokenType.Error;
                                        return result;
                                }
                                escape = false;
                            }
                            else
                            {
                                matchedBuilder.Append(expr[result.EndPos]);
                            }
                        }

                        result.EndPos++;
                    }

                    if (result.EndPos < end)       //遇到引号后跳出while
                    {
                        result.EndPos++;           //跳过引号
                        result.Matched = matchedBuilder.ToString();
                        result.TokenType = TokenType.String;
                        return result;
                    }
                    else                        //遇到End而跳出while,即没有配对引号
                    {
                        kernel.IssueError(ErrorType.SingleQuote, loc.Offset(result.EndPos));
                        result.TokenType = TokenType.Error;
                        return result;
                    }

                default:
                    string substr = expr.Substring(result.EndPos, end - result.EndPos);
                    Match m;
                    if ((m = floatPattern.Match(substr)).Success)
                    {
                        result.Matched = m.Groups[0].Value;
                        result.EndPos += m.Length;
                        result.TokenType = TokenType.Float;
                        return result;
                    }
                    else if ((m = intPattern.Match(substr)).Success)
                    {
                        result.Matched = m.Groups[0].Value;
                        result.EndPos += m.Length;
                        result.TokenType = TokenType.Int;
                        return result;
                    }
                    else if ((m = varPattern.Match(substr)).Success)
                    {
                        result.Matched = m.Groups[1].Value;
                        result.EndPos += m.Length;
                        result.TokenType = TokenType.Variable;
                        return result;
                    }
                    else if ((m = boolPattern.Match(substr)).Success)
                    {
                        result.Matched = m.Groups[0].Value;
                        result.EndPos += m.Length;
                        result.TokenType = TokenType.Bool;
                        return result;
                    }
                    else
                    {
                        kernel.IssueError(ErrorType.UnknownToken, loc.Offset(result.EndPos));
                        result.TokenType = TokenType.Error;
                        return result;
                    }
            }

        }

        private static OpLevel getOpLevel(string op)
        {
            switch (op)
            {
                case "=":
                    return OpLevel.Assign;

                case "&&":
                case "||":
                    return OpLevel.AndOr;

                case "==":
                case "!=":
                    return OpLevel.Equal;

                case ">":
                case ">=":
                case "<":
                case "<=":
                    return OpLevel.Comparer;

                case "+":
                case "-":
                    return OpLevel.AddMinus;

                case "*":
                case "/":
                    return OpLevel.MulDiv;

                case "^":
                    return OpLevel.Power;

                case "!":
                    return OpLevel.Not;

                default:
                    return OpLevel.Null;
            }
        }

        private static string removeWhiteSpace(string str)
        {
            str = str.Replace(" ", string.Empty);
            str = str.Replace("\f", string.Empty);
            str = str.Replace("\n", string.Empty);
            str = str.Replace("\r", string.Empty);
            str = str.Replace("\t", string.Empty);
            str = str.Replace("\v", string.Empty);
            str = str.Replace(Environment.NewLine, string.Empty);
            return str;
        }

        private static bool isWhiteSpace(char c)
        {
            if (c == ' ' || c == '\f' || c == '\n' || c == '\r' || c == '\t' || c == '\v')
                return true;
            else
                return false;
        }

        /// <summary>
        /// 若出错则返回null，否则返回result
        /// </summary>
        /// <param name="exprType"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="loc"></param>
        /// <returns></returns>
        private BinaryLogicExpr processBinaryLogicExpr(BinaryLogicExpr result, Expression left, Expression right, Location loc)
        {
            result.DataType = DataType.Bool;
            if (result.DataType == DataType.Error)
                return null;
            result.FirstOp = left;
            result.SecondOp = right;
            result.Location = loc;
            return result;
        }


        private BinaryCmpExpr processBinaryCmpExpr(BinaryCmpExpr result, Expression left, Expression right, Location loc)
        {
            if (left.DataType == DataType.Bool || left.DataType == DataType.String
                || right.DataType == DataType.Bool || right.DataType == DataType.String)
            {
                kernel.IssueError(ErrorType.OprandTypeError, loc);
                return null;
            }
            else
            {
                result.DataType = DataType.Bool;
            }
 

            result.FirstOp = left;
            result.SecondOp = right;
            result.Location = loc;
            return result;
        }

        private BinaryAlgoExpr processBinaryAlgoExpr(BinaryAlgoExpr result, Expression left, Expression right, Location loc)
        {
            if ((left.DataType == DataType.String || right.DataType == DataType.String) && result is AddExpr)
            {
                result.DataType = DataType.String;
            }
            else if (left.DataType == DataType.Bool || left.DataType == DataType.String
                || right.DataType == DataType.Bool || right.DataType == DataType.String)
            {
                kernel.IssueError(ErrorType.OprandTypeError, loc);
                return null;
            }
            else if (left.DataType == DataType.Float || right.DataType == DataType.Float)
            {
                result.DataType = DataType.Float;
            }
            else if (left.DataType == DataType.Int && right.DataType == DataType.Int)
            {
                result.DataType = DataType.Int;
            }
            else
            {
                result.DataType = DataType.Unknown;
            }

            result.FirstOp = left;
            result.SecondOp = right;
            result.Location = loc;
            return result;
        }

        private struct MatchTokenResult
        {
            public TokenType TokenType { get; set; }
            /// <summary>
            /// 所返回的Token的开始位置
            /// </summary>
            public int BeginPos { get; set; } 
            /// <summary>
            /// Token最后一个字符的下一个位置
            /// </summary>
            public int EndPos { get; set; }
            public string Matched { get; set; }
        }
    }
}
