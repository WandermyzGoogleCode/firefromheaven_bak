using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.IO;
using System.Text;
using FireEngine.FireMLEngine.Compiler;
using FireEngine.FireMLEngine.AST;
using FireEngine.FireMLEngine;

// TODO: replace this with the type you want to import.
using TImport = FireEngine.FireMLEngine.FireMLRoot;

namespace FireEngine.XNAContentPipeline
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to import a file from disk into the specified type, TImport.
    /// 
    /// This should be part of a Content Pipeline Extension Library project.
    /// 
    /// TODO: change the ContentImporter attribute to specify the correct file
    /// extension, display name, and default processor for this importer.
    /// </summary>
    [ContentImporter(".fml", DisplayName = "FireML Assembly - Fire Engine", DefaultProcessor = "No Processing Required")]
    public class FireMLAssemblyImporter : ContentImporter<TImport>
    {
        public override TImport Import(string filename, ContentImporterContext context)
        {
            FileInfo assemblyFileInfo = new FileInfo(filename);
            DirectoryInfo fireMLDirInfo = assemblyFileInfo.Directory;
            DirectoryInfo contentDirInfo = fireMLDirInfo.Parent;

            /*
            DirectoryInfo plotDirInfo = new DirectoryInfo(fireMLDirInfo.FullName + "\\Plot");
            if (!plotDirInfo.Exists)
            {
                throw new InvalidContentException("剧情脚本文件夹Plot不存在！");
            }

            List<string> plotFileList = new List<string>();
            foreach (FileInfo plotFile in plotDirInfo.GetFiles("*.xml"))
            {
                plotFileList.Add(plotFile.FullName);
            }

            DirectoryInfo assetDirInfo = new DirectoryInfo(fireMLDirInfo.FullName + "\\Asset");
            List<string> assetFileList = new List<string>();
            if (assetDirInfo.Exists)
            {
                foreach (FileInfo assetFile in assetDirInfo.GetFiles("*.xml"))
                {
                    assetFileList.Add(assetFile.FullName);
                }
            }*/

            List<string> plotFileList = new List<string>();
            List<string> assetFileList = new List<string>();
            foreach(FileInfo fileInfo in fireMLDirInfo.GetFiles("*.*", SearchOption.AllDirectories))
            {
                string ext = fileInfo.Extension;
                if(ext == ".fmlplot")
                {
                    plotFileList.Add(fileInfo.FullName);
                }
                else if(ext == ".fmlasset")
                {
                    assetFileList.Add(fileInfo.FullName);
                }
            }

            string xsdDirPath = fireMLDirInfo.FullName + "\\" + "XSD";

            CompilerKernel kernel = new CompilerKernel(plotFileList.ToArray(), assetFileList.ToArray(), xsdDirPath,
                new FireEngine.XNAContent.ContentManager(contentDirInfo.FullName));
            
            FireMLRoot root = kernel.CompileFireML();
            Error[] errors = kernel.CheckPoint();

            if (errors.Length > 0)
            {
                StringBuilder msg = new StringBuilder();
                foreach (Error error in errors)
                {
                    msg.AppendLine(error.Message + "(" + error.Location.ToString() + ")");
                }

                ContentIdentity ci = new ContentIdentity(errors[0].Location.FileName);
                ci.FragmentIdentifier = errors[0].Location.Line + ", " + errors[0].Location.Column;

                throw new InvalidContentException(msg.ToString(), ci);
            }

            return root;
        }
    }
}
