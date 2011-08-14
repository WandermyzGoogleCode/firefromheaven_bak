using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using FireEngine.FireMLEngine.Runtime;

namespace FireEngine.FireMLEngine
{
    /// <summary>
    /// 处理包含在字符串中的变量
    /// </summary>
    class StrVarRefProcessor
    {
        private static readonly Regex varRefRegex = new Regex(@"\{\$(\w+[\w\d]*)\}", RegexOptions.Compiled);
        //TODO: 正则有错误
        private RuntimeKernel runtimeKernel;

        internal static bool IsVariableIncluded(string str)
        {
            return varRefRegex.Match(str).Success;
        }

        internal StrVarRefProcessor(RuntimeKernel runtimeKernel)
        {
            this.runtimeKernel = runtimeKernel;
        }

        internal string Replace(string str)
        {
            if (str == null)
                return null;

            return varRefRegex.Replace(str, matchEvaluator);
        }

        private string matchEvaluator(Match match)
        {
            string varName = match.Groups[1].Value;
            string result;
            if (runtimeKernel.RuntimeData.ScopeStack.IsExist(varName))
            {
                result = runtimeKernel.RuntimeData.ScopeStack.GetValue(varName).ToString();
            }
            else
            {
                result = match.Value;
                runtimeKernel.IssueWarning(new Error(ErrorType.VariableNotInitialized, new Location(""), varName));
            }
            return result;
        }
    }
}
