using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseRest.Core.API.Common
{
    public class WebApiConfig
    {
        public string BaseAddress { get; set; }

        public string Token { get; set; }

        public int Timeout { get; set; }
    }
}