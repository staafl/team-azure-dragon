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
    // TODO: Flags
    public enum RecursiveSerializationOption
    {
        Recurse,
        Flatten,
        Assign,
        ForeachAssign,
        ForeachRecurse,
        Skip
    }


    public static class Misc
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


        public static Dictionary<string, object> SerializeToDictionary(object from,
            Func<string, RecursiveSerializationOption> customHandler = null,
            Func<string, string, string> keyGetter = null,
            string basePath = "",
            Dictionary<string, object> baseDict = null)
        {
            var dict = baseDict ?? new Dictionary<string, object>();

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
                    dict[key] = null;
                    continue;
                }

                if (typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType))
                {
                    if (result == RecursiveSerializationOption.ForeachAssign)
                    {
                        var list = new List<object>();
                        dict[key] = list;
                        foreach (object elem in (value as IEnumerable) ?? new object[0])
                        {
                            list.Add(elem);
                        }
                        continue;
                    }
                    if (result == RecursiveSerializationOption.ForeachRecurse)
                    {
                        var list = new List<Dictionary<string, object>>();
                        dict[key] = list;
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
                    dict[key] = SerializeToDictionary(value, customHandler, keyGetter, path);
                    continue;
                }

                if (result == RecursiveSerializationOption.Flatten)
                {
                    SerializeToDictionary(value, customHandler, keyGetter, path, dict);
                    continue;
                }
                throw new ApplicationException();
            }


            return dict;

        }

        public static TVM ModelToViewModel<TM, TVM>(TM model) where TVM : new()
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var viewModel = new TVM();
            foreach (PropertyInfo viewModelProp in typeof(TVM).GetProperties(flags))
            {
                
                var attr = viewModelProp.GetCustomAttribute(typeof(ModelMappingAttribute));
                string path = viewModelProp.Name;
                if (attr != null)
                {
                    path = ((ModelMappingAttribute)attr).ModelPropertyPath;

                }

                //object[] toTraverse = new object[]{model};
                //dynamic list = null;
                //if (typeof(IEnumerable).IsAssignableFrom(viewModelProp.PropertyType))
                //{
                //    var typeOfList = typeof(List<>).MakeGenericType(viewModelProp.PropertyType.GenericTypeArguments[0]);
                //    list = Activator.CreateInstance(typeOfList);
                //    //toTraverse = ((ICollection)model.GetType().GetProperty(path.Split('.')[0]).GetValue(model)).To;

                //}
                object modelValue = model;
                foreach (var pathElement in path.Split('.'))
                {
                    modelValue = modelValue.GetType().GetProperty(pathElement).GetValue(modelValue);
                }
                viewModelProp.SetValue(viewModel, modelValue);
            }

            return viewModel;
        }
        // ViewModel -> Model
        public static TM ViewModelToModel<TVM, TM>(TVM viewModel, DbContext context)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var model = Activator.CreateInstance<TM>();
            foreach (PropertyInfo viewModelProp in typeof(TVM).GetProperties(flags))
            {
                var attr = viewModelProp.GetCustomAttribute(typeof(ModelNavigationIdAttribute));


                var viewModelValue = viewModelProp.GetValue(viewModel);
                if (attr == null)
                {
                    var name = viewModelProp.Name;

                    var modelProp = typeof(TM).GetProperty(name, flags);

                    if (modelProp == null)
                        continue;

                    // map directly
                    modelProp.SetValue(model, viewModelValue);
                    continue;

                }

                var property = typeof(TM).GetProperty(((ModelNavigationIdAttribute)attr).NavigationProperty);
                var entityType = property.PropertyType;
                var entity = context.Set(entityType).Find(viewModelValue);
                property.SetValue(model, entity);
            }

            return model;

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
