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

        public static IFetchRequest<TOriginating, TRelated> FetchMany<TOriginating, TRelated>(
            this IQueryable<TOriginating> query,
            Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
            where TOriginating : class
        {
            return FetchingProvider().FetchMany(query, relatedObjectSelector);
        }

        public static IFetchRequest<TQueried, TRelated> ThenFetch<TQueried, TFetch, TRelated>(
            this IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
            where TQueried : class
        {
            return FetchingProvider().ThenFetch(query, relatedObjectSelector);
        }

        public static IFetchRequest<TQueried, TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(
            this IFetchRequest<TQueried, TFetch> query,
            Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
            where TQueried : class
        {
            return FetchingProvider().ThenFetchMany(query, relatedObjectSelector);
        }
    }
}