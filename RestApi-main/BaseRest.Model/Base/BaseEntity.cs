using BaseRest.Core.Model.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseRest.Core.Model
{
    public abstract class BaseEntity: IEntity, ISoftDelete
    {
        public int Id { get; set; }

        [Column(TypeName = "VARCHAR"), StringLength(64)]
        public string CreatedBy { get; set; }
        [Column(TypeName = "VARCHAR"), StringLength(64)]
        public string UpdatedBy { get; set; }
        [Column(TypeName = "VARCHAR"), StringLength(64)]
        public string DeletedBy { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public DateTime? Deleted { get; set; }
    }
}
