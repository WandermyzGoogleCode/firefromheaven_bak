using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine.Expr;
using System.Text.RegularExpressions;

namespace FireEngine.FireMLEngine.Compiler
{
    class ExpressionParser
    {
        private static readonly Regex intValuePattern = new Regex(@"^([0-9]+)$", RegexOptions.Compiled);
        private static readonly Regex floatValuePattern = new Regex(@"^([0-9]*\.[0-9]+)$", RegexOptions.Compiled);
        private static readonly Regex boolValuePattern = new Regex(@"^((true)|(false))$", RegexOptions.Compiled);

        CompilerKernel kernel;

        internal ExpressionParser(CompilerKernel kernel)
        {
            this.kernel = kernel;
        }

        internal Expression ParseExpr(string expr, Location loc)
        {
            ExpressionParserKernel exprKernel = new ExpressionParserKernel(kernel, expr, loc);
            return exprKernel.Parse();
        }

        internal RightValue ParseValue(string value, Location loc)
        {
            //TODO: 类型歧义问题
            if (intValuePattern.IsMatch(value))
            {
                IntConst result = new IntConst();
                result.Value = int.Parse(value);
                return result;
            }
            else if (floatValuePattern.IsMatch(value))
            {
                FloatConst result = new FloatConst();
                result.Value = double.Parse(value);
                return result;
            }
            else if (boolValuePattern.IsMatch(value))
            {
                BoolConst result = new BoolConst();
                result.Value = bool.Parse(value);
                return result;
            }
            else
            {
                StringConst result = new StringConst();
                result.Value = value;
                return result;
            }
        }

        internal RightValue ParseValue(string value, DataType type, Location loc)
        {
            switch (type)
            {
                case DataType.Int:
                    int intParsed;
                    if (!int.TryParse(value, out intParsed))
                    {
                        kernel.IssueError(ErrorType.ParseTypeError, loc, value, type.ToString());
                        return null;
                    }
                    IntConst intResult = new IntConst();
                    intResult.Value = intParsed;
                    return intResult;

                case DataType.Float:
                    float floatParsed;
                    if (!float.TryParse(value, out floatParsed))
                    {
                        kernel.IssueError(ErrorType.ParseTypeError, loc, value, type.ToString());
                        return null;
                    }
                    FloatConst floatResult = new FloatConst();
                    floatResult.Value = floatParsed;
                    return floatResult;

                case DataType.Bool:
                    bool boolParsed;
                    int boolIntParsed;
                    double boolFloatParsed;
                    BoolConst boolResult = new BoolConst();
                    if (bool.TryParse(value, out boolParsed))
                    {
                        boolResult.Value = boolParsed;
                        return boolResult;
                    }
                    else if (int.TryParse(value, out boolIntParsed))
                    {
                        boolResult.Value = (boolIntParsed != 0);
                        return boolResult;
                    }
                    else if (double.TryParse(value, out boolFloatParsed))
                    {
                        boolResult.Value = (boolFloatParsed != 0);
                        return boolResult;
                    }
                    else
                    {
                        kernel.IssueError(ErrorType.ParseTypeError, loc, value, type.ToString());
                        return null;
                    }
                case DataType.String:
                    StringConst strConst = new StringConst();
                    strConst.Value = value;
                    return strConst;
            }
            return null;
        }

        internal RightValue ParseValue(object value, DataType type, Location loc)
        {
            //TODO: 完整实现
            switch (type)
            {
                case DataType.Int:
                    IntConst intResult = new IntConst();
                    intResult.Value = (int)value;
                    return intResult;

                case DataType.Float:
                    FloatConst floatResult = new FloatConst();
                    floatResult.Value = (double)value;
                    return floatResult;

                case DataType.Bool:
                    BoolConst boolResult = new BoolConst();
                    boolResult.Value = (bool)value;
                    return boolResult;

                case DataType.String:
                    StringConst strResult = new StringConst();
                    strResult.Value = (string)value;
                    return strResult;
            }

            return null;
        }

        internal RightValueExpr CreateStringConst(string value, Location loc)
        {
            StringConst strConst = new StringConst();
            strConst.Value = value;
            RightValueExpr expr = new RightValueExpr();
            expr.RightValue = strConst;
            expr.Location = loc;
            return expr;
        }

        //internal VarRef ParseVarRef(string varRef, Location loc)
        //{
        //    return null;
        //}

        internal LeftValueExpr ParseLeftValueExpr(string expr, Location loc)
        {
            ExpressionParserKernel exprKernel = new ExpressionParserKernel(kernel, expr, loc);
            Expression result =  exprKernel.Parse();
            if (!(result is LeftValueExpr))
            {
                kernel.IssueError(ErrorType.NotLeftValue, loc);
                return null;
            }
            else
            {
                return result as LeftValueExpr;
            }
        }
    }
}
