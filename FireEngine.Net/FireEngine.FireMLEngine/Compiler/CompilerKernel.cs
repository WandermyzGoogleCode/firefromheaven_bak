using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using FireEngine.ContentInterface;
using FireEngine.FireMLEngine.AST;

namespace FireEngine.FireMLEngine.Compiler
{
    public class CompilerKernel
    {
        private string[] plotFiles;
        private string[] assetFiles;
        private XmlSchemaSet fireMLSchemaSet;
        internal XmlSchemaSet FireMLSchemaSet
        {
            get { return fireMLSchemaSet; }
        }
        internal IContentManager ContentManager { get; set; }

        private List<Error> errorList = new List<Error>();

        public CompilerKernel(string[] plotFiles, string[] assetFiles, string xsdDirPath, IContentManager contentManager)
        {
            this.plotFiles = plotFiles;
            this.assetFiles = assetFiles;
            this.ContentManager = contentManager;

            fireMLSchemaSet = new XmlSchemaSet();
            foreach(string xsdPath in Directory.GetFiles(xsdDirPath, "*.xsd", SearchOption.AllDirectories))
            {
                fireMLSchemaSet.Add(FireEngine.Library.FireEngineConstant.FIREML_XMLNS, xsdPath);
            }
        }

        /// <summary>
        /// 编译FireML
        /// </summary>
        public FireMLRoot CompileFireML()
        {
            FireMLRoot root = new FireMLRoot();

            #region 读取Asset
            AssetBuilder assetBuilder = new AssetBuilder(this);
            assetBuilder.Build(assetFiles, root);
            #endregion

            #region 构造AST
            ASTBuilder astBuilder = new ASTBuilder(this);
            astBuilder.Build(plotFiles, root);
            #endregion

            #region AST合法性检查
            ASTChecker astChecker = new ASTChecker(this);
            astChecker.Check(root);
            #endregion

            #region 生成ID号
            IDGenerator idGenerator = new IDGenerator();
            idGenerator.Generate(root);
            #endregion

            #region 资源存在性检查
            //ContentChecker contentChecker = new ContentChecker(this);
            //contentChecker.Check(root);
            #endregion

            if (errorList.Count > 0)
                return null;
            else
                return root;
        }

        /// <summary>
        /// 返回当前所有的错误，并清空错误列表
        /// </summary>
        public Error[] CheckPoint()
        {
            Error[] errorArray = errorList.ToArray();
            errorList.Clear();
            return errorArray;
        }

        internal void IssueError(ErrorType errorType, Location location)
        {
            errorList.Add(new Error(errorType, location));
        }

        internal void IssueError(ErrorType errorType, string msg, Location location)
        {
            errorList.Add(new Error(errorType, msg, location));
        }

        internal void IssueError(ErrorType errorType, Location location, params string[] args)
        {
            errorList.Add(new Error(errorType, location, args));
        }

        #region Properties

        public string[] FireMLPlotFiles
        {
            get
            {
                return plotFiles;
            }
        }

        public string[] FireMLAssetFiles
        {
            get
            {
                return assetFiles;
            }
        }

        #endregion
    }
}
