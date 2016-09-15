using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace System
{
    public static class StringExtension
    {
        public static string ReplaceRx(this string text, string pattern, string newString)
        {
            return Regex.Replace(text, pattern, newString, RegexOptions.IgnoreCase);
        }


        public static T XmlDeserializeFromString<T>(this string objectData) 
            where T: new()
        {
            var serializer = new XmlSerializer(typeof(T));
            object instance;

            using (TextReader reader = new StringReader(objectData))
            {
                instance = (T)serializer.Deserialize(reader);
            }

            return (T)instance;
        }
    }
}
