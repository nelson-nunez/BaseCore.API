using BaseRest.Core.DataAccess;
using BaseRest.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using BaseRest.Core.Model.Base;
using System.Threading;

namespace BaseRest.Core.Business
{
    public class CustomerBusiness
    {
        public readonly UnitOfWork unitOfWork;
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);


        public CustomerBusiness(UnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<Customer> FindAsync(int id)
        {
            return await unitOfWork.CustomerRepository.FindAsync(id);
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

        public async Task<PagedDataResponse<Customer>> GetPagedResultAsync(PagingSortFilterRequest request)
        {
            IEnumerable<Customer> list = new List<Customer>();
            Expression<Func<Customer, bool>> filter = x => true;

            // PARA AGREGAR CLASES CON FILTROS ESPECIALES
            //if (!string.IsNullOrEmpty(cuit))
            //    filter = filter.And(x => x.Person.CUIT.Contains(cuit) || x.Person.PersonDocuments
            //                   .Where(x => x.DocumentTypeId == DocumentType.CuitTypeId)
            //                   .Any(y => y.DocumentNumber.Contains(cuit)));

            var pagedDataResult = await unitOfWork.CustomerRepository.GetPagedResultAsync(request.FilterBy, request.FilterValue, filter, request.OrderBy, request.PageSize, request.PageIndex);
            return pagedDataResult;
        }

        public async Task<int> CustomerSaveAsync(Customer entity)
        {
            try
            {
                //Prueba con semáforo
                await _semaphoreSlim.WaitAsync();


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
            catch (Exception)
            {

                throw;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
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
