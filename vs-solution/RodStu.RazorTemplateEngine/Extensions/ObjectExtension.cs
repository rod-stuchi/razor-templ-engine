using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace System
{
    public static class ObjectExtension
    {
        public static string XmlSerializeToString(this object objInstance)
        {
            if (objInstance == null)
                return String.Empty;

            StringBuilder xml = new StringBuilder();
            Type t = objInstance.GetType();

            if (t.IsSerializable)
            {
                XmlSerializer serialize = new XmlSerializer(t);
                using (XmlWriter writer = XmlTextWriter.Create(xml,
                    new XmlWriterSettings
                    {
                        OmitXmlDeclaration = true,
                        Indent = true
                    }))
                {
                    serialize.Serialize(writer, objInstance);
                }
                return xml.ToString();
            }
            else
                return "Class precisa estar decorada com [SerializableAttribute]";

        }
    }
}
