using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Expr
{
    [Serializable]
    class StringConst : RightValue
    {
        public string Value
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
            return Value != null && Value.Length > 0;
        }

        public override bool IsEqualValueTo(object obj)
        {
            return (obj is StringConst) && (obj as StringConst).Value == Value;
        }
    }
}
