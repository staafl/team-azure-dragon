﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAzureDragon.Utils
{
    // todo: serializable
    public class VersionedString
    {
        public static T ParseVersionedXml<T>(string versionedData)
        {
            var xml = VersionedString.Read(versionedData).Data;
            return (T)(new XmlSerializer(typeof(T))
                .Deserialize(new StringReader(xml)));
        }

        public string Data { get; set; }
        public int Version { get; set; }

        public override string ToString()
        {
            return this.Version + "\a" + this.Data;
        }

        public static int CurrentVersion { get; set; }
        public static int? DefaultVersion { get; set; }

        public static VersionedString Read(string rawData, bool allowDefaultVersion = true, int? overrideDefaultVersion = null)
        {
            int ix = rawData.IndexOf('\a');
            int version;
            string data;
            if (ix == -1)
            {
                if (allowDefaultVersion && (overrideDefaultVersion ?? DefaultVersion).HasValue)
                {
                    version = (overrideDefaultVersion ?? DefaultVersion).Value;
                    data = rawData;
                }
                else
                {
                    throw new ArgumentException("rawData: " + rawData);
                }
            }
            else
            {
                version = int.Parse(rawData.Substring(0, ix));
                data = rawData.Substring(ix + 1);
            }

            if (version > CurrentVersion)
                throw new VersionMismatchException(
                    "Current version: " + CurrentVersion + ", Data version: " + version);

            return new VersionedString { Data = data, Version = version };
        }
    }
}
