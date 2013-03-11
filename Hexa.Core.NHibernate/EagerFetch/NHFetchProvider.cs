using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;

namespace Hexa.Core.Domain
{
    public class NHFetchProvider : IFetchProvider
    {
        public IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
            where TOriginating : class
        {
            var fetch = EagerFetchingExtensionMethods.Fetch(query, relatedObjectSelector);
            return new NHFetchRequest<TOriginating, TRelated>(fetch);
        }

        public IFetchRequest<TOriginating, TRelated> FetchMany<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
            where TOriginating : class
        {
            var fetch = EagerFetchingExtensionMethods.FetchMany(query, relatedObjectSelector);
            return new NHFetchRequest<TOriginating, TRelated>(fetch);
        }

        public IFetchRequest<TQueried, TRelated> ThenFetch<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
            where TQueried : class
        {
            var impl = query as NHFetchRequest<TQueried, TFetch>;
            var fetch = EagerFetchingExtensionMethods.ThenFetch(impl.FetchRequest, relatedObjectSelector);
            return new NHFetchRequest<TQueried, TRelated>(fetch);
        }

        public IFetchRequest<TQueried, TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
            where TQueried : class
        {
            var impl = query as NHFetchRequest<TQueried, TFetch>;
            var fetch = EagerFetchingExtensionMethods.ThenFetchMany(impl.FetchRequest, relatedObjectSelector);
            return new NHFetchRequest<TQueried, TRelated>(fetch);
        }
    }
}
