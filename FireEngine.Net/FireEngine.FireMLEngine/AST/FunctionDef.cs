using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    [Serializable]
    class FunctionDef : Definition
    {

        public Dictionary<string, ParameterDef> ParaMap
        {
            get;
            set;
        }

        public List<Statement> FuncDefContent
        {
            get;
            set;
        }

        /// <remark>
        /// 在编译时已将所有函数调用的ParaStr转换为实际参数
        /// </remark>
        [NonSerialized]
        private Dictionary<int, string[]> paraStrMap;
        public Dictionary<int, string[]> ParaStrMap
        {
            get { return paraStrMap; }
            set { paraStrMap = value; }
        }

        internal override void Accept(IASTVisitor visitor, params object[] args)
        {
            visitor.Visit(this, args);
        }

        internal override ASTNode[] Children
        {
            get
            {
                return FuncDefContent.ToArray();
            }
        }
    }
}
