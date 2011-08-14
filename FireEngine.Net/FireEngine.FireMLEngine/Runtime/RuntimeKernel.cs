using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using FireEngine.Library;

namespace FireEngine.FireMLEngine.Runtime
{
    public class RuntimeKernel
    {
        private TreeMap<object,object> blockSet = new TreeMap<object,object>();
        private ASTVisitor visitor;
        private bool shouldNext = false;

        private RuntimeData runtimeData = new RuntimeData();
        internal RuntimeData RuntimeData
        {
            get { return runtimeData; }
        }

        private FireMLRoot root;
        internal FireMLRoot FireMLRoot
        {
            get { return root; }
        }

        private IEngineFunctionCaller funcCaller;
        public IEngineFunctionCaller FuncCaller
        {
            get { return funcCaller; }
        }

        public RuntimeKernel(IEngineFunctionCaller funcCaller, FireMLRoot fireMLRoot)
        {
            this.funcCaller = funcCaller;
            this.root = fireMLRoot;
            this.visitor = new ASTVisitor(this, root);
            this.runtimeData.InstructionStack.Push(InstructionStack.ROOT_FLAG);

            funcCaller.UserSelected += new SelectEventHandler(funcCaller_UserSelected);
        }

        void funcCaller_UserSelected(object source, SelectEventArgs e)
        {
            runtimeData.ScopeStack.SetValue(e.VarName, e.Result);
        }
    
        /// <summary>
        /// 将清除blockSet
        /// </summary>
        /// <param name="srcStream"></param>
        public void Load(Stream srcStream)
        {
            runtimeData = RuntimeData.Load(srcStream);
        }

        public void Save(Stream destStream)
        {
            runtimeData.Save(destStream);
        }

        public void Next()
        {
            if (runtimeData.InstructionStack.Count > 0)
            {
                int id = runtimeData.InstructionStack.Pop();
                if (id == InstructionStack.ROOT_FLAG)
                {
                    root.Accept(visitor);
                }
                else if (id == InstructionStack.CLOSE_LOCAL_SCOPE_FLAG)
                {
                    runtimeData.ScopeStack.Close();
                    Next();
                }
                else if (id == InstructionStack.CLOSE_FORMAL_SCOPE_FLAG)
                {
                    runtimeData.ScopeStack.Close();
                    Next();
                }
                else if (id == InstructionStack.CALL_FLAG)
                {
                    CallStackElement elem = runtimeData.CallStack.Pop();
                    Next();
                }
                else
                {
                    root.NodeMap[id].Accept(visitor);
                }
            }
            else
            {
                funcCaller.End();
            }
        }

        public void Behave(FuncReturnBehavior behavior)
        {
            switch (behavior)
            {
                case FuncReturnBehavior.Next:
                    Next();
                    break;

                case FuncReturnBehavior.WaitForBlock:
                    shouldNext = true;
                    break;

                case FuncReturnBehavior.WaitForUser:
                    break;
            }
        }

        /// <summary>
        /// 如果有Block则什么也不做；否则Next()
        /// </summary>
        public void Update()
        {
            if (blockSet.Count > 0)
                return;
            else
            {
                if (shouldNext)
                {
                    Next();
                    shouldNext = false;
                }
            }
        }

        public void AddBlock(object source)
        {
            blockSet.Add(source, null);   
        }

        public void RemoveBlock(object source)
        {
            blockSet.Remove(source);
        }

        ///// <param name="name">变量名，指的是当前作用域下的变量</param>
        ///// <remarks>根据传入的value的数据类型来决定变量的新类型</remarks>
        //public void SetValue(string name, object value)
        //{
        //    throw new System.NotImplementedException();
        //}

        internal void IssueWarning(Error error)
        {
            funcCaller.IssueWarning(error.Message);
        }
    }
}
