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

namespace GH_Document_IO_Changer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //GH_IO.Serialization.GH_Archive arc = new GH_IO.Serialization.GH_Archive();
            //arc.ReadFromFile(filePath);
            //string prevString = arc.Serialize_Xml();
            ////int stringLength = arc.Serialize_Xml();
            //var root = arc.GetRootNode;
            //var definition = root.FindChunk("Definition");
            //var objects = definition.FindChunk("DefinitionObjects");
            //Console.WriteLine(objects.ChunkCount);
            //for (int i = objects.ChunkCount - 1; i > 3 ; i--)
            //{
            //    var match = objects.FindChunk("Name", i);
            //    var obj = objects.Chunks.RemoveAll(match);
            //}
            //Console.WriteLine(objects.ChunkCount);
            //arc.WriteToFile(filePath2, true, true);

            string filePath = @"C:\Users\akhaj\Desktop\temp.ghx";
            string filePath2 = @"C:\Users\akhaj\Desktop\temp2.ghx";
            string filePath3 = @"C:\temp\RemoSharp\openTempFile2.ghx";
            string filePath4 = @"C:\Users\akhaj\Desktop\singleComp.ghx";
            string filePath5 = @"C:\Masters Projects\Machina\Workshop Series\Demo.ghx";

            string fileContent = "";
            using (StreamReader sr = new StreamReader(filePath5))
            {
                fileContent += sr.ReadToEnd();
            }

            XmlDocument document = new XmlDocument();
            document.LoadXml(fileContent);
            string docObjectPaths = "";
            string objectCountPath = FindObjectCountXMLPath(document, out docObjectPaths);

            var objectCount = document.SelectSingleNode(objectCountPath).InnerText;
            int index = 1;
            var objNodes = document.SelectSingleNode(docObjectPaths);
            foreach (XmlNode objnode in objNodes)
            {

                Console.WriteLine(index + "/" + objectCount);
                string innerXML = objnode.InnerXml;
                Console.WriteLine(innerXML);
                Console.ReadKey();

                index++;
            }
            Console.Write(objectCount);
            //for (int i = 0; i < Convert.ToInt32(objectCount); i++)
            //{
                //string objectPath = docObjectPaths + "[6]";
                //var objectNode = document.SelectSingleNode(objectPath);
                //int index = Convert.ToInt32(objectNode.InnerText);
                //FindNodeObjectName(objectNode);
 
            Console.ReadKey();

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
        private static void FindNodeObjectName(XmlNode objNode)
        {
            var nodeItemCount = objNode.SelectSingleNode("chunk/items[1]/@count");
            int itemCount = Convert.ToInt32(nodeItemCount.InnerText);

            for (int i = 0; i < itemCount; i++)
            {
                Console.WriteLine(objNode.SelectSingleNode("chunk/items[1]/item").InnerText);
            }

        }
    }
}
