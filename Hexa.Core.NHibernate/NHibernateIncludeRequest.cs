#region Header

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion Header

namespace Hexa.Core.Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    using NHibernate;
    using NHibernate.Linq;

    public class NHibernateIncludeRequest<TEntity, TInclude> : IIncludeRequest<TEntity, TInclude>
    {
        #region Properties

        public INhFetchRequest<TEntity, TInclude> NhFetchRequest { get; private set; }

        public Type ElementType
        {
            get { return NhFetchRequest.ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return NhFetchRequest.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return NhFetchRequest.Provider; }
        }

        #endregion

        #region Constructors

        public NHibernateIncludeRequest(INhFetchRequest<TEntity, TInclude> nhFetchRequest)
        {
            NhFetchRequest = nhFetchRequest;
        }

        #endregion

        #region Methods

        public IEnumerator<TEntity> GetEnumerator()
        {
            return NhFetchRequest.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return NhFetchRequest.GetEnumerator();
        }

        public IIncludeRequest<TOriginating, TRelated> Include<TOriginating, TRelated>(Expression<Func<TOriginating, TRelated>> path)
        {
            var query = this as IQueryable<TOriginating>;
            var fetch = query.Fetch(path);
            return new NHibernateIncludeRequest<TOriginating, TRelated>(fetch);
        }

        public IIncludeRequest<TOriginating, TRelated> IncludeMany<TOriginating, TRelated>(Expression<Func<TOriginating, IEnumerable<TRelated>>> path)
        {
            var query = this as IQueryable<TOriginating>;
            var fetch = query.FetchMany(path);
            return new NHibernateIncludeRequest<TOriginating, TRelated>(fetch);
        }

        public IIncludeRequest<TQueried, TRelated> ThenInclude<TQueried, TFetch, TRelated>(Expression<Func<TFetch, TRelated>> path)
        {
            var query = this as NHibernateIncludeRequest<TQueried, TFetch>;
            var fetch = query.NhFetchRequest.ThenFetch(path);
            return new NHibernateIncludeRequest<TQueried, TRelated>(fetch);
        }

        public IIncludeRequest<TQueried, TRelated> ThenIncludeMany<TQueried, TFetch, TRelated>(Expression<Func<TFetch, IEnumerable<TRelated>>> path)
        {
            var query = this as NHibernateIncludeRequest<TQueried, TFetch>;
            var fetch = query.NhFetchRequest.ThenFetchMany(path);
            return new NHibernateIncludeRequest<TQueried, TRelated>(fetch);
        }

        #endregion

    }

}