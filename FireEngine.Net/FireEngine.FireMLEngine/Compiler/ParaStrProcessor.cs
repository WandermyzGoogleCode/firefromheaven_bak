using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FireEngine.FireMLEngine.Compiler
{
    static class ParaStrProcessor
    {
        private static readonly Regex varPattern = new Regex(@"^\$(\w+[\w\d]*)$", RegexOptions.Compiled);
        //TODO: 正则表达式有错

        /// <summary>
        /// 读取参数列表的定义字串，返回变量表，不包括$。返回null表示格式有误
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] ReadDefVarList(string str)
        {
            string[] varStrArray = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<string> varList = new List<string>();
            foreach (string varStr in varStrArray)
            {
                Match m = varPattern.Match(varStr.Trim());
                if (!m.Success)
                    return null;

                varList.Add(m.Groups[1].Value);
            }

            return varList.ToArray();
        }

        /// <summary>
        /// 读取参数列表的调用字串，返回每个参数的内容。返回null表示格式有误
        /// </summary>
        /// <param name="paraStr"></param>
        /// <returns></returns>
        public static string[] ReadParaCallList(string paraStr)
        {
            List<string> result = new List<string>();
            bool inQuote = false;
            bool escape = false;
            bool isString = false;

            StringBuilder builder = new StringBuilder();

            foreach (char c in (paraStr + ","))
            {
                if (isString && c != ',' && !isWhiteSpace(c))
                    return null;

                switch (c)
                {
                    case '"':
                        if (!escape)
                        {
                            if (inQuote)
                            {
                                inQuote = false;
                                isString = true;
                            }
                            else
                            {
                                inQuote = true;
                            }
                        }
                        break;

                    case '\\':
                        if (inQuote)
                        {
                            escape = !escape;
                        }
                        break;

                    case ',':
                        if (!inQuote)
                        {
                            if (isString)
                            {
                                result.Add(builder.ToString());
                            }
                            else
                            {
                                if (builder.Length != 0)
                                {
                                    result.Add(builder.ToString());
                                }
                            }
                            builder.Remove(0, builder.Length);
                            isString = false;
                        }
                        break;

                    default:
                        break;
                }

                if(!(!inQuote && c==',')
                    && !(!escape && c == '"')
                    && !(escape && c == '\\')   //前面已经求过反了
                    && (inQuote || !isWhiteSpace(c))
                    )
                {
                    builder.Append(c);
                }

                if (c != '\\')
                {
                    escape = false;
                }
                
                /*
                if (!escape
                    && !(!inQuote && c == ',')
                    && !(!escape && c == '"')
                    && (inQuote || !isWhiteSpace(c))
                    )
                    builder.Append(c);
                 */
            }

            return (result.Count == 1 && result[0].Length == 0) ? null : result.ToArray();
        
        }

        private static bool isWhiteSpace(char c)
        {
            if (c == ' ' || c == '\f' || c == '\n' || c == '\r' || c == '\t' || c == '\v')
                return true;
            else
                return false;
        }
    }
}
