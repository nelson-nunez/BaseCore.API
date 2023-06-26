using BaseRest.Core.Model;
using BaseRest.Core.Model.Base;
using BaseRest.Core.Model.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BaseRest.Core.DataAccess.Repository
{
    public class GenericRepository<TEntity> where TEntity : class
    {
        protected DbModelContext context;
        internal DbSet<TEntity> dbSet;

        public GenericRepository(DbModelContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        #region GET

        public IEnumerable<dynamic> GetListFromRawSql(string sql, List<SqlParameter> parameters)
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandTimeout = 999999;
                command.CommandText = sql;
                command.CommandType = CommandType.StoredProcedure;
                foreach (SqlParameter p in parameters)
                {
                    if (p.Value != DBNull.Value)
                        command.Parameters.Add(p);
                }

                context.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())  // read the first one to get the columns collection
                    {
                        var cols = reader.GetSchemaTable()
                                     .Rows
                                     .OfType<DataRow>()
                                     .Select(r => r["ColumnName"]);

                        do
                        {
                            dynamic t = new System.Dynamic.ExpandoObject();

                            foreach (string col in cols)
                            {
                                if (!reader.IsDBNull(col))
                                    ((IDictionary<System.String, System.Object>)t)[col] = reader[col];
                            }

                            yield return t;
                        } while (reader.Read());
                    }
                }
            }
        }
        
        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            var task = Task.Run(async () => await GetAsync(filter, orderBy, includeProperties));
            var result = task.Result;

            return task.Result;
        }
       
        public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }

        public virtual async Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.SingleOrDefaultAsync();
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync();
        }

        public virtual TEntity LocalFirstOrDefault(Expression<Func<TEntity, bool>> filter)
        {
            var query = dbSet.Local.AsQueryable();
            return query.AsTracking().FirstOrDefault(filter);
        }

        public virtual async Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.GetQueryOrderBy("Id").LastOrDefaultAsync();
        }

        public virtual async Task<PagedDataResponse<TEntity>> GetPagedResultAsync(string filterBy, string filterValue, Expression<Func<TEntity, bool>> filter, string orderBy, int pageSize, int pageIndex)
        {
            var query = dbSet.AsQueryable();


            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(filterBy) && !string.IsNullOrEmpty(filterBy))
                query = query.GetQueryFilterBy(filterBy, filterValue);

            int rowCount = query.Count();

            query = query.GetQueryOrderBy(orderBy);

            query = query.GetQueryPaged(pageSize, pageIndex, orderBy);

            var result = await query.ToListAsync();

            var pagedDataResult = new PagedDataResponse<TEntity>(pageIndex, pageSize, rowCount, result);

            return pagedDataResult;
        }

        public virtual async Task<IList<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> filter, bool ignoreFilters = false)
        {
            var query = dbSet.AsQueryable();
            if (ignoreFilters)
                query = query.IgnoreQueryFilters();


            if (filter != null)
            {
                query = query.Where(filter);
            }

            var result = await query.ToListAsync();

            return result;
        }

        public virtual async Task<IList<TEntity>> GetExecuteAsync(string procedureName, params object[] parameters)
        {
            var query = dbSet.FromSqlRaw(procedureName, parameters);
            var result = await query.ToListAsync();
            return result;
        }

        public virtual async Task<IList<dynamic>> GetDynamicExecuteAsync(string procedureName, params object[] parameters)
        {
            var query = dbSet.FromSqlRaw(procedureName, parameters);
            var result = await query.ToListAsync<dynamic>();
            return result;
        }

        public virtual TEntity Find(object id)
        {
            return dbSet.Find(id);
        }

        public virtual async Task<TEntity> FindAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task<TEntity> FindAsync(params object[] ids)
        {
            return await dbSet.FindAsync(ids);
        }

        #endregion

        #region ADD

        public virtual bool Add(TEntity entity)
        {
            dbSet.Add(entity);
            return true;
        }

        public virtual async Task<bool> AddAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
            return true;
        }

        #endregion

        #region UPDATE

        public virtual void Update(TEntity entityToUpdate)
        {
            try
            {
                //dbSet.Attach(entityToUpdate);
                context.Entry(entityToUpdate).State = EntityState.Modified;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region DELETE

        public virtual bool Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            if (entityToDelete == null) return false;
            return Delete(entityToDelete);
        }

        public virtual async Task<bool> DeleteAsync(object id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(id);
            if (entityToDelete == null) return false;
            return Delete(entityToDelete);
        }

        public virtual async Task<bool> DeleteAsync(params object[] ids)
        {
            TEntity entityToDelete = await dbSet.FindAsync(ids);
            if (entityToDelete == null) return false;
            return Delete(entityToDelete);
        }

        public virtual bool Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
            return true;
        }

        #endregion

        public async Task<object> ExecuteScalarFromRawSqlAsync(string sql)
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.StoredProcedure;
                context.Database.OpenConnection();
                return await command.ExecuteScalarAsync();
            }
        }

        public async Task<int> ExecuteSqlCommandAsync(string sql, params Microsoft.Data.SqlClient.SqlParameter[] parameters)
        {
            return await context.Database.ExecuteSqlRawAsync($"exec {sql}", parameters);
        }

        public int ExecuteSqlCommand(string sql, params Microsoft.Data.SqlClient.SqlParameter[] parameters)
        {
            return context.Database.ExecuteSqlRaw($"exec {sql}", parameters);
        }

    }
}

