using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FireEngine.FireMLEngine.Compiler;
using FireEngine.FireMLEngine;

namespace FireEngine.FireML.Editor
{
    //生成用于iOS的二进制脚本
    class Program
    {
        static void Main(string[] args)
        {
            FileInfo assemblyFileInfo = new FileInfo(args[0]);
            DirectoryInfo fireMLDirInfo = assemblyFileInfo.Directory;
            DirectoryInfo contentDirInfo = fireMLDirInfo.Parent;

            List<string> plotFileList = new List<string>();
            List<string> assetFileList = new List<string>();
            foreach (FileInfo fileInfo in fireMLDirInfo.GetFiles("*.*", SearchOption.AllDirectories))
            {
                string ext = fileInfo.Extension;
                if (ext == ".fmlplot")
                {
                    plotFileList.Add(fileInfo.FullName);
                }
                else if (ext == ".fmlasset")
                {
                    assetFileList.Add(fileInfo.FullName);
                }
            }

            string xsdDirPath = fireMLDirInfo.FullName + "\\" + "XSD";

            //FireEngine.XNAContent.ContentManager contentManager = new FireEngine.XNAContent.ContentManager(contentDirInfo.FullName);
            CompilerKernel kernel = new CompilerKernel(plotFileList.ToArray(), assetFileList.ToArray(), xsdDirPath, null /*contentManager*/);
            FireMLRoot result = kernel.CompileFireML();

            Error[] errors = kernel.CheckPoint();

            foreach (Error e in errors)
            {
                Console.WriteLine("{0}\n{1},{2}\n{3}", e.Location.FileName, e.Location.Line, e.Location.Column, e.Message);
                Console.WriteLine();
            }

            if (errors.Length > 0)
            {
                Environment.Exit(-1);
                return;
            }

            
  
        }

        
    }
}
