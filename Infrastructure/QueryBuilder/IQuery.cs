using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Infrastructure.QueryBuilder
{
    public interface IQuery<T> where T : class
    {
        Expression<Func<T, bool>> FilterExpressions { get; }
        IList<Sorting> Sortings { get; }
        Paging Paging { get; }
        Topping Topping { get; }

        Query<T> AndFilter(Expression<Func<T, bool>> filter);
        Query<T> OrFilter(Expression<Func<T, bool>> filter);
        Query<T> IncludeRelationship(Expression<Func<T, object>> relationship);
        Query<T> IncludeRelationship(IList<Expression<Func<T, object>>> relationships);
        Query<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);
        Query<T> OrderBy(string propertyName);
        Query<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
        Query<T> OrderByDescending(string propertyName);
        Query<T> Page(int? pageIndex, int? pageSize);
        Query<T> Top(int? topRows);
    }
}