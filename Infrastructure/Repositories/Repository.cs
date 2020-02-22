using EntityFramework.Extensions;
using LinqKit;
using Infrastructure.Extensions;
using Infrastructure.QueryBuilder;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Entity Framework

        // Entity Framework:
        //
        // Método AsNoTracking => Sempre quando invocado por consultas que utilizam IQueryable, o mesmo não mantém nenhum rastreamento das entidades, retornando sempre as entidades com o EntityState Detached. (Aplicado para todo o graph).
        //
        // Método Attach => Sempre quando invocado o mesmo altera o EntityState das entidades para Unchanged caso o EntityState das entidades seja Detached, porém sempre altera o EntityState da entidade parent para Unchanged. (Aplicado para todo o graph).
        //                  Obs.: Se for invocado mais de uma vez, altera apenas o EntityState da entidade parent para Unchanged e mantém o EntityState existente das entidades.
        //
        // Método Add => Sempre quando invocado o mesmo altera o EntityState das entidades para Added caso o EntityState das entidades seja Detached, porém sempre altera o EntityState da entidade parent para para Added. (Aplicado para todo o graph).
        //               Obs.: Se for invocado mais de uma vez, altera apenas o EntityState da entidade parent para Added e mantém o EntityState existente das entidades.

        #endregion

        #region Ctor

        public Repository(DbContext dbContext)
        {
            DbContext = dbContext ?? throw new ArgumentNullException("dbContext", "dbContext cannot be null.");
            DbSet = dbContext.Set<T>();
        }

        #endregion

        #region Protected Properties

        protected DbContext DbContext { get; set; }

        protected DbSet<T> DbSet { get; set; }

        #endregion

        #region IRepository<T> Members

        public virtual IQuery<T> Query()
        {
            var container = Container.For<InfrastructureRegistry>();
            return container.GetInstance<IQuery<T>>();
        }

        public virtual IList<T> ListAll() => DbSet.AsNoTracking().ToList();

        public virtual IQueryable<T> GetListAll(IList<Expression<Func<T, object>>> relationships)
        {
            return relationships.Aggregate<Expression<Func<T, object>>, IQueryable<T>>(DbSet,
             (current, path) => current.Include(path));
        }

        public virtual IQueryable<T> GetListAll()
        {
            return DbSet.AsNoTracking();
        }


        public virtual IList<T> ListAll(IList<Expression<Func<T, object>>> relationships)
        {
            var queryable = relationships.Aggregate<Expression<Func<T, object>>, IQueryable<T>>(DbSet,
              (current, path) => current.Include(path));

            return queryable.AsNoTracking().ToList();
        }



        public virtual IList<T> ListWhere(IList<Expression<Func<T, object>>> relationships, Expression<Func<T, bool>> where)
        {
            var queryable = relationships.Aggregate<Expression<Func<T, object>>, IQueryable<T>>(DbSet,
              (current, path) => current.Include(path));

            return queryable.Where(where).AsNoTracking().ToList();
        }

        public virtual IList<T> Search(Query<T> query)
        {
            var queryable = ToQueryable(query);
#if DEBUG
            var sql = queryable.ToString();
#endif
            return queryable.AsNoTracking().ToList();
        }

        public virtual Task<List<T>> SearchAsync(Query<T> query)
        {
            var queryable = ToQueryable(query);
#if DEBUG
            var sql = queryable.ToString();
#endif
            return queryable.AsNoTracking().ToListAsync();
        }

        public virtual T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return DbSet.AsNoTracking().SingleOrDefault(predicate);
        }

        public virtual T SingleOrDefault(Expression<Func<T, bool>> predicate, IList<Expression<Func<T, object>>> relationships)
        {
            var queryable = relationships.Aggregate<Expression<Func<T, object>>, IQueryable<T>>(DbSet,
             (current, path) => current.Include(path));

            return queryable.AsNoTracking().SingleOrDefault(predicate);
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return DbSet.AsNoTracking().FirstOrDefault(predicate);
        }

        public virtual T FirstOrDefault(Expression<Func<T, bool>> predicate, IList<Expression<Func<T, object>>> relationships)
        {
            var queryable = relationships.Aggregate<Expression<Func<T, object>>, IQueryable<T>>(DbSet,
            (current, path) => current.Include(path));

            return queryable.AsNoTracking().FirstOrDefault(predicate);
        }

        public virtual T LastOrDefault(Expression<Func<T, bool>> predicate, IList<Expression<Func<T, object>>> relationships)
        {
            var queryable = relationships.Aggregate<Expression<Func<T, object>>, IQueryable<T>>(DbSet,
            (current, path) => current.Include(path));

            return queryable.AsNoTracking().LastOrDefault(predicate);
        }

        public virtual bool Any(Expression<Func<T, bool>> predicate)
        {
            return DbSet.AsNoTracking().Any(predicate);
        }

        public virtual bool Any(Expression<Func<T, bool>> predicate, IList<Expression<Func<T, object>>> relationships)
        {
            var queryable = relationships.Aggregate<Expression<Func<T, object>>, IQueryable<T>>(DbSet,
             (current, path) => current.Include(path));

            return queryable.AsNoTracking().Any(predicate);
        }

        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            return DbSet.AsNoTracking().Count(predicate);
        }

        public TResult Max<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null)
        {
            return predicate != null ? DbSet.AsNoTracking().Where(predicate).Max(selector) : DbSet.AsNoTracking().Max(selector);
        }

        public virtual T Add(T entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);

            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                entity = DbSet.Add(entity);
            }

            return entity;
        }

        public virtual T Update(T entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);

            if (dbEntityEntry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            dbEntityEntry.State = EntityState.Modified;

            return entity;
        }

        public virtual T Delete(T entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);

            if (dbEntityEntry.State == EntityState.Detached)
            {
                entity = DbSet.Attach(entity);
            }

            entity = DbSet.Remove(entity);

            return entity;
        }

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            DbSet.Where(predicate).Delete();
        }

        public virtual T Reload(T entity)
        {
            var dbEntityEntry = DbContext.Entry(entity);

            if (dbEntityEntry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }

            dbEntityEntry.Reload();

            return entity;
        }

        public IList<T> SqlQuery(string query, params object[] parameters)
        {
            return DbContext.Database.SqlQuery<T>(query, parameters).ToList();
        }

        public void ExecuteSqlCommand(string query, params object[] parameters)
        {
            DbContext.Database.ExecuteSqlCommand(query, parameters);
        }

        public string GetTableName() => DbContext.GetTableName(typeof(T));

        public IDictionary<PropertyInfo, string> GetMappings() => DbContext.GetMappings(typeof(T));

        #endregion

        #region Private Methods

        IQueryable<T> ToQueryable(Query<T> query)
        {
            IQueryable<T> queryable = DbSet;

            foreach (var path in query.Relationships)
                queryable = queryable.Include(path);


            if (query.Paging.IsPagingEnabled)
            {
                IQueryable<T> countQueryable = DbSet;

                foreach (var path in query.Relationships)
                    countQueryable = countQueryable.Include(path);


                countQueryable = query.FilterExpressions != null ? countQueryable.AsExpandable().Where(query.FilterExpressions) : countQueryable.AsExpandable();

                query.Paging.RecordCount = countQueryable.Count();

                var lastPage = (int)Math.Ceiling(Convert.ToDecimal(query.Paging.RecordCount) / Convert.ToDecimal(query.Paging.PageSize.Value));

                if (lastPage < query.Paging.PageIndex.GetValueOrDefault() && lastPage > 0)
                {
                    query.Page(lastPage, query.Paging.PageSize.Value);
                }
            }

            var isOrderedQueryable = false;

            queryable = query.FilterExpressions != null ? queryable.AsExpandable().Where(query.FilterExpressions) : queryable.AsExpandable();

            foreach (var sorting in query.Sortings)
            {
                if (sorting.Direction == SortDirection.Ascending)
                {
                    queryable = !isOrderedQueryable ? queryable.OrderBy(sorting.PropertyName) : ((IOrderedQueryable<T>)queryable).ThenBy(sorting.PropertyName);

                    isOrderedQueryable = true;
                }
                else
                {
                    queryable = !isOrderedQueryable ? queryable.OrderByDescending(sorting.PropertyName) : ((IOrderedQueryable<T>)queryable).ThenByDescending(sorting.PropertyName);

                    isOrderedQueryable = true;
                }
            }

            if (query.Topping.IsDefined)
            {
                queryable = queryable.Take(query.Topping.PageSize);
            }

            if (query.Paging.IsPagingEnabled)
            {
                queryable = queryable.Skip((query.Paging.PageIndex.GetValueOrDefault() - 1) * query.Paging.PageSize.GetValueOrDefault()).Take(query.Paging.PageSize.GetValueOrDefault());
            }

            return queryable;
        }

        #endregion

        #region IDisposable Members

        bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Não dar dispose no context, uma vez que vários repositories podem estar usando o mesmo context
                }
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}