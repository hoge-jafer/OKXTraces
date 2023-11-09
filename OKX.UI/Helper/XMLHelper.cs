using OKX.UI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OKX.UI.Helper
{
    public static class XMLHelper
    {
        public static void XMLCreate()
        {
            string xmlpath = System.AppDomain.CurrentDomain.BaseDirectory + "UserConfig\\UserData.xml";
            string xmlDirectorypath = System.AppDomain.CurrentDomain.BaseDirectory + "UserConfig\\";

            if (!File.Exists(xmlpath))
            {
                Directory.CreateDirectory(xmlDirectorypath);
                XmlDocument xmldoc = new XmlDocument();
                //加入XML的声明段落
                XmlNode xmlnode = xmldoc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                xmldoc.AppendChild(xmlnode);
                //加入一个根元素
                XmlElement xmlelem = xmldoc.CreateElement("", "Data", "");
                xmldoc.AppendChild(xmlelem);
                try
                {
                    xmldoc.Save(xmlpath);
                }
                catch (Exception e)
                {
                    //显示错误信息
                    Console.WriteLine(e.Message);
                }
            }
        }

        public static List<UserConfigModel> XMLReadAll()
        {
            List<UserConfigModel> list = new List<UserConfigModel>();
            string xmlpath = System.AppDomain.CurrentDomain.BaseDirectory + "UserConfig\\UserData.xml";
            XElement xe = XElement.Load(xmlpath);
            IEnumerable<XElement> elements = from ele in xe.Elements("User") select ele;
            list = ShowInfoByElements(elements);
            return list;
        }

        public static List<UserConfigModel> ShowInfoByElements(IEnumerable<XElement> elements)
        {
            List<UserConfigModel> modelList = new List<UserConfigModel>();
            foreach (var ele in elements)
            {
                UserConfigModel model = new UserConfigModel();
                model.ApiKey = ele.Element("ApiKey").Value;
                model.Secret = ele.Element("Secret").Value;
                model.PassPhrase = ele.Element("PassPhrase").Value;
                modelList.Add(model);
            }
            return modelList;
        }

        public static void XMLWrite(string apiKey, string secret, string passphrase)
        {
            string xmlpath = System.AppDomain.CurrentDomain.BaseDirectory + "UserConfig\\UserData.xml";
            //UserConfig
            //XMLCreate();

            //获取根节点对象
            //XDocument document = new XDocument();
            //XElement root = new XElement("School");
            //XElement book = new XElement("BOOK");
            //book.SetElementValue("UserName", "高等数学");
            //book.SetElementValue("PassWord", "大学英语");
            //book.SetElementValue("Permissions", 1);
            //root.Add(book);
            //root.Save(xmlpath);

            XElement loadxmlfile = XElement.Load(xmlpath);
            XElement xmlrecord = new XElement(
                      new XElement("User",
                      //new XAttribute("Type", "选修课"),
                      //new XAttribute("ISBN", "7-111-19149-1"),
                      new XElement("ApiKey", apiKey),
                      new XElement("Secret", secret),
                      new XElement("PassPhrase", passphrase)));
            loadxmlfile.Add(xmlrecord);
            loadxmlfile.Save(xmlpath);

            //XmlDocument doc = new XmlDocument();
            //doc.Load(@"..\..\Book.xml");
            //XmlNode root = doc.SelectSingleNode("bookstore");
            //XmlElement xelKey = doc.CreateElement("book");
            //XmlAttribute xelType = doc.CreateAttribute("Type");
            //xelType.InnerText = "adfdsf";
            //xelKey.SetAttributeNode(xelType);
            //XmlElement xelAuthor = doc.CreateElement("author");
            //xelAuthor.InnerText = "dfdsa";
            //xelKey.AppendChild(xelAuthor);
            //root.AppendChild(xelKey);
            //doc.Save(@"..\..\Book.xml");
        }
    }
}
