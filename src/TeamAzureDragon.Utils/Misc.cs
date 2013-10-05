using Roslyn.Scripting;
using Roslyn.Scripting.CSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
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
using TeamAzureDragon.Utils.Attributes;

namespace TeamAzureDragon.Utils
{
    public static partial class Misc
    {
        public static bool GraphContainsCycles<T>(T root, Func<T, IEnumerable<T>> siblings)
        {
            var stack = new Stack<Tuple<T, T>>();
            stack.Push(Tuple.Create(root, default(T)));
            var seen = new HashSet<Tuple<T, T>>();

            while (stack.Count > 0)
            {
                var edge = stack.Pop();
                if (!seen.Add(edge))
                    return true;
                seen.Add(Tuple.Create(edge.Item2, edge.Item1));
                foreach (var sibling in siblings(edge.Item1))
                {
                    stack.Push(Tuple.Create(sibling, edge.Item1));
                }
            }
            return false;
        }

        public static string NormalizeWhiteSpace(this string str)
        {
            return Regex.Replace(str, @"\s+", " ").Trim();
        }

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

        public static string Abbreviate(this string text, int maxLength)
        {
            if (text.Length <= maxLength)
                return text;
            if (maxLength < 3)
                throw new ArgumentException();

            text = text.Substring(0, maxLength - 3) + "...";

            return text;
        }

        public static string Chop(ref string path)
        {
            var ix = path.IndexOf('.');
            if (ix == -1)
            {
                var value = path;
                path = "";
                return value;
            }
            else
            {
                var value = path.Substring(0, ix);
                path = path.Substring(ix + 1);
                return value;
            }
        }

        public static object EvalPropertyPath(object obj, string path)
        {
            object value = obj;
            foreach (var pathElement in path.Split('.'))
                value = value.GetType().GetProperty(pathElement).GetValue(value);
            return value;
        }

    }
}
