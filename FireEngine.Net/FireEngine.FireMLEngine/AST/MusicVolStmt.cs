using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class MusicVolStmt : Statement
    {
        public MusicVolStmt()
        {
            Amplitude = 1;
            TransitionTime = TimeSpan.Zero;
        }

        public double Amplitude
        {
            get;
            set;
        }

        public TimeSpan TransitionTime
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
