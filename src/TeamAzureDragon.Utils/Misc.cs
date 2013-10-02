using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TeamAzureDragon.Utils
{
    // TODO: Flags
    public enum RecursiveSerializationOption
    {
        Recurse,
        Assign,
        ForeachAssign,
        ForeachRecurse,
        Skip
    }

    public static class Misc
    {
        public static string NormalizeWhiteSpace(this string str)
        {
            return Regex.Replace(str, @"\s+", " ").Trim();
        }
        public static AppDomain CreateSandboxAppDomain()
        {
            var e = new Evidence();
            e.AddHostEvidence(new Zone(SecurityZone.Internet));
            var ps = SecurityManager.GetStandardSandbox(e);
            var security = new SecurityPermission(SecurityPermissionFlag.Execution);
            ps.AddPermission(security);
            var setup = new AppDomainSetup
            {
                ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            };

            var sandbox = AppDomain.CreateDomain("Sandbox", null, setup, ps);
            var core = sandbox.Load("System.Core, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");

            var system = sandbox.Load("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089");
            return sandbox;

        }
        //public static byte[] CompileToAssembly(string code)
        //{
        //    var engine = new ScriptEngine();
        //    var session = engine.CreateSession();

        //    using (var memoryStream = new MemoryStream())
        //    {
        //        engine.CompileSubmission(code, session).Compilation.Emit(memoryStream);
        //        var assembly = memoryStream.ToArray();
        //        return assembly;
        //    }
        //}
        // todo: command line arguments
        //public static IEnumerable<string> ExecuteInAppDomain(byte[] assembly, IEnumerable<string> stdin)
        //{

        //}
        public static object ThrowIfNull<T>(this object obj, Func<T> get) where T : Exception
        {
            if (obj == null)
                throw get();
            return obj;
        }
        public static object ThrowIfNull<T>(this object obj) where T : Exception
        {
            if (obj == null)
                throw Activator.CreateInstance<T>();
            return obj;
        }
        //public static T ParseVersioned<T>(string versionedData)
        //{
        //    var xml = VersionedString.Read(versionedData).Data;
        //    return (T)(new XmlSerializer(typeof(T))
        //        .Deserialize(new StringReader(xml)));
        //}

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
            Func<string, string, string> keyGetter = null,
            string basePath = "")
        {
            var dict = new Dictionary<string, object>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            foreach (PropertyInfo prop in from.GetType().GetProperties(flags))
            {
                var name = prop.Name;

                var path = string.IsNullOrWhiteSpace(basePath) ? name : basePath + "." + name;

                var result = customHandler == null ? RecursiveSerializationOption.Assign : customHandler(path);

                if (result == RecursiveSerializationOption.Skip)
                    continue;

                var value = prop.GetValue(from);

                var key = keyGetter == null ? name : keyGetter(path, name);

                if (value == null)
                {
                    dict[name] = null;
                    continue;
                }

                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    if (result == RecursiveSerializationOption.ForeachAssign)
                    {
                        var list = new List<object>();
                        dict[name] = list;
                        foreach (object elem in (value as IEnumerable) ?? new object[0])
                        {
                            list.Add(elem);
                        }
                        continue;
                    }
                    if (result == RecursiveSerializationOption.ForeachRecurse)
                    {
                        var list = new List<Dictionary<string, object>>();
                        dict[name] = list;
                        foreach (object elem in (value as IEnumerable) ?? new object[0])
                        {
                            list.Add(Misc.SerializeToDictionary(elem, customHandler, keyGetter, path));
                        }
                        continue;
                    }
                }
                if (result == RecursiveSerializationOption.ForeachAssign ||
                    result == RecursiveSerializationOption.ForeachRecurse)
                {
                    throw new ApplicationException();
                }


                if (result == RecursiveSerializationOption.Assign)
                {
                    dict[key] = value;
                    continue;
                }

                if (result == RecursiveSerializationOption.Recurse)
                {
                    dict[name] = SerializeToDictionary(value, customHandler, keyGetter, path);
                    continue;
                }

                throw new ApplicationException();
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
