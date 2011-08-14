using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Expr
{
    [Serializable]
    class BoolConst : RightValue
    {
        public bool Value
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
            return Value;
        }

        public override bool IsEqualValueTo(object obj)
        {
            return (obj is BoolConst) && (obj as BoolConst).Value == Value;
        }
    }
}
