using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using System.IO;
using Rhino;
using Rhino.Geometry;
using GH_IO;
using System.Xml;
using System.Threading;
using System.Windows.Forms;

namespace GH_Document_IO_Changer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string RemoSharpLibraryGUID = "a1d80423-e6e0-49f5-8514-de158ae1193a";
            string RemoSharpNickName = "RemoSharp";

            string filePath = @"C:\Users\akhaj\Desktop\temp.ghx";
            string filePath2 = @"C:\Users\akhaj\Desktop\temp2.ghx";
            string filePath3 = @"C:\temp\RemoSharp\openTempFile2.ghx";
            string filePath4 = @"C:\Users\akhaj\Desktop\singleComp.ghx";
            string filePath5 = @"C:\Masters Projects\Machina\Workshop Series\Demo.ghx";
            string filePath6 = @"C:\Masters Projects\Machina\Workshop Series\Demo2.ghx";

            for (int j = 0; j < 50; j++)
            {
                string fileContent = "";
                using (StreamReader sr = new StreamReader(filePath5))
                {
                    fileContent += sr.ReadToEnd();
                }

                XmlDocument document = new XmlDocument();
                document.LoadXml(fileContent);
                string docObjectPaths = "";
                string objectCountPath = FindObjectCountXMLPath(document, out docObjectPaths);

                var objectCount = Convert.ToInt32(document.SelectSingleNode(objectCountPath).InnerText);

                XmlNode objNodes = document.SelectSingleNode(docObjectPaths);

            
                for (int i = objectCount - 1; i > -1; i--)
                {

                    //Console.WriteLine(i);
                    XmlNode node = objNodes.ChildNodes[i];
                    var attributeNode = node.SelectSingleNode("chunks/chunk/chunks");
                    var nameNode = node.SelectSingleNode("chunks/chunk/items");
                    var libNode = node.SelectSingleNode("items");

                    bool outOfBounds = CleanOutOfBoundComponents(attributeNode, (j * 50), (j * 50) + 300, 0, 3000);
                    bool isNickNameRemoSharp = IsComponentNickNameRemoSharp(nameNode, RemoSharpNickName);
                    bool isOfRemoSharp = IsComponentOfRemoSharp(libNode, RemoSharpLibraryGUID);

                    if (outOfBounds || isNickNameRemoSharp || isOfRemoSharp) objNodes.RemoveChild(node);

                }

                int newObjectCount = document.SelectSingleNode(docObjectPaths).ChildNodes.Count;

                for (int i = newObjectCount - 1; i > -1; i--)
                {
                    XmlNode node = objNodes.ChildNodes[i];
                    node.Attributes["index"].InnerText = i.ToString();
                }

                int newCount = newObjectCount;

                string newCountStr = newCount.ToString();
                document.SelectSingleNode(objectCountPath).InnerText = newCountStr;
                objNodes.Attributes["count"].InnerText = newCountStr;

                while (true)
                {
                    try
                    {
                        document.Save(filePath6);
                        //Thread.Sleep(100);
                        break;
                    }
                    catch { }
                }

            }

            }
        private static string FindObjectCountXMLPath(XmlDocument document, out string docObjectsPath) 
        {
            int index = 1;
            string path = "";
            for (int i = 0; i < 10; i++)
            {
                path = "Archive/chunks[1]/chunk[1]/chunks[1]/chunk[" + index+"]/items/item/@name";
                if (document.SelectSingleNode(path).InnerText.Equals("ObjectCount")) break;
                index++;
            }
            docObjectsPath = "Archive/chunks[1]/chunk[1]/chunks[1]/chunk[" + index + "]/chunks";
            return "Archive/chunks[1]/chunk[1]/chunks[1]/chunk[" + index + "]/items/item";
        }

        private static bool CleanOutOfBoundComponents(XmlNode attributeNode, double minX, double maxX, double minY,double maxY)
        {
            bool deleteThisNode = true;

            foreach (XmlNode subNode in attributeNode.ChildNodes)
            {
                string subNodeXml = subNode.InnerXml;
                string subNodeName = subNode.Attributes["name"].InnerText;

                if (subNodeName == "Attributes")
                {
                    if (string.IsNullOrEmpty(subNodeXml)) continue;
                    var subNodeXML = subNode.SelectSingleNode("items").ChildNodes;
                    foreach (XmlNode subSubNode in subNodeXML)
                    {
                        if (subSubNode.Attributes["name"].InnerText == "Pivot")
                        {
                            double pivotX = Convert.ToDouble(subSubNode.SelectSingleNode("X").InnerText);
                            double pivotY = Convert.ToDouble(subSubNode.SelectSingleNode("Y").InnerText);
                            bool isInside = pivotX > minX &&
                                            pivotX < maxX &&
                                            pivotY > minY &&
                                            pivotY < maxY;

                            if (isInside) deleteThisNode = false;

                            //Console.ReadKey();
                        }

                    }
                    break;
                }

            }

            return deleteThisNode;
        }

        private static bool IsComponentNickNameRemoSharp(XmlNode attributeNode, string nickName)
        {
            //string subNodeXmlText = attributeNode.InnerText;
            bool deleteThisComponent = false;

            foreach (XmlNode node in attributeNode.ChildNodes)
            {
                if (string.IsNullOrEmpty(node.InnerXml) || string.IsNullOrEmpty(node.InnerText)) continue;
                string objectNickNameAttribute = node.Attributes["name"].InnerText;
                string objectNickName = node.InnerXml;
                if (objectNickNameAttribute.Equals("NickName") &&
                   objectNickName.Contains(nickName))
                {
                    deleteThisComponent = true;
                    break;
                }
            }
            //if (subNodeXmlText.Contains(libraryGUID) || subNodeXmlText.Contains(nickName)) return true;
            return deleteThisComponent;
        }

        private static bool IsComponentOfRemoSharp(XmlNode libNode, string remoSharpLibraryGUID)
        {
            bool deleteThisComponent = false;

            foreach (XmlNode node in libNode.ChildNodes)
            {
                if (string.IsNullOrEmpty(node.InnerXml) || string.IsNullOrEmpty(node.InnerText)) continue;
                string objectNickNameAttribute = node.Attributes["name"].InnerText;
                string objectNickName = node.InnerXml;
                if (objectNickNameAttribute.Equals("Lib") &&
                   objectNickName.Contains(remoSharpLibraryGUID))
                {
                    deleteThisComponent = true;
                    break;
                }
            }

            return deleteThisComponent;
        }

        private static void CopyToClipboard(string text)
        {
            Thread thread = new Thread(() => Clipboard.SetText(text));
            thread.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            thread.Start();
            thread.Join();

        }

    }
}
