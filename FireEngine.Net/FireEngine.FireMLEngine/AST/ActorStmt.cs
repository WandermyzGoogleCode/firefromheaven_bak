using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLData;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class ActorStmt : Statement
    {
        public string Name
        {
            get;
            set;
        }

        public string Img
        {
            get;
            set;
        }

        public string Asset
        {
            get;
            set;
        }

        public string Avatar
        {
            get;
            set;
        }

        public string AvaAsset
        {
            get;
            set;
        }

        public string Layer
        {
            get;
            set;
        }

        public PositionData Position
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
