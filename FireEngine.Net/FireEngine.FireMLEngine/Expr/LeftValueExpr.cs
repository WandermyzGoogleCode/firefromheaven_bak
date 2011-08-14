using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Expr
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 变量引用算作LeftValueExpr
    /// </remarks>
    [Serializable]
    class LeftValueExpr : Expression
    {
        public LeftValue LeftValue
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
