using Infrastructure.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public interface IRepository : IDisposable
    { }

    public interface IRepository<T> : IRepository where T : class
    {
        IQuery<T> Query();

        IList<T> ListAll();
        IQueryable<T> GetListAll(IList<Expression<Func<T, object>>> relationships);
        IQueryable<T> GetListAll();
        IList<T> ListAll(IList<Expression<Func<T, object>>> relationships);
        IList<T> ListWhere(IList<Expression<Func<T, object>>> relationships, Expression<Func<T, bool>> where);
        IList<T> Search(Query<T> query);
        Task<List<T>> SearchAsync(Query<T> query);
        T SingleOrDefault(Expression<Func<T, bool>> predicate);
        T SingleOrDefault(Expression<Func<T, bool>> predicate, IList<Expression<Func<T, object>>> relationships);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate, IList<Expression<Func<T, object>>> relationships);
        bool Any(Expression<Func<T, bool>> predicate);
        bool Any(Expression<Func<T, bool>> predicate, IList<Expression<Func<T, object>>> relationships);
        int Count(Expression<Func<T, bool>> predicate);
        TResult Max<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>> predicate = null);
        T Add(T entity);
        T Update(T entity);
        T Delete(T entity);
        void Delete(Expression<Func<T, bool>> predicate);
        T Reload(T entity);
        IList<T> SqlQuery(string query, params object[] parameters);
        void ExecuteSqlCommand(string query, params object[] parameters);
        string GetTableName();
        IDictionary<PropertyInfo, string> GetMappings();
    }
}