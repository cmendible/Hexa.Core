#region License

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

#endregion

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

    public class NHibernateObjectSet<TEntity> : IEntitySet<TEntity> where TEntity : class
    {
        private readonly ISession _session;
        private IQueryable<TEntity> _set;

        public NHibernateObjectSet(ISession session)
        {
            _session = session;
            _set = _session.Query<TEntity>();
        }

        #region IEntitySet<TEntity> Members

        public void AddObject(TEntity entity)
        {
            _session.Save(entity);
        }

        public void Attach(TEntity entity)
        {
            _session.Lock(entity, LockMode.None);
        }

        public void DeleteObject(TEntity entity)
        {
            _session.Delete(entity);
        }

        public void ModifyObject(TEntity entity)
        {
            if (!_session.Contains(entity))
                _session.Update(entity);
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(TEntity); }
        }

        public Expression Expression
        {
            get { return _set.Expression; }
        }

        public IQueryProvider Provider
        {
            get { return _set.Provider; }
        }

        [SuppressMessage("Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEntitySet<TEntity> Include(Expression<Func<TEntity, object>> path)
        {
            _set = _set.Fetch(path);
            return this;
        }

        [SuppressMessage("Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEntitySet<TEntity> Include(Expression<Func<TEntity, object>> path,
                                           Expression<Func<TEntity, bool>> filter)
        {
            _set = _set.Where(filter).Fetch(path);
            return this;
        }

        [SuppressMessage("Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEntitySet<TEntity> Include<S>(Expression<Func<TEntity, object>> path,
                                              Expression<Func<TEntity, bool>> filter,
                                              Expression<Func<TEntity, S>> orderByExpression)
        {
            _set = _set.Where(filter).OrderByDescending(orderByExpression).Fetch(path);
            return this;
        }

        public IEntitySet<TEntity> Cacheable()
        {
            _set = _set.Cacheable();
            return this;
        }

        public IEntitySet<TEntity> Cacheable(string cacheRegion)
        {
            _set = _set.Cacheable().CacheRegion(cacheRegion);
            return this;
        }

        [SuppressMessage("Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IList<TEntity> ExecuteDatabaseQuery(string queryName, IDictionary<string, object> parameters)
        {
            IQuery query = _session.GetNamedQuery(queryName);
            foreach (var param in parameters)
                query = query.SetParameter(param.Key, param.Value);

            return query.List<TEntity>();
        }

        [SuppressMessage("Microsoft.Design",
            "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IList<T> ExecuteDatabaseQuery<T>(string queryName, IDictionary<string, object> parameters)
        {
            IQuery query = _session.GetNamedQuery(queryName);
            foreach (var param in parameters)
                query = query.SetParameter(param.Key, param.Value);

            return query.List<T>();
        }

        #endregion

        public void Detach(TEntity entity)
        {
            _session.Evict(entity);
        }
    }
}