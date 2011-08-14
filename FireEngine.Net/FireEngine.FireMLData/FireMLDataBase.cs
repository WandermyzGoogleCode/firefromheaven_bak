using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using FireEngine.Library;
using System.Text.RegularExpressions;

namespace FireEngine.FireMLData
{
    [Serializable]
    public abstract class FireMLDataBase
    {
        private const string XML_HEADER = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>";
        private static readonly Regex headerRegex = new Regex(@"^(\s*<)[^>\s]+(\s*)",RegexOptions.Compiled);
        private static readonly Regex footerRegex = new Regex(@"(<\/)[^>\s]+(>\s*)$",RegexOptions.Compiled);

        public static T Deserialize<T>(string xmlStr) where T : FireMLDataBase 
        {
            object[] attrArray = typeof(T).GetCustomAttributes(typeof(XmlTypeAttribute), false);
            string elementName;
            XmlTypeAttribute xta;
            if (attrArray != null && attrArray.Length > 0
                && (xta = attrArray[0] as XmlTypeAttribute).TypeName.Length > 0)
            {
                elementName = xta.TypeName;
            }
            else
            {
                elementName = typeof(T).Name;
            }

            xmlStr = xmlStr.Replace(" xmlns=\"" + FireEngineConstant.FIREML_XMLNS + "\"", "");
            //TODO: 考虑用正则替换
            xmlStr = headerRegex.Replace(xmlStr, "$1" + elementName + "$2");
            xmlStr = footerRegex.Replace(xmlStr, "$1" + elementName + "$2");

            /*
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xmlStr);

            //doc.InnerXml = xmlStr;
            XmlNode root = doc.FirstChild;
            doc.RemoveAll();
            XmlNode newRoot = doc.CreateElement(elementName);

            while (root.Attributes.Count > 0)
            {
                newRoot.Attributes.Append(root.Attributes[0]);
            }
            newRoot.InnerXml = root.InnerXml;
            //removeNodeXmlns(newRoot);

            doc.AppendChild(newRoot);

            MemoryStream ms = new MemoryStream();
            doc.Save(ms);
            ms.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            */

            TextReader reader = new StringReader(xmlStr);
            XmlSerializer xs = new XmlSerializer(typeof(T));
            T r = xs.Deserialize(reader) as T;
            return r;
        }

        /*
        private static void removeNodeXmlns(XmlNode node)
        {
            foreach (XmlNode child in node.ChildNodes)
                removeNodeXmlns(child);

            XmlAttribute xmlnsAttr;
            if (node.Attributes!=null && (xmlnsAttr = node.Attributes["xmlns"]) != null)
                node.Attributes.Remove(xmlnsAttr);
        }
        */

        /// <summary>
        /// 检查Data中的引用（SubPlot）是否存在
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public virtual bool CheckReference(IDataCheckHelper helper)
        {
            return true;
        }

        /// <summary>
        /// 检查Data中的内容文件及Asset是否存在
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public virtual bool CheckContent(IDataCheckHelper helper)
        {
            return true;
        }
    }
}
