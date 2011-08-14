using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class SwitchCase : ASTNode
    {
        /// <summary>
        /// null表示Default
        /// </summary>
        public Expr.RightValue Value
        {
            get;
            set;
        }

        public List<Statement> CaseContent
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
                return CaseContent.ToArray();
            }
        }
    }
}
