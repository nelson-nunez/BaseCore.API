using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseRest.Core.Model
{
    public class Gender : BaseEntity
    {
        public const int Male = 1;
        public const int Female = 2;
        public const int Other = 3;

        [Column(TypeName = "VARCHAR"), StringLength(128)]
        public string Name { get; set; }
    }
}
