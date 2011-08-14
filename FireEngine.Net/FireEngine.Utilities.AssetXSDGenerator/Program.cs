using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace FireEngine.Utilities.AssetXSDGenerator
{
    class Program
    {
        const string XMLNS = FireEngine.Library.FireEngineConstant.XSD_XMLNS;

        static void Main(string[] args)
        {
            bool success = true;

            string outputPath = args[0];
            string dir = args[1];
            string[] files = Directory.GetFiles(dir, "*.fmlasset", SearchOption.AllDirectories);

            XmlDocument output = new XmlDocument();
            try
            {
                output.Load(outputPath);
            }
            catch (Exception e)
            {
                Console.WriteLine("读取原始XSD文件时出错(" + e.Message + ")");
                success = false;
                Environment.Exit(-1);
            }

            success &= generateXSDNode(output, files, "Music", "MusicAssetRef", "src");
            success &= generateXSDNode(output, files, "Actor", "ActorAssetRef", "src");
            success &= generateXSDNode(output, files, "CG", "CGAssetRef", "src");
            success &= generateXSDNode(output, files, "Video", "VideoAssetRef", "src");

            if (!success)
            {
                Console.WriteLine("生成预定义资源的XSD时出现错误");
                Environment.Exit(-1);
            }
            else
            {
                Console.WriteLine("已成功生成所有预定义资源的XSD");
            }

            output.Save(outputPath);
        }

        private static bool generateXSDNode(XmlDocument output, string[] files, string assetNodeName, string assetRefTypeName, string srcAttName)
        {
            bool success = true; ;
            Dictionary<string, object> assetNameSet = new Dictionary<string, object>();

            string xpath = "xs:schema/xs:simpleType[@name='" + assetRefTypeName + "']/xs:union/xs:simpleType/xs:restriction";
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(output.NameTable);
            nsmgr.AddNamespace("xs", XMLNS);
            XmlNode outRestrictionNode = output.SelectSingleNode(xpath, nsmgr);

            if (outRestrictionNode == null)
            {
                Console.WriteLine("读取原始XSD文件时出错, 找不到结点：" + xpath);
                return false;
            }

            outRestrictionNode.RemoveAll();
            XmlAttribute outRestrictionBaseAtt = output.CreateAttribute("base");
            outRestrictionBaseAtt.Value = "AssetRef";
            outRestrictionNode.Attributes.Append(outRestrictionBaseAtt);


            int assetCounter = 0;

            foreach (string inputPath in files)
            {
                XmlDocument input = new XmlDocument();
                try
                {
                    input.Load(inputPath);
                }
                catch (IOException e)
                {
                    Console.WriteLine("读取文件{0}时出错，已跳过该文件({1})", inputPath, e.Message);
                    return false;
                }
                catch (XmlException e)
                {
                    Console.WriteLine("解析文件{0}时出错，文件中可能存在语法错误，已跳过该文件({1})", inputPath, e.Message);
                    return false;
                }


                XmlNodeList inAssetList = input.GetElementsByTagName(assetNodeName);

                foreach (XmlNode inAssetNode in inAssetList)
                {
                    string assetName = inAssetNode.Attributes["Name"].Value;
                    if (assetNameSet.ContainsKey(assetName))
                    {
                        Console.WriteLine("资源名\"{0}\"被重复定义。", assetName);
                        success = false;
                        continue;
                    }

                    assetNameSet.Add(assetName, null);

                    XmlElement outEnumElem = output.CreateElement("xs:enumeration", XMLNS);
                    XmlAttribute outEnumAttValue = output.CreateAttribute("value");
                    outEnumAttValue.Value = assetName;
                    outEnumElem.Attributes.Append(outEnumAttValue);

                    if (srcAttName != null)
                    {
                        XmlElement annotationElem = output.CreateElement("xs:annotation", XMLNS);
                        XmlElement docElem = output.CreateElement("xs:documentation", XMLNS);
                        docElem.InnerText = inAssetNode.Attributes["src"].Value;
                        annotationElem.AppendChild(docElem);
                        outEnumElem.AppendChild(annotationElem);
                    }

                    outRestrictionNode.AppendChild(outEnumElem);

                    assetCounter++;
                }
            }

            if (success)
            {
                Console.WriteLine("为{0}生成了{1}个预定义资源的XSD", assetNodeName, assetCounter);
            }

            return success;
        }
    }
}
