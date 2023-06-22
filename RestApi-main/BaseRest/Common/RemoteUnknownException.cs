using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseRest.Core.API.Common
{
    [Serializable]
    public class RemoteUnknownException : Exception
    {
        public string CodeError { get; set; }
        public string BaseAddress { get; set; }

        public string ExternalLogId { get; set; }

        public RemoteUnknownException(string message) : base(message) { }

        public RemoteUnknownException(string message, string codeError) : base(message)
        {
            this.CodeError = codeError;
        }

        public RemoteUnknownException(string message, string baseAddress, string externalLogId) : base(message)
        {
            BaseAddress = baseAddress;
            ExternalLogId = externalLogId;
        }
    }
}