using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAzureDragon.Utils.Attributes
{
    // ViewModel -> Model
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ModelNavigationIdAttribute : Attribute
    {

        // This is a positional argument
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
