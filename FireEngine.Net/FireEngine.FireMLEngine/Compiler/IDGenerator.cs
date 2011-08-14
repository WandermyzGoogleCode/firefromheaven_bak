using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine.AST;

namespace FireEngine.FireMLEngine.Compiler
{
    class IDGenerator
    {
        private FireMLRoot root;
        int counter;

        internal IDGenerator()
        {
        }

        internal void Generate(FireMLRoot root)
        {
            this.root = root;
            this.counter = 1;
            root.NodeMap = new Dictionary<int, ASTNode>();

            if (root.MainPlot != null)
            {
                generate(root.MainPlot);
            }

            foreach (KeyValuePair<string, PlotDef> subPlot in root.SubPlotMap)
            {
                generate(subPlot.Value);
            }

            foreach (KeyValuePair<string, FunctionDef> funcDef in root.FuncDefMap)
            {
                generate(funcDef.Value);
            }

            foreach (KeyValuePair<string, ActionLayerDef> actionLayerDef in root.ActionLayerMap)
            {
                generate(actionLayerDef.Value);
            }

            foreach (KeyValuePair<string, AssetDef> assetDef in root.AssetMap)
            {
                generate(assetDef.Value);
            }
        }

        private void generate(ASTNode node)
        {
            node.ID = counter;
            root.NodeMap.Add(counter, node);

            counter++;

            if (node.Children != null)
            {
                foreach (ASTNode child in node.Children)
                {
                    generate(child);
                }
            }
        }
    }
}
