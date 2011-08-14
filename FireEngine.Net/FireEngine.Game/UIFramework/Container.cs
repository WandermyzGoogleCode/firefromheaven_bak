using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.GameEngine.UIFramework
{
    /// <summary>
    /// 可以作为容器容纳其它控件的控件
    /// </summary>
    abstract class ContentControl : Control
    {
        public override VisualCollection Children
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// 刷新Layout
        /// </summary>
        /// <remarks>
        /// 要递归地调用子容器的RefreshLayout()
        /// </remarks>
        public virtual void RefreshLayout()
        {
            throw new System.NotImplementedException();
        }
    }
}
