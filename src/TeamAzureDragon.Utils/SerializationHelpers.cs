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

    public interface IViewModel { }
    public interface IViewModel<TModel> : IViewModel
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


        public static TVM FillViewModel<TVM, TM>(this TVM viewModel, TM model) where TVM : IViewModel
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            foreach (PropertyInfo viewModelProp in viewModel.GetType().GetProperties(flags))
            {
                var type = viewModelProp.PropertyType;
                var attr = (ModelPropertyPathAttribute)viewModelProp.GetCustomAttribute(typeof(ModelPropertyPathAttribute));
                var name = viewModelProp.Name;

                if (IsICollection(type))
                {
                    // todo: check if generic type

                    // TODO: UGLY HAIRY HACKERY
                    var vmSet = MakeSet(type);
                    var collection = (IEnumerable)(typeof(TM).GetProperty(name).GetValue(model));
                    var vmType = type.GetGenericArguments()[0];

                    var add = vmSet.GetType().GetMethod("Add");
                    foreach (var navEntity in collection)
                    {
                        object vm = FillNavPropertyViewModel(navEntity, vmType);
                        add.Invoke(vmSet, new object[] { vm });
                    }

                    viewModelProp.SetValue(viewModel, vmSet);
                }
                else if (typeof(IViewModel).IsAssignableFrom(type))
                {
                    // get the navigation entity
                    var navEntity = Misc.EvalPropertyPath(model, name);

                    var vm = FillNavPropertyViewModel(navEntity, type);
                    // put it on us
                    viewModelProp.SetValue(viewModel, vm);
                }
                else
                {
                    viewModelProp.SetValue(viewModel, Misc.EvalPropertyPath(model, name));
                    continue;
                }
            }
            return viewModel;
        }

        static object FillNavPropertyViewModel(object navEntity, Type vmType)
        {
            // creaet an empty viewmodel
            var vm = Activator.CreateInstance(vmType);
            // fill it
            var method = typeof(Misc).GetMethod("FillViewModel").MakeGenericMethod(vmType, navEntity.GetType());
            method.Invoke(null, new object[] { vm, navEntity });
            return vm;
        }

        public static TM FillModel<TM>(this IViewModel<TM> viewModel, DbContext context, TM model = null) where TM : class
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            model = model ?? Activator.CreateInstance<TM>();
            foreach (PropertyInfo vmProp in viewModel.GetType().GetProperties(flags))
            {
                var vmValue = vmProp.GetValue(viewModel);
                if (vmValue == null)
                    continue;
                var name = vmProp.Name;

                var modelProp = typeof(TM).GetProperty(name, flags);
                var modelPropType = modelProp.PropertyType;

                if (IsICollection(modelPropType))
                {
                    // TODO: UGLY HACK
                    var navCollection = modelProp.GetValue(model);
                    var addMethod = modelPropType.GetMethod("Add", flags);
                    var vmType = vmProp.PropertyType.GetGenericArguments()[0];
                    var idProp = GetIdProp(vmType);
                    var entityType = modelPropType.GetGenericArguments()[0];
                    var dbset = context.Set(entityType);

                    var vmCollection = (IEnumerable)vmValue;

                    foreach (var vmElem in vmCollection)
                    {
                        var id = idProp.GetValue(vmElem);
                        var entity = context.Set(entityType).Find(id);
                        addMethod.Invoke(navCollection, new object[] { entity });
                    }
                }
                else if (typeof(IViewModel).IsAssignableFrom(vmProp.PropertyType))
                {
                    var entityType = modelPropType;
                    var vmType = vmProp.PropertyType;
                    var idProp = GetIdProp(vmType);
                    var id = idProp.GetValue(vmValue);
                    var entity = context.Set(entityType).Find(id);
                    modelProp.SetValue(model, entity);
                }
                else
                {
                    modelProp.SetValue(model, vmValue);
                }
            }
            return model;

        }

        static PropertyInfo GetIdProp(Type vmType)
        {
            foreach (var prop in vmType.GetProperties())
            {
                if (prop.GetCustomAttributes<ModelNavigationIdAttribute>().Any())
                    return prop;
            }
            return null;
        }

        static object MakeSet(Type typeofICollectionOfT)
        {
            var typeofT = typeofICollectionOfT.GenericTypeArguments[0];
            var typeOfList = typeof(HashSet<>).MakeGenericType(typeofT);
            return Activator.CreateInstance(typeOfList);
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
