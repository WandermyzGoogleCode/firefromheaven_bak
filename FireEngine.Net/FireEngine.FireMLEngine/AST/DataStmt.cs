using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLData;

namespace FireEngine.FireMLEngine.AST
{
    /// <summary>
    /// 通过局部序列化方法来解析的Statement
    /// </summary>
    [Serializable]
    class DataStmt : Statement
    {
        public FireMLDataBase Data { get; set; }
        public string DataType { get; set; }

        public DataStmt() { }
        public DataStmt(Type type)
        {
            DataType = type.Name;
        }

        internal override void Accept(IASTVisitor visitor, params object[] args)
        {
            visitor.Visit(this, args);
        }
    }
}
