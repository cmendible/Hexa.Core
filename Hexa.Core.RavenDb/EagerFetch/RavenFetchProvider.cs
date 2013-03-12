using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Raven.Client;

namespace Hexa.Core.Domain
{
    public class RavenFetchProvider : IFetchProvider
    {
        public IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
            where TOriginating : class
        {
            return new RavenFetchRequest<TOriginating, TRelated>(query);
        }

        //public IFetchRequest<TOriginating, TRelated> FetchMany<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, IEnumerable<TRelated>>> relatedObjectSelector)
        //    where TOriginating : class
        //{
        //    return new RavenFetchRequest<TOriginating, TRelated>(query);
        //}

        //public IFetchRequest<TQueried, TRelated> ThenFetch<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, TRelated>> relatedObjectSelector)
        //    where TQueried : class
        //{
        //    return new RavenFetchRequest<TQueried, TRelated>(query);
        //}

        //public IFetchRequest<TQueried, TRelated> ThenFetchMany<TQueried, TFetch, TRelated>(IFetchRequest<TQueried, TFetch> query, Expression<Func<TFetch, IEnumerable<TRelated>>> relatedObjectSelector)
        //    where TQueried : class
        //{
        //    return new RavenFetchRequest<TQueried, TRelated>(query);
        //}
    }
}
