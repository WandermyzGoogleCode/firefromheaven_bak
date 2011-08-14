using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class SelectStmt : Statement
    {
        public List<SelectOption> OptionList
        {
            get;
            set;
        }

        public Expr.LeftValueExpr Return
        {
            get;
            set;
        }

        internal override void Accept(IASTVisitor visitor, params object[] args)
        {
            visitor.Visit(this, args);
        }

    }
}
