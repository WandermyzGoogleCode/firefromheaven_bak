using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Expr
{
    [Serializable]
    public abstract class RightValue : Value
    {
        public abstract object GetValueObject();
        public abstract override string ToString();
        public abstract bool ToBoolean();

        public abstract bool IsEqualValueTo(object obj);
    }
}
