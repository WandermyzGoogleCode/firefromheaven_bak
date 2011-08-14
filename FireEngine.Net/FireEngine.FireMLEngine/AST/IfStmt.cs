using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class IfStmt : Statement
    {
        public List<IfBlock> BlockList
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
                return BlockList.ToArray();
            }
        }
    }
}
