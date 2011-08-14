using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine.AST;
using FireEngine.FireMLEngine.Expr;

namespace FireEngine.FireMLEngine.Runtime
{
    class ASTVisitor : IASTVisitor
    {
        private RuntimeKernel kernel;
        private ExprProcessor exprProcessor;
        private StrVarRefProcessor varRefProcessor;
        private FireMLRoot root;

        public ASTVisitor(RuntimeKernel kernel, FireMLRoot root)
        {
            this.kernel = kernel;
            this.root = root;
            exprProcessor = new ExprProcessor(kernel);
            varRefProcessor = new StrVarRefProcessor(kernel);
        }

        #region IASTVisitor Members

        public void Visit(FireMLRoot root, object[] args)
        {
            root.MainPlot.Accept(this);
        }

        public void Visit(ActionLayerDef actionLayerDef, object[] args)
        {
            
        }

        public void Visit(FunctionDef functionDef, object[] args)
        {
            
        }

        public void Visit(PlotDef plotDef, object[] args)
        {
            kernel.RuntimeData.ScopeStack.Open(new LocalScope());
            kernel.RuntimeData.InstructionStack.Push(InstructionStack.CLOSE_LOCAL_SCOPE_FLAG);
            kernel.RuntimeData.InstructionStack.Push(plotDef.Content);
            kernel.Next();
        }

        public void Visit(ContinueStmt continueStmt, object[] args)
        {
            //在编译时已经检查过break的合法性
            ASTNode node = null;
            
            do
            {
                int id = kernel.RuntimeData.InstructionStack.Peek();

                if (id == InstructionStack.CLOSE_LOCAL_SCOPE_FLAG)
                {
                    kernel.RuntimeData.ScopeStack.Close();
                }
                else
                {
                    node = root.NodeMap[id];
                }

                if (!(node is LoopStmt))
                {
                    kernel.RuntimeData.InstructionStack.Pop();
                }

            } while (!(node is LoopStmt));

            kernel.Next();
        }

        public void Visit(FunctionCallStmt funcCallStmt, object[] args)
        {
            FormalScope formalScope = new FormalScope();
            foreach (KeyValuePair<string, Expr.Expression> actual in funcCallStmt.ParamMap)
            {
                Expr.RightValue rightValue = exprProcessor.Eval(actual.Value);
                formalScope.SetValue(actual.Key, rightValue);
            }
            kernel.RuntimeData.ScopeStack.Open(formalScope);
            kernel.RuntimeData.ScopeStack.Open(new LocalScope());

            kernel.RuntimeData.InstructionStack.Push(InstructionStack.CALL_FLAG);
            kernel.RuntimeData.InstructionStack.Push(InstructionStack.CLOSE_LOCAL_SCOPE_FLAG);
            kernel.RuntimeData.InstructionStack.Push(InstructionStack.CLOSE_FORMAL_SCOPE_FLAG);

            kernel.RuntimeData.InstructionStack.Push(root.FuncDefMap[funcCallStmt.Name].FuncDefContent);

            CallStackElement elem = new CallStackElement();
            elem.Destination = root.FuncDefMap[funcCallStmt.Name];
            elem.Location = funcCallStmt.Location;
            elem.ReturnDest = exprProcessor.GetVarName(funcCallStmt.ReturnDest);
            kernel.RuntimeData.CallStack.Push(elem);
                        
            kernel.Next();
        }

        public void Visit(MusicStmt musicStmt, object[] args)
        {
            kernel.Behave(kernel.FuncCaller.Music(
                varRefProcessor.Replace(musicStmt.Source), 
                musicStmt.Asset, 
                musicStmt.Loop, 
                musicStmt.FadeIn));
        }

        public void Visit(MusicStopStmt musicStopStmt, object[] args)
        {
            kernel.Behave(kernel.FuncCaller.MusicStop(musicStopStmt.FadeOut));
        }

        public void Visit(MusicVolStmt musicVolStmt, object[] args)
        {
            kernel.Behave(kernel.FuncCaller.MusicVol(musicVolStmt.Amplitude, musicVolStmt.TransitionTime));
        }

        public void Visit(SwitchStmt switchStmt, object[] args)
        {
            RightValue value = exprProcessor.Eval(switchStmt.Expression);
            if (value is FloatConst)
            {
                kernel.IssueWarning(new Error(ErrorType.EqualOnFloat, switchStmt.Location));
            }
            foreach (SwitchCase switchCase in switchStmt.SwitchCaseList)
            {
                if (!(value is FloatConst) && switchCase.Value is FloatConst)
                {
                    kernel.IssueWarning(new Error(ErrorType.EqualOnFloat, switchCase.Location));
                }

                if (switchCase.Value == null || switchCase.Value.IsEqualValueTo(value))
                {
                    kernel.RuntimeData.ScopeStack.Open(new LocalScope());
                    kernel.RuntimeData.InstructionStack.Push(InstructionStack.CLOSE_LOCAL_SCOPE_FLAG);
                    kernel.RuntimeData.InstructionStack.Push(switchCase.CaseContent);
                    break;
                }
            }

            kernel.Next();
        }

        public void Visit(SelectStmt selectStmt, object[] args)
        {
            kernel.Behave(kernel.FuncCaller.Select((selectStmt.Return.LeftValue as Expr.VarRef).VarName, selectStmt.OptionList.ToArray()));
        }

        public void Visit(ActorStmt actorStmt, object[] args)
        {
            kernel.Behave(kernel.FuncCaller.Actor(
                varRefProcessor.Replace(actorStmt.Name),
                varRefProcessor.Replace(actorStmt.Img),
                actorStmt.Asset,
                varRefProcessor.Replace(actorStmt.Avatar),
                actorStmt.AvaAsset,
                actorStmt.Layer,
                actorStmt.Position
                ));
        }

