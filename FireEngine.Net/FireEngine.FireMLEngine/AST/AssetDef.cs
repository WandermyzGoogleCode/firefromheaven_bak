using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLData.Asset;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class AssetDef : Definition
    {
        public AssetDataBase AssetData
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
