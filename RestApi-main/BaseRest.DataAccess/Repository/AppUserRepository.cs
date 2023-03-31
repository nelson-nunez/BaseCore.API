using BaseRest.Core.Model;
using BaseRest.Core.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseRest.Core.DataAccess.Repository
{
    public class AppUserRepository : GenericRepository<AppUser>
    {
        public AppUserRepository(DbModelContext context) : base(context)
        { }
    }
}
