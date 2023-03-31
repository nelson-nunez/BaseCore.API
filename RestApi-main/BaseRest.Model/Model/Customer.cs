using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseRest.Core.Model
{
    public class Customer: BaseEntity
    {
        [Column(TypeName = "VARCHAR"), StringLength(128)]
        public string Name { get; set; }

        [DataType(DataType.Date)] 
        public DateTime BirthDate { get; set; }

        [Column(TypeName = "VARCHAR"), StringLength(16)]
        public string CUIL { get; set; }

        public int GenderId { get; set; }
        public virtual Gender Gender { get; set; }

        [Column(TypeName = "VARCHAR"), StringLength(32)]
        public string Phone { get; set; }

        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var a = (today.Year * 100 + today.Month) * 100 + today.Day;
                var b = (BirthDate.Year * 100 + BirthDate.Month) * 100 + BirthDate.Day;
                return (a - b) / 10000;
            }
        }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
