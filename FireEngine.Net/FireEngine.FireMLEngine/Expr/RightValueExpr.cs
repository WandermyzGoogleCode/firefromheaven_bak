using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Expr
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// 常数表达式算作RightValueExpr
    /// </remarks>
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("Type = {this.GetType().Name}; Value = {RightValue.ToString()}")]
    class RightValueExpr : Expression
    {
        /// <summary>
        /// 该属性在运行时回填
        /// </summary>
        public RightValue RightValue
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
