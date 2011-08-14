using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine.AST;

namespace FireEngine.FireMLEngine.Runtime
{
    [Serializable]
    class CallStack
    {
        private Stack<CallStackElement> callStack = new Stack<CallStackElement>();
        internal Stack<CallStackElement> Stack
        {
            get { return callStack; }
        }

        internal CallStack()
        {
        }

        internal void Push(CallStackElement element)
        {
            callStack.Push(element);
        }

        internal CallStackElement Pop()
        {
            return callStack.Pop();
        }

        internal CallStackElement Peek()
        {
            return callStack.Peek();
        }

        internal int Count
        {
            get { return callStack.Count; }
        }

        internal string StackTrace
        {
            get
            {
                throw new NotImplementedException();
                //TODO: 实现！
            }
        }
    }

    internal class CallStackElement
    {
        /// <summary>
        /// 被调用的函数或SubPlot
        /// </summary>
        internal Definition Destination
        {
            get;
            set;
        }

        /// <summary>
        /// 发生调用的位置
        /// </summary>
        internal Location Location
        {
            get;
            set;
        }

        internal string ReturnDest
        {
            get;
            set;
        }
    }
}
