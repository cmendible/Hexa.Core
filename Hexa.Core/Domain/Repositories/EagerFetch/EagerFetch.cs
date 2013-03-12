using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Hexa.Core.Domain
{
    public static class EagerFetch
    {
        public static Func<IFetchProvider> FetchingProvider;

        public static IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(
            this IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
            where TOriginating : class
        {
            return FetchingProvider().Fetch(query, relatedObjectSelector);
        }
    }
}