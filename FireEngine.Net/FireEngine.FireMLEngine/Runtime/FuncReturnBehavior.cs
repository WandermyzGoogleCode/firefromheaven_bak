using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Runtime
{
    public enum FuncReturnBehavior
    {
        /// <summary>
        /// 立即执行下一条指令
        /// </summary>
        Next = 0,

        /// <summary>
        /// Block结束后执行下一条指令
        /// </summary>
        WaitForBlock,

        /// <summary>
        /// 用户点击鼠标后执行下一条指令（即等待调用Next()方法）
        /// </summary>
        WaitForUser
    }
}
