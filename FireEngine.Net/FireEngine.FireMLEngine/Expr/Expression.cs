using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Expr
{
    [Serializable]
    abstract class Expression
    {
        /// <summary>
        /// 在编译时确定的数据类型；运行时不再改变
        /// </summary>
        public DataType DataType
        {
            get;
            set;
        }

        public Location Location
        {
            get;
            set;
        }

        internal abstract void Accept(IExprVisitor visitor, params object[] args);
    }
}
