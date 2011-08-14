using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using FireEngine.FireMLData.Asset;
using FireEngine.Library;
using FireEngine.FireMLEngine.AST;
using FireEngine.FireMLData;

namespace FireEngine.FireMLEngine.Compiler
{
    class AssetBuilder
    {
        private CompilerKernel kernel;
        private Dictionary<string, AssetDef> assetMap;
        private string currentFile;
        private XmlNode currentNode;
        private Translator dic;
        private bool xmlError = false;

        internal AssetBuilder(CompilerKernel kernel)
        {
            this.kernel = kernel;
            dic = new Translator(null); //TODO: Translate!
        }

        /*
        #region IAssetVisitor Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actorAsset"></param>
        /// <param name="args">XML Node</param>
        public void Visit(ActorAsset actorAsset, object[] args)
        {
            //FINISH: 检查资源存在性
            actorAsset.Source = currentNode.Attributes["src"].Value;

            foreach (XmlNode child in currentNode.ChildNodes)
            {
                switch (dic[child.Name])
                {
                    case "group":
                        actorAsset.Group = child.InnerText;
                        break;

                    default:
                        continue;
                }
            }

            if (assetMap.ContainsKey(actorAsset.Name))
            {
                kernel.IssueError(ErrorType.DuplicatedAsset, new Location(currentFile),
                    actorAsset.Name, "Actor");
                return;
            }

            assetMap.Add(actorAsset.Name, actorAsset);
        }

        public void Visit(CGAsset cgAsset, object[] args)
        {
            //FINISH: 检查资源存在性
            cgAsset.Source = currentNode.Attributes["src"].Value;

            foreach (XmlNode child in currentNode.ChildNodes)
            {
                switch (dic[child.Name])
                {
                    case "group":
                        cgAsset.Group = child.InnerText;
                        break;

                    default:
                        continue;
                }
            }

            if (assetMap.ContainsKey(cgAsset.Name))
            {
                kernel.IssueError(ErrorType.DuplicatedAsset, new Location(currentFile),
                    cgAsset.Name, "CG");
                return;
            }

            assetMap.Add(cgAsset.Name, cgAsset);
        }

        public void Visit(MusicAsset musicAsset, object[] args)
        {
            //TODO: 检查资源存在性
            musicAsset.Source = currentNode.Attributes["src"].Value;

            foreach (XmlNode child in currentNode.ChildNodes)
            {
                switch (dic[child.Name])
                {
                    case "group":
                        musicAsset.Group = child.InnerText;
                        break;

                    case "title":
                        musicAsset.Title = child.InnerText;
                        break;
                        
                    case "artist":
                        musicAsset.Artist = child.InnerText;
                        break;

                    case "lyricsBy":
                        musicAsset.LyricsBy = child.InnerText;
                        break;

                    case "musicBy":
                        musicAsset.MusicBy = child.InnerText;
                        break;

                    case "description":
                        musicAsset.Description = child.InnerText;
                        break;

                    case "lyrics":
                        musicAsset.Lyrics = child.InnerText;
                        break;

                    case "lrc":
                        musicAsset.LRC = child.InnerText;
                        //TODO: 检查和解析LRC
                        break;

                    default:
                        continue;
                }
            }

            if (assetMap.ContainsKey(musicAsset.Name))
            {
                kernel.IssueError(ErrorType.DuplicatedAsset, new Location(currentFile),
                    musicAsset.Name, "Music");
                return;
            }

            assetMap.Add(musicAsset.Name, musicAsset);
        }

        public void Visit(VideoAsset videoAsset, object[] args)
        {
            //TODO: 检查资源存在性
            videoAsset.Source = currentNode.Attributes["src"].Value;

            if (assetMap.ContainsKey(videoAsset.Name))
            {
                kernel.IssueError(ErrorType.DuplicatedAsset, new Location(currentFile),
                    videoAsset.Name, "Video");
                return;
            }

            assetMap.Add(videoAsset.Name, videoAsset);
        }

        #endregion
        */

        public void Build(string[] assetFiles, FireMLRoot root)
        {
            assetMap = root.AssetMap;

            foreach (string file in assetFiles)
            {
                currentFile = file;
                xmlError = false;

                XmlDocument doc = new XmlDocument();
                doc.Schemas = kernel.FireMLSchemaSet;
                try
                {
                    doc.Load(file);
                }
                catch (Exception e)
                {
                    kernel.IssueError(ErrorType.ReadFileError, new Location(file), e.Message);
                    continue;
                }
                doc.Validate(new System.Xml.Schema.ValidationEventHandler(settings_ValidationEventHandler));
                if (xmlError)
                    continue;

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("fm",FireEngine.Library.FireEngineConstant.FIREML_XMLNS);

                foreach (XmlNode node in doc.SelectSingleNode("fm:FireMLAsset",nsmgr).ChildNodes)
                {
                    AssetDef def = new AssetDef();
                    AssetDataBase data;
                    
                    switch (dic[node.Name])
                    {
                        case "Actor":
                            data = FireMLDataBase.Deserialize<ActorAsset>(node.OuterXml);
                            break;

                        case "CG":
                            data = FireMLDataBase.Deserialize<CGAsset>(node.OuterXml);
                            break;

                        case "Music":
                            data = FireMLDataBase.Deserialize<MusicAsset>(node.OuterXml);
                            break;

                        case "Video":
                            data = FireMLDataBase.Deserialize<VideoAsset>(node.OuterXml);
                            break;

                        default:
                            continue;
                    }

                    string name = node.Attributes["Name"].Value;
                    def.Name = name;
                    def.Location = new Location(file);
                    def.AssetData = data;

                    if (assetMap.ContainsKey(name))
                    {
                        kernel.IssueError(ErrorType.DuplicatedAsset, new Location(currentFile),
                            name, data.GetType().Name);
                    }
                    else
                    {
                        assetMap.Add(name, def);
                    }

                    currentNode = node; 
                }
            }
            
        }

        void settings_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            xmlError = true;
            kernel.IssueError(ErrorType.AssetXMLError, new Location(currentFile), e.Message);
        }
    }
}
