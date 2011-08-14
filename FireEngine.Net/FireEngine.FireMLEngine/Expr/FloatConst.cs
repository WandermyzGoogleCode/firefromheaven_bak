using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Expr
{
    [Serializable]
    class FloatConst : RightValue
    {
        public double Value
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
            return (obj is FloatConst) && (obj as FloatConst).Value == Value;
        }
    }
}
