using System.Collections.Generic;
using System.Xml;

namespace FlowOptions.EggOn.ModuleCore
{
    public static class DictionaryXmlSerializer
    {
        // TODO: Error checking.

        static public Dictionary<string, string> ToDictionary(string xmlStr)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                xmlDoc.LoadXml(xmlStr);
            }
            catch (XmlException)
            {
                return new Dictionary<string, string>();
            }

            var dictionary = new Dictionary<string, string>();

            XmlElement rootNode = xmlDoc.DocumentElement;

            if (rootNode.Name == "d" && rootNode.HasChildNodes)
            {
                foreach (XmlElement fieldNode in rootNode.ChildNodes)
                {
                    if (fieldNode.Name != "f")
                        continue;

                    string key = fieldNode.Attributes["k"].Value;
                    string value = fieldNode.Attributes["v"].InnerText;

                    dictionary.Add(key, value);
                }
            }

            return dictionary;
        }

        static public XmlDocument ToXml(Dictionary<string, string> dictionary)
        {
            XmlDocument xmlDoc = new XmlDocument();

            XmlElement rootNode = xmlDoc.CreateElement("d");
            xmlDoc.AppendChild(rootNode);

            if (dictionary != null)
            {
                foreach (KeyValuePair<string, string> field in dictionary)
                {
                    XmlElement fieldNode = xmlDoc.CreateElement("f");
                    fieldNode.SetAttribute("k", field.Key);
                    fieldNode.SetAttribute("v", field.Value);
                    rootNode.AppendChild(fieldNode);
                }
            }

            return xmlDoc;
        }
    }
}
