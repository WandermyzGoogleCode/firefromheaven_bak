using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace FireEngine.Utilities.DataXSDGenerator
{
    class Program
    {
        const string XMLNS = FireEngine.Library.FireEngineConstant.XSD_XMLNS;

        /// <summary>
        /// 
        /// </summary>
    /// <param name="args">inputPath outputPath</param>
        static void Main(string[] args)
        {
            string inputPath = args[0];
            //string outputPath = args[1];

            XmlDocument input = new XmlDocument();
            try
            {
                input.Load(inputPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("读取原始XSD文件时出错(" + e.Message + ")");
                Environment.Exit(-1);
            }

            //XmlDocument output = new XmlDocument();
            //try
            //{
            //    output.Load(outputPath);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine("读取目标XSD文件时出错(" + e.Message + ")");
            //    Environment.Exit(-1);
            //}

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(input.NameTable);
            nsmgr.AddNamespace("xs", XMLNS);         

            XmlNode inputRoot = input.SelectSingleNode("xs:schema", nsmgr);
            XmlComment warningComment = input.CreateComment("请勿修改此文件，它会在编译工程时被重写！");
            input.InsertBefore(warningComment, inputRoot);
            //XmlNode outputRoot = output.SelectSingleNode("xs:schema", nsmgr);

            foreach (XmlNode elementNode in inputRoot.SelectNodes("xs:element", nsmgr))
            {
                inputRoot.RemoveChild(elementNode);
            }

            foreach (XmlNode typeNode in inputRoot.SelectNodes("xs:complexType", nsmgr))
            {
                XmlComment c = input.CreateComment(Environment.NewLine + typeNode.Attributes["name"].Value + Environment.NewLine);
                inputRoot.InsertBefore(c, typeNode);
            }

            //outputRoot.InnerXml = inputRoot.InnerXml;
            //output.Save(outputPath);
            input.Save(inputPath);

            Console.WriteLine("XSD文件已生成，请手动将其内容添加到FireML的XSD中。");
        }
    }
}
