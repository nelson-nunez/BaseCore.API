using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BaseRest.Core.Model.DTO
{
    public class CustomerDTO: BaseEntityDTO<int>
    {
        public string Name { get; set; }

        public DateTime BirthDate { get; set; }

        public string CUIL { get; set; }

        public int GenderId { get; set; }

        public string Phone { get; set; }

        public int Age { get; set; }
    }
}
