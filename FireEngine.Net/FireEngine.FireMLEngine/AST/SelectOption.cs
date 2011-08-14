using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    public class SelectOption : ASTNode
    {
        internal SelectOption() : base() { }

        public Expr.RightValue Value
        {
            get;
            internal set;
        }

        public string Text
        {
            get;
            internal set;
        }

        internal override void Accept(IASTVisitor visitor, params object[] args)
        {
            visitor.Visit(this, args);
        }
    }
}
