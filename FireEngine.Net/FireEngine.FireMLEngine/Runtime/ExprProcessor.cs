using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine.Expr;

namespace FireEngine.FireMLEngine.Runtime
{
    class ExprProcessor : IExprVisitor
    {
        private ScopeStack scopeStack;
        private RuntimeKernel kernel;

        internal ExprProcessor(RuntimeKernel kernel)
        {
            this.kernel = kernel;
            this.scopeStack = kernel.RuntimeData.ScopeStack;
        }

        #region IExprVisitor Members



        public void Visit(AddExpr addExpr, object[] args)
        {
            addExpr.FirstOp.Accept(this);
            addExpr.SecondOp.Accept(this);

            RightValue v1 = readRightValue(addExpr.FirstOp);
            RightValue v2 = readRightValue(addExpr.SecondOp);

            if (v1 is StringConst || v2 is StringConst)
            {
                StringConst result = new StringConst();
                result.Value = v1.ToString() + v2.ToString();
                addExpr.RightValue = result;
                //addExpr.DataType = DataType.String;
            }
            else
            {
                
                DataType resultType = readAlgoOperand(v1, v2, addExpr.Location);
                if (resultType == DataType.Int)
                {
                    IntConst result = new IntConst();
                    result.Value = Convert.ToInt32(v1.GetValueObject()) + Convert.ToInt32(v2.GetValueObject());
                    addExpr.RightValue = result;
                    //addExpr.DataType = DataType.Int;
                }
                else if (resultType == DataType.Float)
                {
                    FloatConst result = new FloatConst();
                    result.Value = Convert.ToDouble(v1.GetValueObject()) + Convert.ToDouble(v2.GetValueObject());
                    addExpr.RightValue = result;
                    //addExpr.DataType = DataType.Float;
                }
                else
                    throw new Exception();
            }
        }

        public void Visit(AndExpr andExpr, object[] args)
        {
            andExpr.FirstOp.Accept(this);
            andExpr.SecondOp.Accept(this);
            RightValue v1 = readRightValue(andExpr.FirstOp);
            RightValue v2 = readRightValue(andExpr.SecondOp);

            BoolConst result = new BoolConst();
            result.Value = v1.ToBoolean() && v2.ToBoolean();
            andExpr.RightValue = result;
        }

        public void Visit(AssignExpr assignExpr, object[] args)
        {
            assignExpr.LeftExpr.Accept(this);
            assignExpr.RightExpr.Accept(this);

            RightValue v = readRightValue(assignExpr.RightExpr);
            assignExpr.RightValue = v;

            if (assignExpr.LeftExpr.LeftValue is VarRef)
            {
                VarRef varRef = assignExpr.LeftExpr.LeftValue as VarRef;
                scopeStack.SetValue(varRef.VarName, v);
            }
            else
            {
                throw new Exception();
            }
        }

        public void Visit(DivExpr divExpr, object[] args)
        {
            divExpr.FirstOp.Accept(this);
            divExpr.SecondOp.Accept(this);

            RightValue v1 = readRightValue(divExpr.FirstOp);
            RightValue v2 = readRightValue(divExpr.SecondOp);

            
            DataType resultType = readAlgoOperand(v1, v2, divExpr.Location);
            if (resultType == DataType.Int)
            {
                IntConst result = new IntConst();
                if (Convert.ToInt32(v2.GetValueObject()) == 0)
                {
                    throw new RuntimeException(new Error(ErrorType.DivisorZero, divExpr.Location));
                }
                result.Value = Convert.ToInt32(v1.GetValueObject()) / Convert.ToInt32(v2.GetValueObject());
                divExpr.RightValue = result;
                //divExpr.DataType = DataType.Int;
            }
            else if (resultType == DataType.Float)
            {
                FloatConst result = new FloatConst();
                if (Convert.ToDouble(v2.GetValueObject()) == 0)
                {
                    throw new RuntimeException(new Error(ErrorType.DivisorZero, divExpr.Location));
                }
                result.Value = Convert.ToDouble(v1.GetValueObject()) / Convert.ToDouble(v2.GetValueObject());
                divExpr.RightValue = result;
                //divExpr.DataType = DataType.Float;
            }
            else
            {
                throw new Exception();
            }
        }

