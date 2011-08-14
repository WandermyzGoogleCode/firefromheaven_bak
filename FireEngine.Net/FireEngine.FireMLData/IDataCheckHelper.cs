using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.ContentInterface;

namespace FireEngine.FireMLData
{
    /// <summary>
    /// 由FireMLEngine给出实现，在要求Data做内容检查时传入。如果检查未通过，则FireMLEngine调用其IssueError报告错误
    /// </summary>
    public interface IDataCheckHelper
    {
        bool CheckContent(string path, ContentType expectedType);
        bool CheckSubPlot(string name);
        bool CheckAsset(string name, Type expectedType);
    }
}
