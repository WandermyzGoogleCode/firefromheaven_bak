using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine.AST;
using FireEngine.FireMLEngine.Expr;
using FireEngine.FireMLData;

namespace FireEngine.FireMLEngine.Compiler
{
    class ASTChecker : ASTRecursor, IDataCheckHelper
    {
        private CompilerKernel kernel;
        private Stack<LoopStmt> loopStack = new Stack<LoopStmt>();
        private FireMLRoot root;
        private ExpressionParser exprParser;
        private Location checkLocation; //当前正在被检查的位置，用于序列化

        public ASTChecker(CompilerKernel kernel)
        {
            this.kernel = kernel;
            exprParser = new ExpressionParser(kernel);
        }

        public void Check(FireMLRoot root)
        {
            this.root = root;
            root.Accept(this);
        }

        public override void Visit(FireMLRoot root, object[] args)
        {
            if (root.MainPlot == null)
            {
                kernel.IssueError(ErrorType.MainPlotNotExist, new Location(""));
            }
            base.Visit(root, args);
        }

        public override void Visit(ContinueStmt continueStmt, object[] args)
        {
            if (loopStack.Count == 0)
            {
                kernel.IssueError(ErrorType.ContinueNotInLoop, continueStmt.Location);
            }
            base.Visit(continueStmt, args);
        }

        public override void Visit(BreakStmt breakStmt, object[] args)
        {
            if (loopStack.Count == 0)
            {
                kernel.IssueError(ErrorType.BreakNotInLoop, breakStmt.Location);
            }
            base.Visit(breakStmt, args);
        }

        public override void Visit(LoopStmt loopStmt, object[] args)
        {
            loopStack.Push(loopStmt);
            base.Visit(loopStmt, args);
            loopStack.Pop();
        }

        public override void Visit(IncludeStmt includeStmt, object[] args)
        {
            checkLocation = includeStmt.Location;
            CheckSubPlot(includeStmt.SubPlot);
            base.Visit(includeStmt, args);
        }

        public override void Visit(FunctionCallStmt funcCallStmt, object[] args)
        {
            if (!root.FuncDefMap.ContainsKey(funcCallStmt.Name))
            {
                kernel.IssueError(ErrorType.FunctionNotExist, funcCallStmt.Location, funcCallStmt.Name);
                return;
            }

            FunctionDef def = root.FuncDefMap[funcCallStmt.Name];

            //解析ParaStr
            if (funcCallStmt.ParaStr != null)
            {
                int count = funcCallStmt.ParaStr.Length;
                if (def.ParaStrMap.ContainsKey(count))
                {
                    for (int i = 0; i < funcCallStmt.ParaStr.Length; i++)
                    {
                        string varName = def.ParaStrMap[count][i];
                        string content = funcCallStmt.ParaStr[i];
                        if (funcCallStmt.ParamMap.ContainsKey(varName))
                        {
                            kernel.IssueError(ErrorType.DuplicatedParaAndParaStr, funcCallStmt.Location, varName);
                        }
                        else
                        {
                            funcCallStmt.ParamMap.Add(varName, exprParser.CreateStringConst(content, funcCallStmt.Location));
                        }
                    }
                }
                else
                {
                    kernel.IssueError(ErrorType.NoMatchedParaStrDef, funcCallStmt.Location);
                }
            }

            //解析实参
            List<KeyValuePair<string, Expression>> toModify = new List<KeyValuePair<string, Expression>>();
            foreach (KeyValuePair<string, Expression> actual in funcCallStmt.ParamMap)
            {
                ParameterDef paramDef = def.ParaMap[actual.Key];
                string actualStr = ((actual.Value as RightValueExpr).RightValue as StringConst).Value;
                Location loc = actual.Value.Location;

                RightValue parsedValue = null;
                RightValueExpr rightValueExpr = new RightValueExpr();

                switch (paramDef.ParameterType)
                {
                    case ParameterDef.ParameterTypeEnum.Auto:
                        parsedValue = exprParser.ParseValue(actualStr, loc);
                        break;

                    case ParameterDef.ParameterTypeEnum.String:
                        continue;

                    case ParameterDef.ParameterTypeEnum.Expression:
                        toModify.Add(new KeyValuePair<string,Expression>(actual.Key, exprParser.ParseExpr(actualStr, loc)));
                        continue;

                    case ParameterDef.ParameterTypeEnum.Int:
                        parsedValue = exprParser.ParseValue(actualStr, DataType.Int, loc);
                        break;

                    case ParameterDef.ParameterTypeEnum.Float:
                        parsedValue = exprParser.ParseValue(actualStr, DataType.Float, loc);
                        break;

                    case ParameterDef.ParameterTypeEnum.Bool:
                        parsedValue = exprParser.ParseValue(actualStr, DataType.Bool, loc);
                        break;
                }

                rightValueExpr.RightValue = parsedValue;
                rightValueExpr.Location = loc;
                toModify.Add(new KeyValuePair<string,Expression>(actual.Key, rightValueExpr));
            }

            foreach(KeyValuePair<string,Expression> elem in toModify)
            {
                funcCallStmt.ParamMap[elem.Key] = elem.Value;
            }

            //添加默认值
            foreach (KeyValuePair<string, ParameterDef> pDef in def.ParaMap)
            {
                if (!funcCallStmt.ParamMap.ContainsKey(pDef.Key) && pDef.Value.Default != null)
                {
                    RightValueExpr expr = new RightValueExpr();
                    expr.RightValue = pDef.Value.Default;
                    funcCallStmt.ParamMap.Add(pDef.Key, expr);
                }
            }

            //参数完整性检查
            foreach (KeyValuePair<string, ParameterDef> pDef in def.ParaMap)
            {
                if (!funcCallStmt.ParamMap.ContainsKey(pDef.Key))
                {
                    kernel.IssueError(ErrorType.ParameterNotDefined, funcCallStmt.Location, pDef.Key);
                }
            }

            base.Visit(funcCallStmt, args);

            //TODO: 刷新Expression的DataType
        }

        public override void Visit(ActorStmt actorStmt, object[] args)
        {
            if (actorStmt.Layer != null && !root.ActionLayerMap.ContainsKey(actorStmt.Layer))
            {
                kernel.IssueError(ErrorType.ActionLayerNotExist, actorStmt.Location, actorStmt.Layer);
            }
            base.Visit(actorStmt, args);
        }

        public override void Visit(DataStmt dataStmt, object[] args)
        {
            checkLocation = dataStmt.Location;
            dataStmt.Data.CheckReference(this);
            base.Visit(dataStmt, args);
        }

        public override void Visit(AssetDef assetDef, object[] args)
        {
            checkLocation = assetDef.Location;
            assetDef.AssetData.CheckReference(this);
            base.Visit(assetDef, args);
        }

        #region IDataCheckHelper Members

        public bool CheckContent(string path, FireEngine.ContentInterface.ContentType expectedType)
        {
            throw new NotSupportedException();
        }

        public bool CheckSubPlot(string name)
        {
            if (!root.SubPlotMap.ContainsKey(name))
            {
                kernel.IssueError(ErrorType.SubPlotNotExist, checkLocation, name);
                return false;
            }

            return true;
        }

        public bool CheckAsset(string name, Type expectedType)
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
