using BaseRest.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseRest.Core.DataAccess.Repository
{
    public class GenderRepository : GenericRepository<Gender>
    {
        public GenderRepository(DbModelContext context) : base(context)
        {

        }
    }
}


