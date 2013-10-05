using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAzureDragon.Utils.Attributes
{
    // Model -> ViewModel
    /// <summary>
    /// <para>Specifies the Model property path used to populate this ViewModel property.</para>
    /// <para>Used in Model to ViewModel mapping.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ModelPropertyPathAttribute : Attribute
    {
        public ModelPropertyPathAttribute(string modelPropertyPath)
        {
            this.ModelPropertyPath = modelPropertyPath;
        }

        public string ModelPropertyPath
        {
            get;
            set;
        }

    }
}
