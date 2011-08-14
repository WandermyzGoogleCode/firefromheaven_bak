using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine.AST;
using FireEngine.FireMLData.Asset;
using FireEngine.ContentInterface;
using FireEngine.FireMLData;

namespace FireEngine.FireMLEngine.Compiler
{
    class ContentChecker : ASTRecursor, IDataCheckHelper
    {
        CompilerKernel kernel;
        IContentManager contentManager;
        FireMLRoot root;
        Location checkLocation; //当前正在被检查的位置，用于序列化

        public ContentChecker(CompilerKernel kernel)
        {
            this.kernel = kernel;
            this.contentManager = kernel.ContentManager;
        }

        #region IASTVisitor Members

        public override void Visit(FireMLRoot root, object[] args)
        {
            this.root = root;
            foreach (AssetDef asset in root.AssetMap.Values)
            {
                asset.Accept(this);
            }
            base.Visit(root, args);
        }

        public override void Visit(MusicStmt musicStmt, object[] args)
        {
            if (musicStmt.Source != null && !StrVarRefProcessor.IsVariableIncluded(musicStmt.Source))
            {
                CheckContent(musicStmt.Source, ContentType.Music, musicStmt.Location);
            }

            if (musicStmt.Asset != null)
            {
                CheckAsset(musicStmt.Asset, typeof(MusicAsset), musicStmt.Location);
            }

            base.Visit(musicStmt, args);
        }

        public override void Visit(ActorStmt actorStmt, object[] args)
        {
            if (actorStmt.Img != null && !StrVarRefProcessor.IsVariableIncluded(actorStmt.Img))
            {
                CheckContent(actorStmt.Img, ContentType.Texture, actorStmt.Location);
            }

            if (actorStmt.Asset != null)
            {
                CheckAsset(actorStmt.Asset, typeof(ActorAsset), actorStmt.Location);
            }

            if (actorStmt.Avatar != null && !StrVarRefProcessor.IsVariableIncluded(actorStmt.Avatar))
            {
                CheckContent(actorStmt.Avatar, ContentType.Texture, actorStmt.Location);
            }

            if (actorStmt.AvaAsset != null)
            {
                CheckAsset(actorStmt.AvaAsset, typeof(ActorAsset), actorStmt.Location);
            }
        }

        public override void Visit(BackgroundStmt backgroundStmt, object[] args)
        {
            if (backgroundStmt.Img != null && !StrVarRefProcessor.IsVariableIncluded(backgroundStmt.Img))
            {
                CheckContent(backgroundStmt.Img, ContentType.Texture, backgroundStmt.Location);
            }

            if (backgroundStmt.Asset != null)
            {
                CheckAsset(backgroundStmt.Asset, typeof(CGAsset), backgroundStmt.Location);
            }
        }

        public override void Visit(DataStmt dataStmt, object[] args)
        {
            checkLocation = dataStmt.Location;
            dataStmt.Data.CheckContent(this);
            base.Visit(dataStmt, args);
        }

        public override void Visit(AssetDef assetDef, object[] args)
        {
            checkLocation = assetDef.Location;
            assetDef.AssetData.CheckContent(this);
            base.Visit(assetDef, args);
        }

        #endregion

        /*
        #region IAssetVisitor Members

        public void Visit(ActorAsset actorAsset, object[] args)
        {
            CheckContent(actorAsset.Source, ContentType.Texture, actorAsset.Location);
        }

        public void Visit(CGAsset cgAsset, object[] args)
        {
            CheckContent(cgAsset.Source, ContentType.Texture, cgAsset.Location);
        }

        public void Visit(MusicAsset musicAsset, object[] args)
        {
            CheckContent(musicAsset.Source, ContentType.Music, musicAsset.Location);
        }

        public void Visit(VideoAsset videoAsset, object[] args)
        {
            CheckContent(videoAsset.Source, ContentType.Video, videoAsset.Location);
        }

        #endregion
        */

        public void Check(FireMLRoot root)
        {
            root.Accept(this);
        }

        private bool checkSourceResult(AvailableCheckResult result, string source, string expectedType, Location location)
        {
            switch (result)
            {
                case AvailableCheckResult.Available:
                    return true;
                    
                case AvailableCheckResult.NotExist:
                    kernel.IssueError(ErrorType.SourceNotExist, location, source);
                    return false;

                case AvailableCheckResult.NotSupported:
                    kernel.IssueError(ErrorType.SourceNotSupported, location, source);
                    return false;

                case AvailableCheckResult.TypeError:
                    //TODO:类型检查
                    kernel.IssueError(ErrorType.SourceTypeError, location, expectedType, "TODO!");
                    return false;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region IDataCheckHelper Members

        public bool CheckContent(string path, ContentType expectedType)
        {
            AvailableCheckResult result = contentManager.CheckAvailabe(path, expectedType);
            return checkSourceResult(result, path, expectedType.ToString(), checkLocation);
        }

        public bool CheckSubPlot(string name)
        {
            throw new NotSupportedException();
        }

        public bool CheckAsset(string name, Type expectedType)
        {
            bool pass = true;
            if (!root.AssetMap.ContainsKey(name))
                pass = false;
            else if (!expectedType.IsInstanceOfType(root.AssetMap[name]))
                pass = false;

            if (!pass)
            {
                kernel.IssueError(ErrorType.AssetNotExist, checkLocation, name, expectedType.Name);
            }

            return pass;
        }

        #endregion

        public bool CheckAsset(string name, Type expectedType, Location location)
        {
            checkLocation = location;
            return CheckAsset(name, expectedType);
        }

        public bool CheckContent(string path, ContentType expectedType, Location location)
        {
            checkLocation = location;
            return CheckContent(path, expectedType);
        }
    }
}
