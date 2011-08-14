using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine.AST;

namespace FireEngine.FireMLEngine
{
    [Serializable]
    public class FireMLRoot
    {
        internal PlotDef MainPlot { get; set; }
        internal Dictionary<string, PlotDef> SubPlotMap { get; set; }
        internal Dictionary<string, FunctionDef> FuncDefMap { get; set; }
        internal Dictionary<string, ActionLayerDef> ActionLayerMap { get; set; }
        internal Dictionary<int, ASTNode> NodeMap { get; set; }
        internal Dictionary<string, AssetDef> AssetMap { get; set; }

        internal FireMLRoot()
        {
            SubPlotMap = new Dictionary<string, PlotDef>();
            FuncDefMap = new Dictionary<string, FunctionDef>();
            ActionLayerMap = new Dictionary<string, ActionLayerDef>();
            NodeMap = new Dictionary<int, ASTNode>();
            AssetMap = new Dictionary<string, AssetDef>();
        }

        internal void Accept(IASTVisitor visitor, params object[] args)
        {
            visitor.Visit(this, args);
        }
    }
}
