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

    public class NHibernateEntitySet<TEntity> : IEntitySet<TEntity>
        where TEntity : class
    {
        #region Fields

        private readonly ISession _session;

        private IQueryable<TEntity> _set;

        #endregion Fields

        #region Constructors

        public NHibernateEntitySet(ISession session)
        {
            this._session = session;
            this._set = this._session.Query<TEntity>();
        }

        #endregion Constructors

        #region Properties

        public Type ElementType
        {
            get
            {
                return typeof(TEntity);
            }
        }

        public Expression Expression
        {
            get
            {
                return this._set.Expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this._set.Provider;
            }
        }

        #endregion Properties

        #region Methods

        public void AddObject(TEntity entity)
        {
            this._session.Save(entity);
        }

        public void Attach(TEntity entity)
        {
            this._session.Lock(entity, LockMode.None);
        }

        public IEntitySet<TEntity> Cacheable()
        {
            this._set = this._set.Cacheable();
            return this;
        }

        public IEntitySet<TEntity> Cacheable(string cacheRegion)
        {
            this._set = this._set.Cacheable().CacheRegion(cacheRegion);
            return this;
        }

        public void DeleteObject(TEntity entity)
        {
            this._session.Delete(entity);
        }

        public void Detach(TEntity entity)
        {
            this._session.Evict(entity);
        }

        public IList<TEntity> ExecuteDatabaseQuery(string queryName, IDictionary<string, object> parameters)
        {
            return this.ExecuteDatabaseQuery<TEntity>(queryName, parameters);
        }

        public IList<T> ExecuteDatabaseQuery<T>(string queryName, IDictionary<string, object> parameters)
        {
            IQuery query = this._session.GetNamedQuery(queryName);
            foreach (var param in parameters)
            {
                query = query.SetParameter(param.Key, param.Value);
            }

            return query.List<T>();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return this._set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._set.GetEnumerator();
        }

        public void ModifyObject(TEntity entity)
        {
            if (!_session.Contains(entity))
            {
                this._session.Update(entity);
            }
        }

        public IIncludeRequest<TEntity, TRelated> Include<TRelated>(Expression<Func<TEntity, TRelated>> relatedObjectSelector)
        {
            return new NHibernateIncludeRequest<TEntity, TRelated>(this._set.Fetch(relatedObjectSelector));
        }

        public IIncludeRequest<TEntity, TRelated> IncludeMany<TRelated>(Expression<Func<TEntity, IEnumerable<TRelated>>> relatedObjectSelector)
        {
            return new NHibernateIncludeRequest<TEntity, TRelated>(this._set.FetchMany(relatedObjectSelector));
        }

        public IIncludeRequest<TEntity, TRelated> Include<TRelated>(Expression<Func<TEntity, TRelated>> path,
           Expression<Func<TEntity, bool>> filter)
        {
            this._set = this._set.Where(filter).Fetch(path);
            return new NHibernateIncludeRequest<TEntity, TRelated>((INhFetchRequest<TEntity, TRelated>)this._set);
        }

        public IIncludeRequest<TEntity, TRelated> Include<TRelated, S>(Expression<Func<TEntity, TRelated>> path,
            Expression<Func<TEntity, bool>> filter,
            Expression<Func<TEntity, S>> orderByExpression)
        {
            this._set = this._set.Where(filter).Fetch(path);
            return new NHibernateIncludeRequest<TEntity, TRelated>((INhFetchRequest<TEntity, TRelated>)this._set);
        }

        public IIncludeRequest<TEntity, TRelated> IncludeMany<TRelated>(Expression<Func<TEntity, IEnumerable<TRelated>>> path,
           Expression<Func<TEntity, bool>> filter)
        {
            this._set = this._set.Where(filter).FetchMany(path);
            return new NHibernateIncludeRequest<TEntity, TRelated>((INhFetchRequest<TEntity, TRelated>)this._set);
        }

        public IIncludeRequest<TEntity, TRelated> IncludeMany<TRelated, S>(Expression<Func<TEntity, IEnumerable<TRelated>>> path,
            Expression<Func<TEntity, bool>> filter,
            Expression<Func<TEntity, S>> orderByExpression)
        {
            this._set = this._set.Where(filter).FetchMany(path);
            return new NHibernateIncludeRequest<TEntity, TRelated>((INhFetchRequest<TEntity, TRelated>)this._set);
        }

        #endregion Methods
    }

}