using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FlowOptions.EggOn.ModuleCore
{

    public class MessageSerializer<T> {
        public static T Deserialize(string type) {
            if (type == null) {
                return default(T);
            }
            
            var serializer = new XmlSerializer(typeof(T));

            var result = (T)serializer.Deserialize(new StringReader(type));

            return result;
        }

        public static string Serialize(T type) {
            var serializer = new XmlSerializer(typeof(T));
            string originalMessage;

            using (var ms = new MemoryStream()) {
                serializer.Serialize(ms, type);
                ms.Position = 0;
                var document = new XmlDocument();
                document.Load(ms);

                originalMessage = document.OuterXml;
            }

            return originalMessage;
        }
    }
}