        public void Visit(DialogStmt dialogStmt, object[] args)
        {
            kernel.Behave(kernel.FuncCaller.Dialog(
                varRefProcessor.Replace(dialogStmt.Text)
                ));
        }

        public void Visit(IfStmt ifStmt, object[] args)
        {
            foreach (IfBlock block in ifStmt.BlockList)
            {
                if (block.Condition != null)
                {
                    RightValue cond = exprProcessor.Eval(block.Condition);
                    if (cond.ToBoolean())
                    {
                        kernel.RuntimeData.ScopeStack.Open(new LocalScope());
                        kernel.RuntimeData.InstructionStack.Push(InstructionStack.CLOSE_LOCAL_SCOPE_FLAG);
                        kernel.RuntimeData.InstructionStack.Push(block.Content);
                        break;
                    }
                }
                else
                {
                    kernel.RuntimeData.ScopeStack.Open(new LocalScope());
                    kernel.RuntimeData.InstructionStack.Push(InstructionStack.CLOSE_LOCAL_SCOPE_FLAG);
                    kernel.RuntimeData.InstructionStack.Push(block.Content);
                    break;
                }
            }

            kernel.Next();
        }

        public void Visit(LoopStmt loopStmt, object[] args)
        {
            RightValue rightValue = exprProcessor.Eval(loopStmt.Condition);
            if (rightValue.ToBoolean())
            {
                kernel.RuntimeData.InstructionStack.Push(loopStmt);
                kernel.RuntimeData.ScopeStack.Open(new LocalScope());
                kernel.RuntimeData.InstructionStack.Push(InstructionStack.CLOSE_LOCAL_SCOPE_FLAG);
                kernel.RuntimeData.InstructionStack.Push(loopStmt.Content);
            }
            else
            {
                //Nothing to do
            }

            kernel.Next();
        }

        public void Visit(BackgroundStmt backgroundStmt, object[] args)
        {
            kernel.Behave(kernel.FuncCaller.Background(
                varRefProcessor.Replace(backgroundStmt.Img),
                backgroundStmt.Asset));
        }

        public void Visit(EchoStmt echoStmt, object[] args)
        {
            string result = exprProcessor.EvalStr(echoStmt.Expression);
            kernel.FuncCaller.Echo(result);
        }

        public void Visit(IncludeStmt includeStmt, object[] args)
        {
            kernel.RuntimeData.ScopeStack.Open(new LocalScope());
            kernel.RuntimeData.InstructionStack.Push(InstructionStack.CALL_FLAG);
            //kernel.RuntimeData.InstructionStack.Push(InstructionStack.CLOSE_FORMAL_SCOPE_FLAG);
            kernel.RuntimeData.InstructionStack.Push(InstructionStack.CLOSE_LOCAL_SCOPE_FLAG);

            kernel.RuntimeData.InstructionStack.Push(root.SubPlotMap[includeStmt.SubPlot].Content);

            CallStackElement elem = new CallStackElement();
            elem.Destination = root.SubPlotMap[includeStmt.SubPlot];
            elem.Location = includeStmt.Location;
            elem.ReturnDest = null;
            kernel.RuntimeData.CallStack.Push(elem);

            kernel.Next();
        }

        public void Visit(BreakStmt breakStmt, object[] args)
        {
            //在编译时已经检查过break的合法性
            ASTNode node = null;
            do
            {
                int id = kernel.RuntimeData.InstructionStack.Pop();

                if (id == InstructionStack.CLOSE_LOCAL_SCOPE_FLAG)
                {
                    kernel.RuntimeData.ScopeStack.Close();
                    continue;
                }

                node = root.NodeMap[id];
            } while (!(node is LoopStmt));

            kernel.Next();
        }

        public void Visit(ExpressionStmt expressionStmt, object[] args)
        {
            foreach (Expr.Expression expr in expressionStmt.ExpressionList)
            {
                exprProcessor.Eval(expr);
            }
            kernel.Next();
        }

        public void Visit(ReturnStmt returnStmt, object[] args)
        {
            RightValue returnValue = exprProcessor.Eval(returnStmt.Expression);
            int id;
            do
            {
                id = kernel.RuntimeData.InstructionStack.Pop();
                if (id == InstructionStack.CLOSE_LOCAL_SCOPE_FLAG)
                {
                    kernel.RuntimeData.ScopeStack.Close();
                }
                else if (id == InstructionStack.CLOSE_FORMAL_SCOPE_FLAG)
                {
                    kernel.RuntimeData.ScopeStack.Close();
                }
                else if (id == InstructionStack.CALL_FLAG)
                {
                    CallStackElement elem = kernel.RuntimeData.CallStack.Pop();
                    if (elem.ReturnDest != null)
                    {
                        kernel.RuntimeData.ScopeStack.SetValue(elem.ReturnDest, returnValue);
                    }
                }
            } while (id != InstructionStack.CALL_FLAG);

            kernel.Next();
        }

        public void Visit(ParameterDef parameterDef, object[] args)
        {
            
        }

        public void Visit(SelectOption selectOption, object[] args)
        {
            
        }

        public void Visit(SwitchCase switchCase, object[] args)
        {
            
        }

        public void Visit(IfBlock ifBlock, object[] args)
        {
            
        }


        public void Visit(DataStmt dataStmt, object[] args)
        {
            throw new NotImplementedException();
        }

        public void Visit(AssetDef assetDef, object[] args)
        {

        }

        #endregion
    }
}
