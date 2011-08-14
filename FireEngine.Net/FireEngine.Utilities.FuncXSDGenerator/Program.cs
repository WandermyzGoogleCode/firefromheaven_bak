using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace FireEngine.Utilities.FuncXSDGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            const string XMLNS = FireEngine.Library.FireEngineConstant.XSD_XMLNS;
            
            Dictionary<string, object> funcNameSet = new Dictionary<string, object>();
            bool success = true;

            string outputPath = args[0];

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

            XmlNode choiceNode = output.GetElementsByTagName("xs:choice")[0];
            choiceNode.RemoveAll();

            string dir = args[1];
            string[] files = Directory.GetFiles(dir, "*.fmlplot", SearchOption.AllDirectories);

            int funcCounter = 0;

            //for (int i = 1; i < args.Length; i++)
            foreach(string inputPath in files)
            {
                //string inputPath = args[i];
                XmlDocument input = new XmlDocument();
                try
                {
                    input.Load(inputPath);
                }
                catch (IOException e)
                {
                    Console.WriteLine("读取文件{0}时出错，已跳过该文件({1})", inputPath, e.Message);
                    success = false;
                    continue;
                }
                catch (XmlException e)
                {
                    Console.WriteLine("解析文件{0}时出错，文件中可能存在语法错误，已跳过该文件({1})", inputPath, e.Message);
                    success = false;
                    continue;
                }

                XmlNamespaceManager nsmgr = new XmlNamespaceManager(input.NameTable);
                nsmgr.AddNamespace("fm", FireEngine.Library.FireEngineConstant.FIREML_XMLNS);


                XmlNodeList funcList = input.GetElementsByTagName("Function");

                foreach (XmlNode funcNode in funcList)
                {
                    //检查重复
                    string funcName = funcNode.Attributes["name"].Value;
                    if (funcNameSet.ContainsKey(funcName))
                    {
                        Console.WriteLine("函数名\"{0}\"被重复定义。FireML不支持函数重载。", funcName);
                        success = false;
                        continue;
                    }

                    funcNameSet.Add(funcName, null);

                    //函数元素
                    XmlElement funcSchemaElem = output.CreateElement("xs:element", XMLNS);

                    XmlAttribute nameAtt = output.CreateAttribute("name");
                    nameAtt.Value = funcNode.Attributes["name"].Value;
                    funcSchemaElem.Attributes.Append(nameAtt);

                    XmlAttribute nilAtt = output.CreateAttribute("nillable");
                    nilAtt.Value = "true";
                    funcSchemaElem.Attributes.Append(nilAtt);

                    if (funcNode.Attributes["doc"] != null)
                    {
                        XmlElement annotationElem = output.CreateElement("xs:annotation", XMLNS);
                        XmlElement docElem = output.CreateElement("xs:documentation", XMLNS);
                        docElem.InnerText = funcNode.Attributes["doc"].Value;
                        annotationElem.AppendChild(docElem);
                        funcSchemaElem.AppendChild(annotationElem);
                    }

                    //参数表
                    XmlElement complexTypeElem = output.CreateElement("xs:complexType", XMLNS);

                    XmlNodeList paramNodeList = funcNode.SelectNodes("fm:para", nsmgr);
                    foreach (XmlNode paramNode in paramNodeList)
                    {
                        XmlElement paramAttElem = output.CreateElement("xs:attribute", XMLNS);
                        XmlAttribute paramNameAtt = output.CreateAttribute("name");
                        paramNameAtt.Value = paramNode.Attributes["name"].Value.Remove(0, 1); //检查$符号！
                        paramAttElem.Attributes.Append(paramNameAtt);

                        XmlAttribute paramTypeAtt = output.CreateAttribute("type");
                        paramTypeAtt.Value = "Expression";
                        if (paramNode.Attributes["type"] != null)
                        {
                            switch (paramNode.Attributes["type"].Value)
                            {
                                case "Auto":
                                case "String":
                                default:
                                    paramTypeAtt.Value = "xs:string";
                                    break;

                                case "Int":
                                    paramTypeAtt.Value = "xs:int";
                                    break;

                                case "Float":
                                    paramTypeAtt.Value = "xs:double";
                                    break;

                                case "Bool":
                                    paramTypeAtt.Value = "xs:boolean";
                                    break;

                                case "Expression":
                                    paramTypeAtt.Value = "Expression";
                                    break;
                            }
                        }
                        else
                        {
                            paramTypeAtt.Value = "xs:string";
                        }

                        paramAttElem.Attributes.Append(paramTypeAtt);




                        if (paramNode.Attributes["doc"] != null)
                        {
                            XmlElement annotationElem = output.CreateElement("xs:annotation", XMLNS);
                            XmlElement docElem = output.CreateElement("xs:documentation", XMLNS);
                            docElem.InnerText = paramNode.Attributes["doc"].Value;
                            annotationElem.AppendChild(docElem);
                            paramAttElem.AppendChild(annotationElem);
                        }

                        if (paramNode.Attributes["default"] != null)
                        {
                            XmlAttribute paramDefaultAtt = output.CreateAttribute("default");
                            paramDefaultAtt.Value = paramNode.Attributes["default"].Value;
                            paramAttElem.Attributes.Append(paramDefaultAtt);
                        }

                        complexTypeElem.AppendChild(paramAttElem);
                    }

                    //参数字串
                    List<string> paramStrList = new List<string>();
                    XmlNodeList paramStrNodeList = funcNode.SelectNodes("fm:parastr", nsmgr);
                    foreach (XmlNode paramStrNode in paramStrNodeList)
                    {
                        paramStrList.Add(paramStrNode.InnerText);
                    }
                    if (paramStrList.Count > 0)
                    {
                        XmlElement parAttElem = output.CreateElement("xs:attribute", XMLNS);
                        XmlAttribute parNameAtt = output.CreateAttribute("name");
                        parNameAtt.Value = "par";
                        parAttElem.Attributes.Append(parNameAtt);
                        XmlAttribute parTypeAtt = output.CreateAttribute("type");
                        parTypeAtt.Value = "ActualParameters";
                        parAttElem.Attributes.Append(parTypeAtt);
                        XmlElement annotationElem = output.CreateElement("xs:annotation", XMLNS);
                        XmlElement docElem = output.CreateElement("xs:documentation", XMLNS);
                        docElem.InnerText = "参数字串：" + string.Join("; ", paramStrList.ToArray());
                        annotationElem.AppendChild(docElem);
                        parAttElem.AppendChild(annotationElem);

                        complexTypeElem.AppendChild(parAttElem);
                    }

                    //return
                    XmlElement returnAttElem = output.CreateElement("xs:attribute", XMLNS);
                    XmlAttribute returnNameAtt = output.CreateAttribute("name");
                    returnNameAtt.Value = "return";
                    returnAttElem.Attributes.Append(returnNameAtt);
                    XmlAttribute returnTypeAtt = output.CreateAttribute("type");
                    returnTypeAtt.Value = "VarRef";
                    returnAttElem.Attributes.Append(returnTypeAtt);
                    XmlElement returnAnnotationElem = output.CreateElement("xs:annotation", XMLNS);
                    XmlElement returnDocElem = output.CreateElement("xs:documentation", XMLNS);
                    returnDocElem.InnerText = "将返回值保存到某个变量中";
                    returnAnnotationElem.AppendChild(returnDocElem);
                    returnAttElem.AppendChild(returnAnnotationElem);

                    complexTypeElem.AppendChild(returnAttElem);

                    funcSchemaElem.AppendChild(complexTypeElem);

                    choiceNode.AppendChild(funcSchemaElem);

                    funcCounter++;
                }
            }   //for end

            output.Save(outputPath);

            if (!success)
            {
                Console.WriteLine("解析FireML时出现错误");
                Environment.Exit(-1);
            }
            else
            {
                Console.WriteLine("共生成{0}个函数的XSD", funcCounter);
            }

        }   //Main End
    }
}