        public void Visit(EquExpr equExpr, object[] args)
        {
            equExpr.FirstOp.Accept(this);
            equExpr.SecondOp.Accept(this);

            RightValue v1 = readRightValue(equExpr.FirstOp);
            RightValue v2 = readRightValue(equExpr.SecondOp);

            
            DataType resultType = readAlgoOperand(v1, v2, equExpr.Location);
            BoolConst result = new BoolConst();
            if (resultType == DataType.Int)
            {
                result.Value = Convert.ToInt32(v1.GetValueObject()) == Convert.ToInt32(v2.GetValueObject());
                
            }
            else if (resultType == DataType.Float)
            {
                kernel.IssueWarning(new Error(ErrorType.EqualOnFloat, equExpr.Location));   
                result.Value = Convert.ToDouble(v1.GetValueObject()) == Convert.ToDouble(v2.GetValueObject());
            }
            else
            {
                throw new Exception();
            }
            equExpr.RightValue = result;
        }

        public void Visit(GreatEquExpr greatEquExpr, object[] args)
        {
            greatEquExpr.FirstOp.Accept(this);
            greatEquExpr.SecondOp.Accept(this);

            RightValue v1 = readRightValue(greatEquExpr.FirstOp);
            RightValue v2 = readRightValue(greatEquExpr.SecondOp);


            DataType resultType = readAlgoOperand(v1, v2, greatEquExpr.Location);
            BoolConst result = new BoolConst();
            if (resultType == DataType.Int)
            {
                result.Value = Convert.ToInt32(v1.GetValueObject()) >= Convert.ToInt32(v2.GetValueObject());

            }
            else if (resultType == DataType.Float)
            {
                result.Value = Convert.ToDouble(v1.GetValueObject()) >= Convert.ToDouble(v2.GetValueObject());
            }
            else
            {
                throw new Exception();
            }
            greatEquExpr.RightValue = result;
        }

        public void Visit(GreatExpr greatExpr, object[] args)
        {
            greatExpr.FirstOp.Accept(this);
            greatExpr.SecondOp.Accept(this);

            RightValue v1 = readRightValue(greatExpr.FirstOp);
            RightValue v2 = readRightValue(greatExpr.SecondOp);


            DataType resultType = readAlgoOperand(v1, v2, greatExpr.Location);
            BoolConst result = new BoolConst();
            if (resultType == DataType.Int)
            {
                result.Value = Convert.ToInt32(v1.GetValueObject()) > Convert.ToInt32(v2.GetValueObject());

            }
            else if (resultType == DataType.Float)
            {
                result.Value = Convert.ToDouble(v1.GetValueObject()) > Convert.ToDouble(v2.GetValueObject());
            }
            else
            {
                throw new Exception();
            }
            greatExpr.RightValue = result;
        }

        public void Visit(LeftValueExpr leftValueExpr, object[] args)
        {
           
        }

        public void Visit(LessEquExpr lessEquExpr, object[] args)
        {
            lessEquExpr.FirstOp.Accept(this);
            lessEquExpr.SecondOp.Accept(this);

            RightValue v1 = readRightValue(lessEquExpr.FirstOp);
            RightValue v2 = readRightValue(lessEquExpr.SecondOp);


            DataType resultType = readAlgoOperand(v1, v2, lessEquExpr.Location);
            BoolConst result = new BoolConst();
            if (resultType == DataType.Int)
            {
                result.Value = Convert.ToInt32(v1.GetValueObject()) <= Convert.ToInt32(v2.GetValueObject());

            }
            else if (resultType == DataType.Float)
            {
                result.Value = Convert.ToDouble(v1.GetValueObject()) <= Convert.ToDouble(v2.GetValueObject());
            }
            else
            {
                throw new Exception();
            }
            lessEquExpr.RightValue = result;
        }

