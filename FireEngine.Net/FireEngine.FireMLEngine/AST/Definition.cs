using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.AST
{
    /// <summary>
    /// 直接属于FireMLRoot的结点，均为全局定义
    /// </summary>
    [Serializable]
    abstract class Definition : ASTNode
    {
        public string Name
        {
            get;
            set;
        }
    }
}
