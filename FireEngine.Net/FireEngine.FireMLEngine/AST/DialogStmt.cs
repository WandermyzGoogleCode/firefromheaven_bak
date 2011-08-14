using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    /// <summary>
    /// 一条对白：即完全显示后，等待用户点击鼠标
    /// </summary>
    [Serializable]
    class DialogStmt : Statement
    {
        public string Text
        {
            get;
            set;
        }

        internal override void Accept(IASTVisitor visitor, params object[] args)
        {
            visitor.Visit(this, args);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
