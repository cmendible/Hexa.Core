using System;
using System.Linq;

namespace Hexa.Core.Domain.Specification
{
    /// <summary>
    /// Base contract for Specification pattern, for more information
    /// about this pattern see http://martinfowler.com/apsupp/spec.pdf
    /// or http://en.wikipedia.org/wiki/Specification_pattern.
    /// This is really a variant implementation where we have added Linq and
    /// lambda expression into this pattern.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity</typeparam>
    public interface IOrderBySpecification<TEntity>
        where TEntity : class
    {
        IOrderedQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> query);
    }

    public static class OrderBySpecificationExtensions
    {
        public static IOrderedQueryable<TEntity> OrderBySpecification<TEntity>(this IQueryable<TEntity> query, IOrderBySpecification<TEntity> orderBy)
        where TEntity : class
        {
            Guard.Against<ArgumentNullException>(query == null, "query");
            Guard.Against<ArgumentNullException>(orderBy == null, "orderBy");

            return orderBy.ApplyOrderBy(query);
        }
    }
}
