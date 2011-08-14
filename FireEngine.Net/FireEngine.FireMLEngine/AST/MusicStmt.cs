using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class MusicStmt : Statement
    {
        public MusicStmt()
        {
            FadeIn = TimeSpan.Zero;
        }

        public string Source
        {
            get;
            set;
        }

        public string Asset
        {
            get;
            set;
        }

        public bool Loop
        {
            get;
            set;
        }

        public TimeSpan FadeIn
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
