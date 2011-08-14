using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLData;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class ActionLayerDef : Definition
    {
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
