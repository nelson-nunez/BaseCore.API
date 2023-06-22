using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseRest.Core.API.Common
{
    public class SigninRequest
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Clave del usuario
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Identificador de la aplicacion cliente
        /// </summary>
        public string AppClientId { get; set; }

        /// <summary>
        /// Direccion IP de la aplicacion cliente
        /// </summary>
        public string ClientIPAddress { get; set; }

    }
}