using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseRest.Core.API.Common
{
    [Serializable]
    public class RemoteBusinessException : Exception
    {
        public string CodeError { get; set; }
        public RemoteBusinessException(string message) : base(message) { }

        public RemoteBusinessException(string message, string codeError) : base(message)
        {
            this.CodeError = codeError;
        }

        public RemoteBusinessException(string message, Exception innerException) : base(message, innerException) { }
    }
}