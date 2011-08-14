using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.GameEngine.UIFramework
{
    public enum HitTestStrategy
    {
        /// <summary>
        /// 找到第一个非叶节点：Button.HitTest()鼠标是否在Button的范围内
        /// </summary>
        FirstNonLeaf,
        /// <summary>
        /// 找到第一个叶节点：获得最上层的元素
        /// </summary>
        FirstLeaf,
    }
}
