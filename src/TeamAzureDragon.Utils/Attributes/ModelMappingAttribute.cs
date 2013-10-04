using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAzureDragon.Utils.Attributes
{
    // Model -> ViewModel
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class ModelMappingAttribute : Attribute
    {
        public ModelMappingAttribute(string modelProperty)
        {
            this.ModelPropertyPath = modelProperty;
        }

        public string ModelPropertyPath
        {
            get;
            set;
        }

    }
}
