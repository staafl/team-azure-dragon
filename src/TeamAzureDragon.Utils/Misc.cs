using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TeamAzureDragon.Utils
{
    public static class Misc
    {
        public static T ParseVersioned<T>(string versionedData)
        {
            var xml = VersionedString.Read(versionedData).Data;
            return (T)(new XmlSerializer(typeof(T))
                .Deserialize(new StringReader(xml)));
        }
    }
}
