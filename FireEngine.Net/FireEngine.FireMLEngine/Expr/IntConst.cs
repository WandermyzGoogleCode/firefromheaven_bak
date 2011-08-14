using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Expr
{
    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{Value} : {this.GetType().Name}")]
    class IntConst : RightValue
    {
        public int Value
        {
            get;
            set;
        }

        public override object GetValueObject()
        {
            return Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public override bool ToBoolean()
        {
            return Value != 0;
        }

        public override bool IsEqualValueTo(object obj)
        {
            return (obj is IntConst) && (obj as IntConst).Value == Value;
        }
    }
}
