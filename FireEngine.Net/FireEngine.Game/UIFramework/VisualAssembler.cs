using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.GameEngine.UIFramework
{
    /// <summary>
    /// 装配显示所有Visual对象，相当于VisualTree的根节点
    /// </summary>
    /// <remarks>
    /// 包含ScreenManager的所有功能；显示光标、Debug信息等
    /// </remarks>
    public class VisualAssembler : Visual
    {

        public override VisualCollection Children
        {
            get { throw new NotImplementedException(); }
        }

        public override Visual Parent
        {
            get { throw new NotImplementedException(); }
        }
    }
}
