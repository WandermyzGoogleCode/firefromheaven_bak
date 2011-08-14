using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class LoopStmt : Statement
    {
        public Expr.Expression Condition
        {
            get;
            set;
        }

        public List<Statement> Content
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
                return Content.ToArray();
            }
        }
    }
}
