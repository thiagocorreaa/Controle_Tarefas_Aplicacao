using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Infrastructure.QueryBuilder
{
    public class Query<T> : IQuery<T> where T : class
    {
        public Query()
        {
            FilterExpressions = PredicateBuilder.New<T>(true);
            Relationships = new List<Expression<Func<T, object>>>();
            Sortings = new List<Sorting>();
            Paging = new Paging();
            Topping = new Topping();
        }

        #region IQuery<T> Members

        public Expression<Func<T, bool>> FilterExpressions { get; private set; }
        public IList<Expression<Func<T, object>>> Relationships { get; private set; }
        public IList<Sorting> Sortings { get; private set; }
        public Paging Paging { get; private set; }
        public Topping Topping { get; private set; }

        public Query<T> AndFilter(Expression<Func<T, bool>> filter)
        {
            FilterExpressions = FilterExpressions.And(filter);

            return this;
        }

        public Query<T> OrFilter(Expression<Func<T, bool>> filter)
        {
            FilterExpressions = FilterExpressions.Or(filter);

            return this;
        }

        public Query<T> IncludeRelationship(Expression<Func<T, object>> relationship)
        {
            Relationships.Add(relationship);

            return this;
        }

        public Query<T> IncludeRelationship(IList<Expression<Func<T, object>>> relationships)
        {
            foreach (var relationship in relationships)
                Relationships.Add(relationship);


            return this;
        }

        public Query<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            Sortings.Add(new Sorting
            {
                PropertyName = GetPropertyPath(keySelector),
                Direction = SortDirection.Ascending
            });

            return this;
        }

        public Query<T> OrderBy(string propertyName)
        {
            Sortings.Add(new Sorting
            {
                PropertyName = propertyName,
                Direction = SortDirection.Ascending
            });

            return this;
        }

        public Query<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            Sortings.Add(new Sorting
            {
                PropertyName = GetPropertyPath(keySelector),
                Direction = SortDirection.Descending
            });

            return this;
        }

        public Query<T> OrderByDescending(string propertyName)
        {
            Sortings.Add(new Sorting
            {
                PropertyName = propertyName,
                Direction = SortDirection.Descending
            });

            return this;
        }

        public Query<T> Page(int? pageIndex, int? pageSize)
        {
            Paging.PageIndex = pageIndex;
            Paging.PageSize = pageSize;

            return this;
        }

        public Query<T> Top(int? topRows)
        {
            Topping.PageSize = topRows ?? Topping.TOP_NONE;

            return this;
        }

        #endregion

        static string GetPropertyPath<TProperty>(Expression<Func<T, TProperty>> selector)
        {
            var stringBuilder = new StringBuilder();

            var memberExpression = selector.Body as MemberExpression;

            while (memberExpression != null)
            {
                var name = memberExpression.Member.Name;

                if (stringBuilder.Length > 0)
                {
                    name = string.Concat(name, ".");
                }

                stringBuilder.Insert(0, name);

                if (memberExpression.Expression is ParameterExpression)
                {
                    return stringBuilder.ToString();
                }

                memberExpression = memberExpression.Expression as MemberExpression;
            }

            throw new ArgumentException("The expression must be a MemberExpression", "selector");
        }
    }
}