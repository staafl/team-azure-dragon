using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAzureDragon.Utils.Attributes
{
    /// <summary>
    /// <para>Specifies that this ViewModel property contains navigation property id value(s), which will be used when building the Model object.</para>
    /// <para>Used in ViewModel to Model mapping.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ModelNavigationIdAttribute : Attribute
    {
        public ModelNavigationIdAttribute(string navigationProperty)
        {
            this.NavigationProperty = navigationProperty;
        }

        public string NavigationProperty
        {
            get;
            set;
        }
    }
}
