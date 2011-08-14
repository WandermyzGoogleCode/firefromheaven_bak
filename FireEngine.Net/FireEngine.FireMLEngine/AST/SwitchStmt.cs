using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class SwitchStmt : Statement
    {
        public List<SwitchCase> SwitchCaseList
        {
            get;
            set;
        }

        public Expr.Expression Expression
        {
            get;
            set;
        }

        internal override void Accept(IASTVisitor visitor, params object[] args)
        {
            visitor.Visit(this, args);
        }

        internal override ASTNode[] Children
        {
            get
            {
                return SwitchCaseList.ToArray();
            }
        }
    }
}
