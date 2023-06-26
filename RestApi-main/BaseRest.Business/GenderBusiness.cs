using BaseRest.Core.DataAccess;
using BaseRest.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BaseRest.Core.Model.Base;

namespace BaseRest.Core.Business
{
    public class GenderBusiness
    {
        public readonly UnitOfWork unitOfWork;
        public GenderBusiness(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Gender>> GetAsync()
        {
            return await unitOfWork.GenderRepository.GetAsync();
        }

        public async Task<IEnumerable<Gender>> GetListAsync(string name)
        {
            IEnumerable<Gender> list = new List<Gender>();
            list = await unitOfWork.GenderRepository.GetListAsync(x => x.Name.ToUpper().Contains(name.ToUpper()));
            return list;
        }
    }
}
