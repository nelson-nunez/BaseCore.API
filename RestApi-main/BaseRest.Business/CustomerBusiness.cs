using BaseRest.Core.DataAccess;
using BaseRest.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BaseRest.Core.Business
{
    public class CustomerBusiness
    {
        public readonly UnitOfWork unitOfWork;
        public CustomerBusiness(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Customer> FindAsync(int id)
        {
            return await unitOfWork.CustomerRepository.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Customer> FindByUserNameAsync(string name)
        {
            Customer result = null;
            result = await unitOfWork.CustomerRepository.FirstOrDefaultAsync(x => x.Name == name);
            return result;
        }

        public async Task<IEnumerable<Customer>> GetAsync()
        {
            return await unitOfWork.CustomerRepository.GetAsync();
        }

        public async Task<IEnumerable<Customer>> GetListAsync(string sellerName)
        {
            IEnumerable<Customer> list = new List<Customer>();
            list = await unitOfWork.CustomerRepository.GetListAsync(x => x.Name.ToUpper().Contains(sellerName.ToUpper()));
            return list;
        }

        public async Task<int> CustomerSaveAsync(Customer entity)
        {
            if (entity.Id == 0)
            {
                await unitOfWork.CustomerRepository.AddAsync(entity);
            }
            else
            {
                unitOfWork.CustomerRepository.Update(entity);
            }
            await unitOfWork.CompleteAsync();
            return entity.Id;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customerToDelete = await unitOfWork.CustomerRepository.FindAsync(id);
            unitOfWork.Delete(customerToDelete);
            await unitOfWork.CompleteAsync();
            return true;
        }
    }
}
