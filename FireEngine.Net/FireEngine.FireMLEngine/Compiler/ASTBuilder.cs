using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using FireEngine.FireMLEngine.AST;
using FireEngine.FireMLData;

namespace FireEngine.FireMLEngine.Compiler
{
    class ASTBuilder : AST.IASTVisitor
    {
        private XmlTextReader reader;
        private string file;

        private CompilerKernel kernel;
        private Translator dic;
        private ExpressionParser exprParser;

        private bool xmlError = false;

        //private Dictionary<string, List<string>> funcParaStrMap = new Dictionary<string, List<string>>();

        public ASTBuilder(CompilerKernel kernel)
        {
            this.kernel = kernel;
            dic = new Translator(null); //TODO: Translate!
            exprParser = new ExpressionParser(kernel);
        }

        public void Build(string[] plotFiles, FireMLRoot root)
        {
            foreach (string file in plotFiles)
            {
                this.file = file;
                xmlError = false;
                //FINISH: 检查XML语法
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Schemas = kernel.FireMLSchemaSet;
                try
                {
                    xmlDoc.Load(file);
                }
                catch (Exception e)
                {
                    kernel.IssueError(ErrorType.ReadFileError, e.Message, new Location(file));
                    continue;
                }
                xmlDoc.Validate(new ValidationEventHandler(settings_ValidationEventHandler));
                if (xmlError)
                    continue;
                

                XmlTextReader reader;
                try
                {
                    reader = new XmlTextReader(file);
                }
                catch (Exception e)
                {
                    kernel.IssueError(ErrorType.ReadFileError, e.Message, new Location(file));
                    continue;
                }


                for (; reader.Name != "FireML"; reader.Read()) ;
                reader.MoveToContent();

                this.reader = reader;

                root.Accept(this);
            }
        }

        void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            xmlError = true;
            kernel.IssueError(ErrorType.PlotXMLError, new Location(file), e.Message);
        }

        #region IASTVisitor Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">
        /// XMLReader, filename
        /// </param>
        public void Visit(FireMLRoot root, object[] args)
        {
            //reader = args[0] as XmlTextReader;
            //file = args[1] as string;

            while (reader.Read())
            {
                reader.MoveToContent();
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        Location location = new Location(file,reader.LineNumber, reader.LinePosition);
                        switch (dic[reader.Name])
                        {
                            case "MainPlot":
                                if (root.MainPlot != null)
                                {
                                    kernel.IssueError(ErrorType.DuplicatedMainPlot, location);
                                    continue;
                                }

                                PlotDef plotDef = new PlotDef();
                                plotDef.Location = location;

                                root.MainPlot = plotDef;
                                //reader.MoveToContent();
                                plotDef.Accept(this);
                                break;
                                
                            case "SubPlot":
                                reader.MoveToAttribute("name");
                                string subPlotName = reader.Value;
                                if(root.SubPlotMap.ContainsKey(subPlotName))
                                {
                                    kernel.IssueError(ErrorType.DuplicatedSubPlot, subPlotName,
                                        new Location(file, reader.LineNumber, reader.LinePosition));
                                    continue;
                                }

                                PlotDef subPlotDef = new PlotDef();
                                subPlotDef.Location = location;
                                subPlotDef.Name = subPlotName;

                                root.SubPlotMap.Add(subPlotName, subPlotDef);
                                //reader.MoveToContent();
                                subPlotDef.Accept(this);
                                break;

                            case "ActionLayer":
                                reader.MoveToAttribute("name");
                                string layerName = reader.Value;
                                if (root.ActionLayerMap.ContainsKey(layerName))
                                {
                                    kernel.IssueError(ErrorType.DuplicatedActorLayer, layerName,
                                        new Location(file,reader.LineNumber, reader.LinePosition));
                                    continue;
                                }

                                ActionLayerDef actionLayerDef = new ActionLayerDef();
                                actionLayerDef.Location = location;
                                actionLayerDef.Name = layerName;

                                root.ActionLayerMap.Add(layerName, actionLayerDef);
                                //reader.Read();
                                reader.MoveToElement();
                                actionLayerDef.Accept(this);
                                break;

                            case "Function":
                                reader.MoveToAttribute("name");
                                string funcName = reader.Value;
                                if (root.FuncDefMap.ContainsKey(funcName))
                                {
                                    kernel.IssueError(ErrorType.DuplicatedFunction, funcName,
                                        new Location(file, reader.LineNumber, reader.LinePosition));
                                    continue;
                                }

                                FunctionDef funcDef = new FunctionDef();
                                funcDef.Location = location;
                                funcDef.Name = funcName;

                                root.FuncDefMap.Add(funcName, funcDef);
                                //reader.MoveToContent();
                                funcDef.Accept(this);
                                break;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        reader.Read();
                        break;
                }
            }
        }

