using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FireEngine.FireMLEngine.Runtime
{
    [Serializable]
    internal class RuntimeData
    {
        private InstructionStack instructionStack = new InstructionStack();
        internal InstructionStack InstructionStack
        {
            get { return instructionStack; }
        }

        private ScopeStack scopeStack = new ScopeStack();
        internal ScopeStack ScopeStack
        {
            get { return scopeStack; }
        }

        private CallStack callStack = new CallStack();
        public CallStack CallStack
        {
            get { return callStack; }
        }

        internal RuntimeData()
        {
        }

        internal void Save(Stream destStream)
        {
            throw new NotSupportedException();
        }

        static internal RuntimeData Load(Stream srcStream)
        {
            throw new NotSupportedException();
        }
    }
}
