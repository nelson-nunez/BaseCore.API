using BaseRest.Core.DataAccess.Repository;
using BaseRest.Core.Model;
using BaseRest.Core.Model.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BaseRest.Core.DataAccess
{
    public class UnitOfWork : IDisposable
    {
        #region Vars

        private readonly DbModelContext context;

        private readonly IHttpContextAccessor httpContextAccessor;

        private bool disposed = false;

        #endregion

        #region Repositorios

        private GenderRepository genderRepository;
        public GenderRepository GenderRepository
        {
            get
            {
                this.genderRepository ??= new GenderRepository(context);
                return genderRepository;
            }
        }
        
        private CustomerRepository customerRepository;
        public CustomerRepository CustomerRepository
        {
            get
            {
                this.customerRepository ??= new CustomerRepository(context);
                return customerRepository;
            }
        }
     
        private AppUserRepository appUserRepository;
        public AppUserRepository AppUserRepository
        {
            get
            {
                this.appUserRepository ??= new AppUserRepository(context);
                return appUserRepository;
            }
        }
        #endregion

        public UnitOfWork(DbModelContext context, IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        public void Delete(object entity)
        {
            context.Entry(entity).State = EntityState.Deleted;
        }

        public void Complete()
        {
            var task = Task.Run(async () => await CompleteAsync());
        }

        public async Task CompleteAsync()
        {
            var changedObjects = context.ChangeTracker.Entries<IEntity>();
            foreach (var entry in changedObjects)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = DateTime.Now;
                        entry.Entity.CreatedBy = httpContextAccessor.HttpContext.User.Identity.IsAuthenticated ? httpContextAccessor.HttpContext.User.Identity.Name : string.Empty;
                        break;

                    case EntityState.Modified:
                        entry.Entity.Updated = DateTime.Now;
                        entry.Entity.UpdatedBy = httpContextAccessor.HttpContext.User.Identity.IsAuthenticated ? httpContextAccessor.HttpContext.User.Identity.Name : string.Empty;
                        break;

                    case EntityState.Deleted when entry.Entity is ISoftDelete:
                        entry.State = EntityState.Modified;
                        entry.Entity.Deleted = DateTime.Now;
                        entry.Entity.DeletedBy = httpContextAccessor.HttpContext.User.Identity.IsAuthenticated ? httpContextAccessor.HttpContext.User.Identity.Name : string.Empty;
                        break;
                }
            }
            await context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

