using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseRest.Core.API.Common
{
    public class SigninResponse
    {
        public string SignInResult { get; set; }
        public string AccessToken { get; set; }

    }
}