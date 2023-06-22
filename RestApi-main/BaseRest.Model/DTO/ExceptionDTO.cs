using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseRest.Core.Model.DTO
{
    public class ExceptionDTO
    {
        public string Message { get; set; }

        public string LogId { get; set; }

        public string StackTrace { get; set; }
    }
}