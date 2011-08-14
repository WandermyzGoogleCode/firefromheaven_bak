using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Expr
{
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("VarName = {VarName}")]
    class VarRef : LeftValue
    {
        public string VarName
        {
            get;
            set;
        }

        /*
        public override bool IsNull
        {
            get
            {
                return (VarName == null);
            }
        }*/
    }
}
