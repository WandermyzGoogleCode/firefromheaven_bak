using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    abstract class ASTRecursor : IASTVisitor
    {
        public virtual void Visit(FireMLRoot root, object[] args)
        {
            if (root.MainPlot != null)
            {
                root.MainPlot.Accept(this);
            }

            foreach (KeyValuePair<string, PlotDef> subPlot in root.SubPlotMap)
            {
                subPlot.Value.Accept(this);
            }

            foreach (KeyValuePair<string, FunctionDef> funcDef in root.FuncDefMap)
            {
                funcDef.Value.Accept(this);
            }

            foreach (KeyValuePair<string, ActionLayerDef> actionLayerDef in root.ActionLayerMap)
            {
                actionLayerDef.Value.Accept(this);
            }
        }

        public virtual void Visit(ActionLayerDef actionLayerDef, object[] args)
        {
        }

        public virtual void Visit(FunctionDef functionDef, object[] args)
        {
            foreach (Statement s in functionDef.FuncDefContent)
            {
                s.Accept(this);
            }
        }

        public virtual void Visit(PlotDef plotDef, object[] args)
        {
            foreach (Statement s in plotDef.Content)
            {
                s.Accept(this);
            }
        }

        public virtual void Visit(ContinueStmt continueStmt, object[] args)
        {
        }

        public virtual void Visit(FunctionCallStmt funcCallStmt, object[] args)
        {
        }

        public virtual void Visit(MusicStmt musicStmt, object[] args)
        {
        }

        public virtual void Visit(MusicStopStmt musicStopStmt, object[] args)
        {
        }

        public virtual void Visit(MusicVolStmt musicVolStmt, object[] args)
        {
        }

        public virtual void Visit(SwitchStmt switchStmt, object[] args)
        {
            foreach (SwitchCase switchCase in switchStmt.SwitchCaseList)
            {
                switchCase.Accept(this);
            }
        }

        public virtual void Visit(SelectStmt selectStmt, object[] args)
        {
        }

        public virtual void Visit(ActorStmt actorStmt, object[] args)
        {
        }

        public virtual void Visit(DialogStmt dialogStmt, object[] args)
        {
        }

        public virtual void Visit(IfStmt ifStmt, object[] args)
        {
            foreach (IfBlock block in ifStmt.BlockList)
            {
                block.Accept(this);
            }
        }

        public virtual void Visit(LoopStmt loopStmt, object[] args)
        {
            foreach (Statement s in loopStmt.Content)
            {
                s.Accept(this);
            }

        }

        public virtual void Visit(BackgroundStmt backgroundStmt, object[] args)
        {
        }

        public virtual void Visit(EchoStmt echoStmt, object[] args)
        {
        }

        public virtual void Visit(IncludeStmt includeStmt, object[] args)
        {
        }

        public virtual void Visit(BreakStmt breakStmt, object[] args)
        {
        }

        public virtual void Visit(ExpressionStmt expressionStmt, object[] args)
        {
        }

        public virtual void Visit(ReturnStmt returnStmt, object[] args)
        {
        }

        public virtual void Visit(ParameterDef parameterDef, object[] args)
        {
        }

        public virtual void Visit(SelectOption selectOption, object[] args)
        {
        }

        public virtual void Visit(SwitchCase switchCase, object[] args)
        {
            foreach (Statement s in switchCase.CaseContent)
            {
                s.Accept(this);
            }
        }

        public virtual void Visit(IfBlock ifBlock, object[] args)
        {
            foreach (Statement s in ifBlock.Content)
            {
                s.Accept(this);
            }
        }

        public virtual void Visit(DataStmt dataStmt, object[] args)
        {
            
        }

        public virtual void Visit(AssetDef assetDef, object[] args)
        {
        }
    }
}
