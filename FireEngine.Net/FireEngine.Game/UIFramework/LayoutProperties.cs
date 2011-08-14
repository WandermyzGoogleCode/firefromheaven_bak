using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.GameEngine.UIFramework
{
    /// <summary>
    /// 用来保存某个UIElement在某个Layout下的属性
    /// </summary>
    /// <remarks>
    /// 可以直接设置这些属性，达到进行Layout管理的目的。
    /// 通过事件将修改情况通知Panel，以进行位置大小计算、刷新等操作
    /// </remarks>
    public abstract class LayoutProperties
    {
        /// <summary>
        /// 任意一个Layout Property被改变时触发
        /// </summary>
        public event EventHandler LayoutPropertyChanged;
    }
}
