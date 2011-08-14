using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.GameEngine.UIFramework
{
    /// <summary>
    /// 可以显示在屏幕上的元素，作为VisualTree的节点
    /// </summary>
    /// <remarks>考虑实现XNA的Drawable接口</remarks>
    public abstract class Visual : DependencyObject
    {
        public abstract VisualCollection Children
        {
            get;
        }

        public abstract Visual Parent
        {
            get;
        }

        //TODO: Draw
    }
}
