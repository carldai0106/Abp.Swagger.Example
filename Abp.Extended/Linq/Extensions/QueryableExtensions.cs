using System;
using System.Linq;
using System.Linq.Expressions;
using Abp.Application.Services.Dto;

namespace Abp.Linq.Extensions
{
    /// <summary>
    /// Some useful extension methods for <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class QueryableExtensions
    {

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Func<Expression<Func<T, bool>>> action)
        {
            var predicate = action();
            return condition
                ? query.Where(predicate)
                : query;
        }
    }
}
