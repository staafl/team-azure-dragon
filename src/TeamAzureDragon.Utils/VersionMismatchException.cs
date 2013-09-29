using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamAzureDragon.Utils
{

    [Serializable]
    public class VersionMismatchException : Exception
    {
        public VersionMismatchException() { }
        public VersionMismatchException(string message) : base(message) { }
        public VersionMismatchException(string message, Exception inner) : base(message, inner) { }
        protected VersionMismatchException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
