using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Expr
{
    [Serializable]
    class AssignExpr : RightValueExpr
    {
        public LeftValueExpr LeftExpr
        {
            get;
            set;
        }

        public Expression RightExpr
        {
            get;
            set;
        }

        internal override void Accept(IExprVisitor visitor, params object[] args)
        {
            visitor.Visit(this, args);
        }
    }
}
