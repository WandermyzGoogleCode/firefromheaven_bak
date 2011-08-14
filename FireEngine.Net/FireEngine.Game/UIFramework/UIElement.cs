using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace FireEngine.GameEngine.UIFramework
{
    /// <summary>
    /// UI组件的基类。
    /// </summary>
    /// <remarks>响应用户事件，可受Layout管理</remarks>
    public abstract class UIElement : Visual
    {
        public Media.RectangleGeometry Bound
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int ZIndex
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// 只影响视觉效果；不影响Layout
        /// </remarks>
        public Matrix RenderTransform
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public Vector2 Position
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Width
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public int Height
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public LayoutProperties LayoutProperties
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public virtual bool IsSizeFixed
        {
            get
            {
                return false;
            }
        }

        public static DependencyProperty WidthProperty
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public static DependencyProperty HeightProperty
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public static DependencyProperty PositionProperty
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        public static DependencyProperty RenderTransformProperty
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <remarks>
        /// * ContentControl组成UITree, ScreenManager作为根节点
        ///  * 按ZIndex从大到小排序；从根节点做深度优先搜索
        ///  * this.HitTest()以this为根节点; ScreenManager.HitTest()做全局的HitTest
        /// </remarks>
        /// <param name="Point">待测点想对于根节点的坐标</param>
        /// <param name="Strategy">策略</param>
        public virtual UIElement HitTest(Vector2 Point, HitTestStrategy Strategy)
        {
            throw new System.NotImplementedException();
        }

        public override Visual Parent
        {
            get { throw new NotImplementedException(); }
        }

        public override VisualCollection Children
        {
            get { throw new NotImplementedException(); }
        }
    }
}