        public void Visit(ActionLayerDef actionLayerDef, object[] args)
        {
            //reader.Read();
            //reader.MoveToContent();
            reader.MoveToAttribute("position");

            actionLayerDef.Position = PositionData.Parse(reader.Value);
            
            //reader.Read();  //</ActionLayer>
        }

        public void Visit(FunctionDef functionDef, object[] args)
        {
            Location location = new Location(file, reader.LineNumber, reader.LinePosition);

            functionDef.ParaMap = new Dictionary<string, ParameterDef>();
            functionDef.FuncDefContent = new List<Statement>();
            //funcParaStrMap.Add(functionDef.Name, new List<string>());
            functionDef.ParaStrMap = new Dictionary<int, string[]>();

            while (reader.Read())
            {
                reader.MoveToContent();
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        switch (dic[reader.Name])
                        {
                            case "para":
                                reader.MoveToAttribute("name");
                                string parName = reader.Value;
                                parName = parName.Substring(1, parName.Length - 1); //去掉$符
                                if (functionDef.ParaMap.ContainsKey(parName))
                                {
                                    kernel.IssueError(ErrorType.DuplicatedParameter,
                                        new Location(file, reader.LineNumber, reader.LinePosition),
                                        parName,
                                        functionDef.Name
                                        );
                                    continue;
                                }

                                ParameterDef parDef = new ParameterDef();
                                parDef.Location = location;
                                parDef.Parent = functionDef;
                                functionDef.ParaMap.Add(parName, parDef);

                                parDef.Accept(this);

                                break;

                            case "parastr":
                                //funcParaStrMap[functionDef.Name].Add(reader.ReadString());
                                string paraStrDef = reader.ReadString();
                                string[] paraStr = ParaStrProcessor.ReadDefVarList(paraStrDef);
                                if (paraStr == null)
                                {
                                    kernel.IssueError(ErrorType.ParaStrDefFormatError, getCurrentLoc(), paraStrDef);
                                }
                                else if(functionDef.ParaStrMap.ContainsKey(paraStr.Length))
                                {
                                    kernel.IssueError(ErrorType.SameNumberInParaStr, getCurrentLoc());
                                }
                                else
                                {
                                    foreach (string varName in paraStr)
                                    {
                                        if (!functionDef.ParaMap.ContainsKey(varName))
                                        {
                                            kernel.IssueError(ErrorType.ParameterNotDefined, getCurrentLoc(), varName);
                                        }
                                    }
                                    functionDef.ParaStrMap.Add(paraStr.Length, paraStr);
                                }
                                break;

                            case "funcdef":
                                if (reader.IsEmptyElement)
                                    break;

                                visitMainContent(functionDef, functionDef.FuncDefContent);
                                break;

                            default:
                                break;
                        }
                        break;

                    case XmlNodeType.EndElement:
                        reader.Read();
                        return;
                }
            }
        }

        public void Visit(PlotDef plotDef, object[] args)
        {
            if (reader.IsEmptyElement)
                return;

            plotDef.Content = new List<Statement>();
            visitMainContent(plotDef, plotDef.Content);
        }

        public void Visit(ContinueStmt continueStmt, object[] args)
        {
            //FINISH: 检查合法性！
        }

        public void Visit(FunctionCallStmt funcCallStmt, object[] args)
        {
            //检查存在性！
            funcCallStmt.Name = reader.Name;
            funcCallStmt.ParamMap = new Dictionary<string, FireEngine.FireMLEngine.Expr.Expression>();
            
            int attCount = reader.AttributeCount;
            for (int i = 0; i < attCount; i++)
            {
                reader.MoveToAttribute(i);
                if (reader.Name == "return")
                {
                    funcCallStmt.ReturnDest = exprParser.ParseLeftValueExpr(reader.Value,
                        new Location(file, reader.LineNumber, reader.LinePosition).Offset(reader.Name.Length + 2)); //TODO: Location
                }
                else if (reader.Name == "par")
                {
                    //FINISH: 解析参数字串！
                    string[] paraStrList = ParaStrProcessor.ReadParaCallList(reader.Value);
                    if (paraStrList == null)
                    {
                        kernel.IssueError(ErrorType.ParaStrCallFormatError, getCurrentLoc(), reader.Value);
                    }
                    else
                    {
                        funcCallStmt.ParaStr = paraStrList;
                    }
                }
                else
                {
                    //先把实参按字符串暂存，之后扫描AST树时再按指定类型解析
                    funcCallStmt.ParamMap.Add(reader.Name, 
                        exprParser.CreateStringConst(reader.Value,
                            new Location(file, reader.LineNumber, reader.LinePosition).Offset(reader.Name.Length + 2)   //TODO: Location
                            ));
                }
            }
        }

        public void Visit(MusicStmt musicStmt, object[] args)
        {
            //检查存在性
            bool srcDefined;
            if (srcDefined = reader.MoveToAttribute("src"))
            {
                musicStmt.Source = reader.Value;
            }

            bool assetDefined;
            if (assetDefined = reader.MoveToAttribute("asset"))
            {
                musicStmt.Asset = reader.Value;
            }

            if (srcDefined && assetDefined)
            {
                kernel.IssueError(ErrorType.BothSrcAndAsset, getCurrentLoc());
            }
            else if (!srcDefined && !assetDefined)
            {
                kernel.IssueError(ErrorType.SrcAndAssetMissed, getCurrentLoc());
            }

            if (reader.MoveToAttribute("loop"))
            {
                musicStmt.Loop = (reader.Value == "true" || reader.Value == "1") ? true : false;
            }

            if (reader.MoveToAttribute("fadeIn"))
            {
                musicStmt.FadeIn = TimeSpan.FromSeconds(double.Parse(reader.Value));
            }
            else
            {
                musicStmt.FadeIn = TimeSpan.Zero;
            }

        }

        public void Visit(MusicStopStmt musicStopStmt, object[] args)
        {
            if (reader.MoveToAttribute("fadeOut"))
            {
                musicStopStmt.FadeOut = TimeSpan.FromSeconds(double.Parse(reader.Value));
            }
            else
            {
                musicStopStmt.FadeOut = TimeSpan.Zero;
            }
        }

        public void Visit(MusicVolStmt musicVolStmt, object[] args)
        {
            if (reader.MoveToAttribute("amplitude"))
            {
                musicVolStmt.Amplitude = double.Parse(reader.Value);
            }
            else
            {
                musicVolStmt.Amplitude = 1;
            }

            if(reader.MoveToAttribute("transition"))
            {
                musicVolStmt.TransitionTime = TimeSpan.FromSeconds(double.Parse(reader.Value));
            }
            else
            {
                musicVolStmt.TransitionTime = TimeSpan.Zero;
            }
        }

        public void Visit(SwitchStmt switchStmt, object[] args)
        {
            reader.MoveToAttribute("expr");
            switchStmt.Expression = exprParser.ParseExpr(reader.Value,
                new Location(file,reader.LineNumber, reader.LinePosition).Offset(6));

            switchStmt.SwitchCaseList = new List<SwitchCase>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    SwitchCase switchCase = new SwitchCase();
                    switchCase.Parent = switchStmt;
                    switchCase.Location = new Location(file, reader.LineNumber, reader.LinePosition);

                    switchCase.Accept(this);
                    switchStmt.SwitchCaseList.Add(switchCase);
                }
                else if(reader.NodeType == XmlNodeType.EndElement)
                {
                    break;
                }
            }
        }

        public void Visit(SelectStmt selectStmt, object[] args)
        {
            bool complex = (dic[reader.Name] == "selectWithValue");

            reader.MoveToAttribute("return");
            selectStmt.Return = exprParser.ParseLeftValueExpr(reader.Value, 
                new Location(file, reader.LineNumber, reader.LinePosition).Offset(8));
            selectStmt.OptionList = new List<SelectOption>();

            //FINISH: 复杂形式
            if (!complex)
            {
                string text = reader.ReadString();
                string[] lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                int counter = 0;
                foreach (string line in lines)
                {
                    string trimed = line.Trim();
                    if (trimed.Length == 0)
                        continue;

                    SelectOption option = new SelectOption();
                    option.Parent = selectStmt;
                    option.Location = new Location(file, reader.LineNumber, reader.LinePosition);
                    option.Text = trimed;
                    option.Value = exprParser.ParseValue(counter, FireEngine.FireMLEngine.Expr.DataType.Int,
                        new Location(file, reader.LineNumber, reader.LinePosition));
                    selectStmt.OptionList.Add(option);

                    counter++;
                }
            }
            else
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            SelectOption option = new SelectOption();
                            option.Parent = selectStmt;
                            option.Location = getCurrentLoc();
                            option.Accept(this);
                            selectStmt.OptionList.Add(option);
                            break;

                        case XmlNodeType.EndElement:
                            return;
                    }
                }
            }
        }

        public void Visit(ActorStmt actorStmt, object[] args)
        {
            //TODO: 参数字串
            if(reader.MoveToAttribute("name"))
            {
                actorStmt.Name = reader.Value;
            }

            //FINISH: 检查
            bool imgDefined;
            if (imgDefined = reader.MoveToAttribute("img"))
            {
                actorStmt.Img = reader.Value;
            }

            bool assetDefined;
            if (assetDefined = reader.MoveToAttribute("asset"))
            {
                actorStmt.Asset = reader.Value;
            }

            if (imgDefined && assetDefined)
            {
                kernel.IssueError(ErrorType.BothSrcAndAsset, new Location(file, reader.LineNumber, reader.LinePosition));
            }

            bool avatarDefined;
            if (avatarDefined = reader.MoveToAttribute("avatar"))
            {
                actorStmt.Avatar = reader.Value;
            }
            bool avaassetDefined;
            if (avaassetDefined = reader.MoveToAttribute("avaasset"))
            {
                actorStmt.AvaAsset = reader.Value;
            }
            if (avatarDefined && avaassetDefined)
            {
                kernel.IssueError(ErrorType.BothSrcAndAsset, new Location(file, reader.LineNumber, reader.LinePosition));
            }

            //FINISH: 检查存在性
            bool layerDefined;
            if (layerDefined = reader.MoveToAttribute("layer"))
            {
                actorStmt.Layer = reader.Value;
            }

            bool posDefined;
            if (posDefined = reader.MoveToAttribute("position"))
            {
                actorStmt.Position = PositionData.Parse(reader.Value);
            }

            if (layerDefined && posDefined)
            {
                kernel.IssueError(ErrorType.BothActorLayerAndCustom, new Location(file, reader.LineNumber, reader.LinePosition));
            }
        }

        public void Visit(DialogStmt dialogStmt, object[] args)
        {
            
        }

        public void Visit(IfStmt ifStmt, object[] args)
        {
            ifStmt.BlockList = new List<IfBlock>();
            do
            {
                IfBlock block = new IfBlock();
                block.Content = new List<Statement>();
                block.Parent = ifStmt;
                block.Location = new Location(file, reader.LineNumber, reader.LinePosition);

                if (dic[reader.Name] == "else")
                {
                    block.Condition = null;
                }
                else
                {
                    reader.MoveToAttribute("cond");
                    string condExpr = reader.Value;
                    Expr.Expression expr = exprParser.ParseExpr(condExpr,
                        new Location(file, reader.LineNumber, reader.LinePosition).Offset(6));  //TODO：Location
                    block.Condition = expr;
                }

                visitMainContent(block, block.Content);

                ifStmt.BlockList.Add(block);
            } while (dic[reader.Name] == "elseif" || dic[reader.Name] == "else");
        }

        public void Visit(LoopStmt loopStmt, object[] args)
        {
            reader.MoveToAttribute("cond");
            loopStmt.Condition = exprParser.ParseExpr(reader.Value,
                new Location(file, reader.LineNumber, reader.LinePosition).Offset(6));  //TODO: Location
            loopStmt.Content = new List<Statement>();

            visitMainContent(loopStmt, loopStmt.Content);
        }

        public void Visit(BackgroundStmt backgroundStmt, object[] args)
        {
            //TODO: 参数字串
            bool imgDefined;
            if (imgDefined = reader.MoveToAttribute("img"))
            {
                backgroundStmt.Img = reader.Value;
            }

            //FINISH: 存在性
            bool assetDefined;
            if (assetDefined = reader.MoveToAttribute("asset"))
            {
                backgroundStmt.Asset = reader.Value;
            }

            if (imgDefined && assetDefined)
            {
                kernel.IssueError(ErrorType.BothSrcAndAsset, new Location(file, reader.LineNumber, reader.LinePosition));
            }

            if (!imgDefined && !assetDefined)
            {
                kernel.IssueError(ErrorType.SrcAndAssetMissed, new Location(file, reader.LineNumber, reader.LinePosition));
            }
        }

        public void Visit(EchoStmt echoStmt, object[] args)
        {
            string expr = reader.ReadString();

            Expr.Expression result = exprParser.ParseExpr(expr,
                new Location(file, reader.LineNumber, reader.LinePosition).Offset(6));
            echoStmt.Expression = result;
        }

        public void Visit(IncludeStmt includeStmt, object[] args)
        {
            //FINISH: 检查存在性
            reader.MoveToAttribute("SubPlot");
            includeStmt.SubPlot = reader.Value;
        }

        public void Visit(BreakStmt breakStmt, object[] args)
        {
            //FINISH: 检查合法性！
        }

        public void Visit(ExpressionStmt expressionStmt, object[] args)
        {
            expressionStmt.ExpressionList = new List<Expr.Expression>();

            string expr = reader.ReadString();

            bool escape = false;
            bool inQuot = false;

            int firstPos = 0;
            string subExpr;

            for (int i = 0; i < expr.Length; i++)
            {
                if (expr[i] == '\\')
                {
                    escape = !escape;
                }
                else if (expr[i] == '"')
                {
                    if (!inQuot)
                    {
                        inQuot = true;
                    }
                    else if (escape)
                    {
                        escape = false;
                    }
                    else
                    {
                        inQuot = false;
                    }
                }
                else if (expr[i] == ';')
                {
                    subExpr = expr.Substring(firstPos, i - firstPos);
                    if (subExpr.Trim().Length > 0)
                    {
                        Expr.Expression result = exprParser.ParseExpr(subExpr,
                            new Location(file, reader.LineNumber, reader.LinePosition).Offset(6 + firstPos));
                        expressionStmt.ExpressionList.Add(result);
                    }

                    firstPos = i + 1;
                    escape = false;
                }
                else
                {
                    escape = false;
                }               
            }

            if (firstPos < expr.Length)
            {
                subExpr = expr.Substring(firstPos, expr.Length - firstPos);
                if (subExpr.Trim().Length > 0)
                {
                    Expr.Expression result = exprParser.ParseExpr(subExpr,
                        new Location(file, reader.LineNumber, reader.LinePosition).Offset(6 + firstPos));
                    expressionStmt.ExpressionList.Add(result);
                }
            }
        }

        public void Visit(ReturnStmt returnStmt, object[] args)
        {
            string expr = reader.ReadString();

            Expr.Expression result = exprParser.ParseExpr(expr,
                new Location(file, reader.LineNumber, reader.LinePosition).Offset(8));
            returnStmt.Expression = result;
        }

        public void Visit(ParameterDef parameterDef, object[] args)
        {


            if (reader.MoveToAttribute("type"))
            {
                parameterDef.ParameterType = (ParameterDef.ParameterTypeEnum)Enum.Parse(typeof(ParameterDef.ParameterTypeEnum), reader.Value);
            }
            else
            {
                parameterDef.ParameterType = ParameterDef.ParameterTypeEnum.Auto;
            }
            
            if (reader.MoveToAttribute("default"))
            {
                Location loc = new Location(file, reader.LineNumber, reader.LinePosition).Offset(9);    //TODO: Position
                switch (parameterDef.ParameterType)
                {
                    case ParameterDef.ParameterTypeEnum.Auto:
                        parameterDef.Default = exprParser.ParseValue(reader.Value, loc);
                        break;

                    case ParameterDef.ParameterTypeEnum.String:
                        parameterDef.Default = exprParser.ParseValue(reader.Value, Expr.DataType.String, loc);
                        break;

                    case ParameterDef.ParameterTypeEnum.Expression:
                        parameterDef.Default = exprParser.ParseValue(reader.Value, loc);
                        break;

                    case ParameterDef.ParameterTypeEnum.Int:
                        parameterDef.Default = exprParser.ParseValue(reader.Value, Expr.DataType.Int, loc);
                        break;

                    case ParameterDef.ParameterTypeEnum.Float:
                        parameterDef.Default = exprParser.ParseValue(reader.Value, Expr.DataType.Float, loc);
                        break;

                    case ParameterDef.ParameterTypeEnum.Bool:
                        parameterDef.Default = exprParser.ParseValue(reader.Value, Expr.DataType.Bool, loc);
                        break;
                }
            }
        }

        public void Visit(SelectOption selectOption, object[] args)
        {
            reader.MoveToAttribute("value");
            selectOption.Value = exprParser.ParseValue(reader.Value, getCurrentLoc());
            selectOption.Text = reader.ReadString();
        }

        public void Visit(SwitchCase switchCase, object[] args)
        {
            switchCase.CaseContent = new List<Statement>();
            if (reader.Name == "default")
            {
                switchCase.Value = null;
            }
            else
            {
                reader.MoveToAttribute("value");
                switchCase.Value = exprParser.ParseValue(reader.Value,
                    new Location(file, reader.LineNumber, reader.LinePosition).Offset(7));
                switchCase.CaseContent = new List<Statement>();
            }
            visitMainContent(switchCase, switchCase.CaseContent);
        }

        public void Visit(IfBlock ifBlock, object[] args)
        {
            
        }

        
        public void Visit(DataStmt dataStmt, object[] args)
        {
            
        }

        public void Visit(AssetDef assetDef, object[] args)
        {

        }

        private void visitMainContent(ASTNode parent, List<Statement> content)
        {
            bool endWithBr = false;
            while (reader.Read())
            {
                reader.MoveToContent();
                Location location = new Location(file, reader.LineNumber, reader.LinePosition);
                switch (reader.NodeType)
                {
                    case XmlNodeType.Text:
                        string text = reader.Value;
                        string[] lines = text.Split(new char[] { '\n', '\r'}, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string line in lines)
                        {
                            string trimed = line.Trim();
                            if (trimed.Length == 0)
                                continue;

                            if (endWithBr && content.Count > 0 && content[content.Count - 1] is DialogStmt)
                            {
                                DialogStmt dialog = content[content.Count - 1] as DialogStmt;
                                dialog.Text += Environment.NewLine + trimed;
                            }
                            else
                            {
                                DialogStmt dialog = new DialogStmt();
                                dialog.Parent = parent;
                                dialog.Location = location;

                                dialog.Text = trimed;
                                content.Add(dialog);
                            }
                            endWithBr = false;
                        }
                        break;

                    case XmlNodeType.Element:
                        Statement statement = null;
                        switch (dic[reader.Name])
                        {
                            case "br":
                                endWithBr = true;
                                continue;

                            case "expr":
                                statement = new ExpressionStmt();
                                break;

                            case "return":
                                statement = new ReturnStmt();
                                break;

                            case "include":
                                statement = new IncludeStmt();
                                break;

                            case "actor":
                                statement = new ActorStmt();
                                break;

                            case "bg":
                                statement = new BackgroundStmt();
                                break;

                            case "echo":
                                statement = new EchoStmt();
                                break;

                            case "select":
                                statement = new SelectStmt();
                                break;

                            case "selectWithValue":
                                statement = new SelectStmt();
                                break;

                            case "if":
                                statement = new IfStmt();
                                break;

                            case "else":
                                return;

                            case "elseif":
                                return;

                            case "switch":
                                statement = new SwitchStmt();
                                break;

                            case "break":
                                statement = new BreakStmt();
                                break;

                            case "continue":
                                statement = new ContinueStmt();
                                break;

                            case "loop":
                                statement = new LoopStmt();
                                break;

                            case "music":
                                statement = new MusicStmt();
                                break;

                            case "musicStop":
                                statement = new MusicStopStmt();
                                break;

                            case "musicVol":
                                statement = new MusicVolStmt();
                                break;

                            default:
                                statement = new FunctionCallStmt();
                                break;
                        }
                        statement.Parent = parent;
                        statement.Location = location;
                        statement.Accept(this);
                        content.Add(statement);
                        break;

                    case XmlNodeType.EndElement:
                        //reader.Read();  //MainContent结束
                        return;
                }
            }
        }

        //private void parsePosition(string str, out int x, out int y)
        //{
        //    string[] nums = str.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        //    x = int.Parse(nums[0]);
        //    y = int.Parse(nums[1]);
        //}

        private Location getCurrentLoc()
        {
            return new Location(file, reader.LineNumber, reader.LinePosition);
        }

        #endregion

    }
}
