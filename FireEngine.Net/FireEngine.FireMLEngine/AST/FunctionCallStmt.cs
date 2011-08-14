using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class FunctionCallStmt : Statement
    {
        public string Name
        {
            get;
            set;
        }

        public Dictionary<string, Expr.Expression> ParamMap
        {
            get;
            set;
        }

        public Expr.LeftValueExpr ReturnDest
        {
            get;
            set;
        }

        [NonSerialized]
        private string[] paraStr;
        public string[] ParaStr
        {
            get { return paraStr; }
            set { paraStr = value; }
        }

        internal override void Accept(IASTVisitor visitor, params object[] args)
        {
            visitor.Visit(this, args);
        }

    }
}
