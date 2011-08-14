using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine.Expr;

namespace FireEngine.FireMLEngine.Runtime
{
    [Serializable]
    abstract class Scope
    {
        Dictionary<string, RightValue> variables = new Dictionary<string, RightValue>();

        /// <summary>
        /// 若不存在指定名称的变量，则返回null
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        internal RightValue GetValue(string varName)
        {
            if (variables.ContainsKey(varName))
                return variables[varName];
            else
                return null;
        }

        internal RightValue GetValue(VarRef varRef)
        {
            return GetValue(varRef.VarName);
        }

        internal void SetValue(string varName, RightValue value)
        {
            if (variables.ContainsKey(varName))
            {
                variables[varName] = value;
            }
            else
            {
                variables.Add(varName, value);
            }
        }

        internal bool IsExist(string varName)
        {
            return variables.ContainsKey(varName);
        }
    }
}
