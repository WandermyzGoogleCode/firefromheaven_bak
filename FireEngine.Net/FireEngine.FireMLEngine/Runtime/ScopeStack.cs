using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine.Expr;

namespace FireEngine.FireMLEngine.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 1、被调用者可以访问调用者的变量
    /// 2、如果函数的形参与已有变量重名，则覆盖
    /// 3、if, switch和loop作为单独的块
    /// </remarks>
    [Serializable]
    class ScopeStack
    {
        LinkedList<Scope> scopeStack = new LinkedList<Scope>();
        //GlobalScope globalScope;

        internal ScopeStack()
        {

        }

        internal void Open(Scope scope)
        {
            scopeStack.AddFirst(scope);
        }

        internal void Close()
        {
            scopeStack.RemoveFirst();
        }

        /// <summary>
        /// 从最近的作用域中取得变量；若变量不存在则返回null
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        internal RightValue GetValue(string varName)
        {
            Scope scope = searchVar(varName);
            if (scope != null)
                return scope.GetValue(varName);
            else
                return null;
        }

        internal RightValue GetValue(VarRef varRef)
        {
            return GetValue(varRef.VarName);
        }

        /// <summary>
        /// 从最近的作用域寻找变量并赋值；若变量不存在则在栈顶作用域声明之
        /// </summary>
        /// <param name="varName"></param>
        /// <param name="value"></param>
        internal void SetValue(string varName, RightValue value)
        {
            Scope scope = searchVar(varName);
            if (scope != null)
            {
                scope.SetValue(varName, value);
            }
            else
            {
                scope = scopeStack.First.Value;
                scope.SetValue(varName, value);
            }
        }

        internal bool IsExist(string varName)
        {
            return searchVar(varName) != null;
        }

        private Scope searchVar(string varName)
        {
            foreach (Scope scope in scopeStack)
            {
                if (scope.IsExist(varName))
                {
                    return scope;
                }
            }

            return null;
        }
    }
}
