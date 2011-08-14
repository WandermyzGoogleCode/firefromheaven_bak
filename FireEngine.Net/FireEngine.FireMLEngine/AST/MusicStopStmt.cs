using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class MusicStopStmt : Statement
    {
        public MusicStopStmt()
        {
            FadeOut = TimeSpan.Zero;
        }

        public TimeSpan FadeOut
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
