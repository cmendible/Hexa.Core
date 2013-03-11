using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Entity;

namespace Hexa.Core.Domain
{
    public class EFFetchProvider : IFetchProvider
    {
        public IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
            where TOriginating : class
        {
            var fetch = DbExtensions.Include(query, relatedObjectSelector);
            return new EFFetchRequest<TOriginating, TRelated>(fetch);
        }

        public IFetchRequest<TOriginating, TRelated> FetchMany<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
            where TOriginating : class
        {
            var fetch = DbExtensions.Include(query, relatedObjectSelector);
            return new EFFetchRequest<TOriginating, TRelated>(fetch);
        }

        public IFetchRequest<TQueried, TRelated> ThenFetch<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
             where TQueried : class
        {
            throw new NotImplementedException();
        }

        public IFetchRequest<TQueried, TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
             where TQueried : class
        {
            throw new NotImplementedException();
        }
    }
}
