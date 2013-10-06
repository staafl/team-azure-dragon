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

    public interface IViewModel<TModel>
    {
    }

    public static partial class Misc
    {
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
                            list.Add(SerializeToDictionary(elem, customHandler, keyGetter, path));
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

        public static bool IsICollection(Type type)
        {
            if (type == typeof(string))
                return false;

            if (!typeof(IEnumerable).IsAssignableFrom(type))
                return false;

            if (type.IsInterface)
            {

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
                    return true;
                return false;
            }
            else
            {
                var ifaces = type.GetInterfaces();
                return ifaces.Any(IsICollection);
            }

        }

        public static TVM FillViewModel<TVM, TM>(this TVM viewModel, TM model) where TVM : IViewModel<TM>
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            foreach (PropertyInfo viewModelProp in viewModel.GetType().GetProperties(flags))
            {
                var type = viewModelProp.PropertyType;
                var attr = (ModelPropertyPathAttribute)viewModelProp.GetCustomAttribute(typeof(ModelPropertyPathAttribute));

                string path = attr == null ? viewModelProp.Name : attr.ModelPropertyPath;

                if (IsICollection(type))
                {
                    // todo: check if generic type

                    // TODO: UGLY HAIRY HACKERY
                    var list = MakeIList(type);
                    var collectionName = Misc.Chop(ref path);

                    var collection = (IEnumerable)(typeof(TM).GetProperty(collectionName).GetValue(model));
                    foreach (var elem in collection)
                        list.Add(Misc.EvalPropertyPath(elem, path));

                    viewModelProp.SetValue(viewModel, list);
                }
                else
                {
                    viewModelProp.SetValue(viewModel, Misc.EvalPropertyPath(model, path));
                }
            }
            return viewModel;
        }

        public static TM CreateModel<TM>(this IViewModel<TM> viewModel, DbContext context)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var model = Activator.CreateInstance<TM>();
            foreach (PropertyInfo viewModelProp in viewModel.GetType().GetProperties(flags))
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

                var navProperty = typeof(TM).GetProperty(((ModelNavigationIdAttribute)attr).NavigationProperty);
                var navPropertyType = navProperty.PropertyType;

                if (IsICollection(navPropertyType))
                {

                    // TODO: UGLY HACK
                    var navCollection = navProperty.GetValue(model);
                    var addMethod = navPropertyType.GetMethod("Add", flags);
                    var entityType = navPropertyType.GetGenericArguments()[0];
                    var dbset = context.Set(entityType);

                    var idCollection = (IEnumerable)viewModelValue;
                    foreach (var id in idCollection)
                    {
                        var entity = context.Set(entityType).Find(id);
                        addMethod.Invoke(navCollection, new object[] { entity });
                    }
                }
                else
                {
                    var entityType = navPropertyType;
                    var entity = context.Set(entityType).Find(viewModelValue);
                    navProperty.SetValue(model, entity);
                }
            }

            return model;

        }

        static IList MakeIList(Type typeofICollectionOfT)
        {
            var typeofT = typeofICollectionOfT.GenericTypeArguments[0];
            var typeOfList = typeof(List<>).MakeGenericType(typeofT);
            return (IList)Activator.CreateInstance(typeOfList);
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
