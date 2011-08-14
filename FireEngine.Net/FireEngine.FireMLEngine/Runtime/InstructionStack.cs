using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine.AST;

namespace FireEngine.FireMLEngine.Runtime
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 将待执行的指令逆序压栈
    /// 存储时，要存储ASTNode的ID，以支持序列化
    /// </remarks>
    [Serializable]
    class InstructionStack
    {
        private Stack<int> instStack = new Stack<int>();

        public const int ROOT_FLAG = 0;
        /// <summary>
        /// 表示执行到此时应该关闭最上层的Local Scope
        /// </summary>
        public const int CLOSE_LOCAL_SCOPE_FLAG = -1;
        /// <summary>
        /// 表示执行到此时应该关闭最上层的Formal Scope
        /// </summary>
        public const int CLOSE_FORMAL_SCOPE_FLAG = -2;

        /// <summary>
        /// 表示此处发生一个函数或者子情节的调用
        /// </summary>
        public const int CALL_FLAG = -3;

        internal InstructionStack()
        {
        }

        internal void Push(Statement statement)
        {
            instStack.Push(statement.ID);
        }

        internal void Push(int id)
        {
            instStack.Push(id);
        }

        internal void Push(List<Statement> statementSequence)
        {
            for (int i = statementSequence.Count - 1; i >= 0; i--)
            {
                instStack.Push(statementSequence[i].ID);
            }
        }

        internal void Push(List<int> idSequence)
        {
            for (int i = idSequence.Count - 1; i >= 0; i--)
            {
                instStack.Push(idSequence[i]);
            }
        }

        internal int Pop()
        {
            return instStack.Pop();
        }

        internal int Peek()
        {
            return instStack.Peek();
        }

        internal int Count
        {
            get { return instStack.Count; }
        }
    }
}
