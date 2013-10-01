using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TeamAzureDragon.Utils
{
    // TODO: Flags
    public enum RecursiveSerializationOption
    {
        Recurse,
        Assign,
        Foreach,
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


        public static Dictionary<string, object> SerializeToDictionary(object from,
            Func<string, RecursiveSerializationOption> customHandler = null,
            Func<string, string> keyGetter = null,
            string basePath = "")
        {
            var dict = new Dictionary<string, object>();

            var stack = new Stack<Tuple<string, PropertyInfo, object>>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            stack.Push(Tuple.Create(basePath, (PropertyInfo)null, from));

            while (stack.Count() > 0)
            {
                var pair = stack.Pop();

                var nameAbove = pair.Item1;
                var prop = pair.Item2;
                var owner = pair.Item3;

                if (prop == null)
                {
                    foreach (PropertyInfo ownerProp in owner.GetType().GetProperties(flags))
                    {
                        stack.Push(Tuple.Create(nameAbove, ownerProp, owner));
                    }
                    continue;
                }


                var result = customHandler == null ? RecursiveSerializationOption.Assign : customHandler(prop.Name);

                if (result == RecursiveSerializationOption.Skip)
                    continue;

                var value = prop.GetValue(from);


                var name = string.IsNullOrWhiteSpace(nameAbove) ? prop.Name : nameAbove + "." + prop.Name;

                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    if (result == RecursiveSerializationOption.Foreach)
                    {
                        var list = new List<Dictionary<string, object>>();
                        dict[name] = list;
                        foreach (object elem in (value as IEnumerable) ?? new object[0])
                        {
                            list.Add(Misc.SerializeToDictionary(elem, customHandler, keyGetter, name));
                        }
                        continue;
                    }
                }
                if (result == RecursiveSerializationOption.Foreach)
                {
                    throw new Exception();
                }

                var key = keyGetter == null ? name : keyGetter(name);

                if (result == RecursiveSerializationOption.Assign)
                {
                    dict[key] = value;
                    continue;
                }

                if (result == RecursiveSerializationOption.Recurse)
                {
                    if (value == null)
                    {
                        dict[name] = null;
                        continue;
                    }

                    dict[name] = SerializeToDictionary(value, customHandler, keyGetter, name);
                }

                throw new Exception();
            }
            //if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType))
            //{

            //}


            //if (hisProps.TryGetValue(prop.Name, out his))
            //{
            //    
            //    if (ignoreEmpty)
            //    {
            //        if (value == null)
            //            continue;
            //        if (value == "")
            //            continue;
            //        if (prop.PropertyType.IsValueType && value.Equals(Activator.CreateInstance(prop.PropertyType)))
            //            continue;
            //    }
            //    prop.SetValue(from, value, null);
            //}

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
                        if ((string)value == "")
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
