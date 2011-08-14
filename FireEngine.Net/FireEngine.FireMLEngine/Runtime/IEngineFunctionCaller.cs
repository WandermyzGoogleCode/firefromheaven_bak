using System;
using System.Collections.Generic;
using System.Text;

namespace FireEngine.FireMLEngine.Runtime
{
    public interface IEngineFunctionCaller
    {
        FuncReturnBehavior ActionLayerDef(string name, FireEngine.FireMLData.PositionData position);
        FuncReturnBehavior Actor(string name, string img, string asset, string avatar, string avaasset, string layer, FireMLData.PositionData position);
        FuncReturnBehavior Background(string img, string asset);    //Asset待定，考虑不传Asset
        FuncReturnBehavior Dialog(string text);
        FuncReturnBehavior Echo(string result);
        FuncReturnBehavior Music(string src, string asset, bool loop, TimeSpan fadeIn);
        FuncReturnBehavior MusicStop(TimeSpan fadeOut);
        FuncReturnBehavior MusicVol(double amplitude, TimeSpan transitionTime);
        FuncReturnBehavior Select(string varName, FireEngine.FireMLEngine.AST.SelectOption[] options);

        void End();

        void IssueWarning(string message);

        /// <summary>
        /// 用户选择了选项后，请触发该事件
        /// </summary>
        event SelectEventHandler UserSelected;
    }

    public class SelectEventArgs : EventArgs
    {
        private string varName;
        public string VarName { get { return varName; } }

        private Expr.RightValue result;
        public Expr.RightValue Result { get { return result; } }

        public SelectEventArgs(string varName, Expr.RightValue result)
        {
            this.varName = varName;
            this.result = result;
        }
    }
    public delegate void SelectEventHandler(object source, SelectEventArgs e);
}
