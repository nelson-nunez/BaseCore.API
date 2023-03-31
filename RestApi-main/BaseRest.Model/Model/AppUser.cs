using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseRest.Core.Model.Model
{
    public class AppUser : BaseEntity
    {
        [Column(TypeName = "VARCHAR"), StringLength(64)]
        public string Name { get; set; }

        [Column(TypeName = "VARCHAR"), StringLength(128)]
        public string Password { get; set; }
    }
}
