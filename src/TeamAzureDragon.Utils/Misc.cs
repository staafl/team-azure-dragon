using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TeamAzureDragon.Utils
{
    public enum RecursiveSerializationOption
    {
        Recurse,
        Assign,
        Skip
    }

    public static class Misc
    {
        public static T ParseVersioned<T>(string versionedData)
        {
            var xml = VersionedString.Read(versionedData).Data;
            return (T)(new XmlSerializer(typeof(T))
                .Deserialize(new StringReader(xml)));
        }

        public static string Abbreviate(this string text, int maxLength)
        {
            if (text.Length <= maxLength)
                return text;
            if (maxLength < 3)
                throw new ArgumentException();

            text = text.Substring(0, maxLength - 3) + "...";

            return text;
        }


        public static Dictionary<string, string> SerializeToDictionary(object source,
            Func<string, RecursiveSerializationOption> customHandler)
        {
            var dict = new Dictionary<string, string>();

            var stack = new Stack<Tuple<string, object>>();



            return dict;

        }

        public static void Fill(object me, object from, bool ignoreEmpty = false, params string[] ignoreHeaders)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            PropertyInfo[] myProps = me.GetType().GetProperties(flags);
            var hisProps = from.GetType().GetProperties(flags).ToDictionary(p => p.Name, p => p);
            foreach (PropertyInfo mine in myProps)
            {
                PropertyInfo his;
                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(mine.PropertyType) &&
                    typeof(string) != mine.PropertyType)
                    continue;
                if (ignoreHeaders.Contains(mine.Name))
                    continue;
                if (hisProps.TryGetValue(mine.Name, out his))
                {
                    var value = his.GetValue(from);
                    if (ignoreEmpty)
                    {
                        if (value == null)
                            continue;
                        if (value == "")
                            continue;
                        if (mine.PropertyType.IsValueType && value.Equals(Activator.CreateInstance(mine.PropertyType)))
                            continue;
                    }
                    mine.SetValue(me, value, null);
                }
            }
        }
    }
}
