using System;
using System.Collections.Generic;
using System.Text;
using FireEngine.FireMLEngine;
using FireEngine.FireMLEngine.AST;
using FireEngine.FireMLEngine.Compiler;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FireEngine.Test.ConsoleCompiler
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            int plotNum = int.Parse(args[0]);
            int assetNum = int.Parse(args[1]);
            string contentPath = args[2];
            const int BEGIN_ARG = 3;

            List<string> plotList = new List<string>();
            List<string> assetList = new List<string>();
            for (int i = BEGIN_ARG; i < BEGIN_ARG + plotNum; i++)
            {
                plotList.Add(args[i]);
            }
            for (int i = BEGIN_ARG + plotNum; i < BEGIN_ARG + plotNum + assetNum; i++)
            {
                assetList.Add(args[i]);
            }*/

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

            FireEngine.XNAContent.ContentManager contentManager = new FireEngine.XNAContent.ContentManager(contentDirInfo.FullName);
            CompilerKernel kernel = new CompilerKernel(plotFileList.ToArray(), assetFileList.ToArray(), xsdDirPath, contentManager);
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

            BinaryFormatter bf = new BinaryFormatter();
            FileStream writeStream = new FileStream("compiled.bin", FileMode.Create);
            bf.Serialize(writeStream,result);
            writeStream.Close();

            Console.WriteLine("编译完成");

            FileStream readStream = new FileStream("compiled.bin", FileMode.Open);
            FireMLRoot deserialized = bf.Deserialize(readStream) as FireMLRoot;
        }
    }
}