        public void Visit(LessExpr lessExpr, object[] args)
        {
            lessExpr.FirstOp.Accept(this);
            lessExpr.SecondOp.Accept(this);

            RightValue v1 = readRightValue(lessExpr.FirstOp);
            RightValue v2 = readRightValue(lessExpr.SecondOp);


            DataType resultType = readAlgoOperand(v1, v2, lessExpr.Location);
            BoolConst result = new BoolConst();
            if (resultType == DataType.Int)
            {
                result.Value = Convert.ToInt32(v1.GetValueObject()) < Convert.ToInt32(v2.GetValueObject());

            }
            else if (resultType == DataType.Float)
            {
                result.Value = Convert.ToDouble(v1.GetValueObject()) < Convert.ToDouble(v2.GetValueObject());
            }
            else
            {
                throw new Exception();
            }
            lessExpr.RightValue = result;
        }

        public void Visit(MulExpr mulExpr, object[] args)
        {
            mulExpr.FirstOp.Accept(this);
            mulExpr.SecondOp.Accept(this);

            RightValue v1 = readRightValue(mulExpr.FirstOp);
            RightValue v2 = readRightValue(mulExpr.SecondOp);

            
            DataType resultType = readAlgoOperand(v1, v2, mulExpr.Location);
            if (resultType == DataType.Int)
            {
                IntConst result = new IntConst();
                result.Value = Convert.ToInt32(v1.GetValueObject()) * Convert.ToInt32(v2.GetValueObject());
                mulExpr.RightValue = result;
                //mulExpr.DataType = DataType.Int;
            }
            else if (resultType == DataType.Float)
            {
                FloatConst result = new FloatConst();
                result.Value = Convert.ToDouble(v1.GetValueObject()) * Convert.ToDouble(v2.GetValueObject());
                mulExpr.RightValue = result;
                //mulExpr.DataType = DataType.Float;
            }
            else
            {
                throw new Exception();
            }
        }

        public void Visit(NegativeExpr negativeExpr, object[] args)
        {
            negativeExpr.Op.Accept(this);
            RightValue v = readRightValue(negativeExpr);
            if (v is IntConst)
            {
                IntConst result = new IntConst();
                result.Value = -(v as IntConst).Value;
                negativeExpr.RightValue = result;
            }
            else if (v is FloatConst)
            {
                FloatConst result = new FloatConst();
                result.Value = -(v as FloatConst).Value;
                negativeExpr.RightValue = result;
            }
            else
            {
                throw new RuntimeException(new Error(ErrorType.OprandTypeError, negativeExpr.Location));
            }
        }

        public void Visit(NeqExpr neqExpr, object[] args)
        {
            neqExpr.FirstOp.Accept(this);
            neqExpr.SecondOp.Accept(this);

            RightValue v1 = readRightValue(neqExpr.FirstOp);
            RightValue v2 = readRightValue(neqExpr.SecondOp);

            
            DataType resultType = readAlgoOperand(v1, v2, neqExpr.Location);
            BoolConst result = new BoolConst();
            if (resultType == DataType.Int)
            {
                result.Value = Convert.ToInt32(v1.GetValueObject()) != Convert.ToInt32(v2.GetValueObject());

            }
            else if (resultType == DataType.Float)
            {
                kernel.IssueWarning(new Error(ErrorType.EqualOnFloat, neqExpr.Location));   
                result.Value = Convert.ToDouble(v1.GetValueObject()) != Convert.ToDouble(v2.GetValueObject());
            }
            else
            {
                throw new Exception();
            }
            neqExpr.RightValue = result;
        }

        public void Visit(NotExpr notExpr, object[] args)
        {
            notExpr.Op.Accept(this);
            RightValue v1 = readRightValue(notExpr.Op);

            BoolConst result = new BoolConst();
            result.Value = !v1.ToBoolean();
            notExpr.RightValue = result;
        }

        public void Visit(OrExpr orExpr, object[] args)
        {
            orExpr.FirstOp.Accept(this);
            orExpr.SecondOp.Accept(this);
            RightValue v1 = readRightValue(orExpr.FirstOp);
            RightValue v2 = readRightValue(orExpr.SecondOp);

            BoolConst result = new BoolConst();
            result.Value = v1.ToBoolean() || v2.ToBoolean();
            orExpr.RightValue = result;
        }

