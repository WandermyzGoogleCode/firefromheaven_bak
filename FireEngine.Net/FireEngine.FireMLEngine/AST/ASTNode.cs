using System;
using System.Collections.Generic;
using System.Text;


namespace FireEngine.FireMLEngine.AST
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Visitor模式
    /// </remarks>
    [Serializable]
    public abstract class ASTNode
    {
        internal ASTNode() { }

        internal ASTNode Parent { get; set; }
        internal virtual ASTNode[] Children
        {
            get { return null; }
        }

        internal Location Location { get; set; }

        /// <summary>
        /// 用来标识结点的ID，在AST中唯一
        /// </summary>
        internal int ID { get; set; }

        internal abstract void Accept(IASTVisitor visitor, params object[] args);
    }
}
