using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine.Expr;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class ParameterDef : Definition
    {
        /// <summary>
        /// 默认值在编译时已经存储好
        /// </summary>
        [NonSerialized]
        private RightValue defaultValue;
        public RightValue Default
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        public ParameterTypeEnum ParameterType
        {
            get;
            set;
        }

        internal override void Accept(IASTVisitor visitor, params object[] args)
        {
            visitor.Visit(this, args);
        }

        public enum ParameterTypeEnum
        {
            Auto = 0,
            Int,
            Float,
            Bool,
            String,
            Expression
        }
    }
}