        public void Visit(PowExpr powExpr, object[] args)
        {
            powExpr.FirstOp.Accept(this);
            powExpr.SecondOp.Accept(this);

            RightValue v1 = readRightValue(powExpr.FirstOp);
            RightValue v2 = readRightValue(powExpr.SecondOp);

            
            DataType resultType = readAlgoOperand(v1, v2, powExpr.Location);
            if (resultType == DataType.Int)
            {
                IntConst result = new IntConst();
                result.Value = (int)Math.Pow(Convert.ToInt32(v1.GetValueObject()), Convert.ToInt32(v2.GetValueObject()));
                powExpr.RightValue = result;
                //powExpr.DataType = DataType.Int;
            }
            else if (resultType == DataType.Float)
            {
                FloatConst result = new FloatConst();
                result.Value = Math.Pow(Convert.ToDouble(v1.GetValueObject()), Convert.ToDouble(v2.GetValueObject()));
                powExpr.RightValue = result;
                //powExpr.DataType = DataType.Float;
            }
            else
            {
                throw new Exception();
            }
        }

        public void Visit(RightValueExpr rightValueExpr, object[] args)
        {
            
        }

        public void Visit(SubExpr subExpr, object[] args)
        {
            subExpr.FirstOp.Accept(this);
            subExpr.SecondOp.Accept(this);

            RightValue v1 = readRightValue(subExpr.FirstOp);
            RightValue v2 = readRightValue(subExpr.SecondOp);

            
            DataType resultType = readAlgoOperand(v1, v2, subExpr.Location);
            if (resultType == DataType.Int)
            {
                IntConst result = new IntConst();
                result.Value = Convert.ToInt32(v1.GetValueObject()) - Convert.ToInt32(v2.GetValueObject());
                subExpr.RightValue = result;
                //subExpr.DataType = DataType.Int;
            }
            else if (resultType == DataType.Float)
            {
                FloatConst result = new FloatConst();
                result.Value = Convert.ToDouble(v1.GetValueObject()) - Convert.ToDouble(v2.GetValueObject());
                subExpr.RightValue = result;
                //subExpr.DataType = DataType.Float;
            }
            else
            {
                throw new Exception();
            }
        }

        private DataType readAlgoOperand(RightValue v1, RightValue v2, Location loc)
        {
            if (v1 is BoolConst || v2 is BoolConst
                || v1 is StringConst || v2 is StringConst)
            {
                throw new RuntimeException(new Error(ErrorType.OprandTypeError, loc));
            }
            else if (v1 is FloatConst || v2 is FloatConst)
            {
                return DataType.Float;
            }
            else if (v1 is IntConst && v2 is IntConst)
            {
                return DataType.Int;
            }
            else
            {
                throw new RuntimeException(new Error(ErrorType.OprandTypeError, loc));
            }
        }

        private RightValue readRightValue(Expression expression)
        {
            if (expression is LeftValueExpr)
            {
                return readRightValue((expression as LeftValueExpr).LeftValue, expression.Location);
            }
            else if (expression is RightValueExpr)
            {
                return (expression as RightValueExpr).RightValue;
            }

            //只有上面两种情况
            throw new Exception();
        }

        private RightValue readRightValue(LeftValue leftValue, Location loc)
        {
            if (leftValue is VarRef)
            {
                VarRef var = leftValue as VarRef;
                RightValue result = scopeStack.GetValue(var.VarName);
                if (result == null)
                {
                    throw new RuntimeException(new Error(ErrorType.VariableNotInitialized, loc, var.VarName));
                }
                else
                {
                    return result;
                }
            }

            //只有上面一种情况
            throw new Exception();
        }

        public string EvalStr(Expression expr)
        {
            return Eval(expr).ToString();
        }

        public RightValue Eval(Expression expr)
        {
            expr.Accept(this);
            RightValue result = readRightValue(expr);
            return result;
        }

        public string GetVarName(LeftValueExpr expr)
        {
            if (expr == null || expr.LeftValue == null)
            {
                return null;
            }

            LeftValue leftValue = expr.LeftValue;
            if (leftValue is VarRef)
            {
                return (leftValue as VarRef).VarName;
            }
            else
            {
                throw new Exception();
            }
        }

        #endregion
    }
}
