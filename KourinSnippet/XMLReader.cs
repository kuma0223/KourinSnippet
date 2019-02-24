using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bank
{
    class XMLReader
    {
        static public void writeXML(string filePath,Object it,Type type)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);
            using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                var ns =  new System.Xml.Serialization.XmlSerializerNamespaces();
                ns.Add(String.Empty, string.Empty);

                serializer.Serialize(fs, it, ns);
            }
        }

        static public object readXML(string filePath, Type type)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(type);
            using (System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
            {
                object ret = serializer.Deserialize(fs);
                return ret;
            }
        }
    }
}
