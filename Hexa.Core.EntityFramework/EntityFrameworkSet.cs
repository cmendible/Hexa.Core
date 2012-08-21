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

    using System.Data.Entity;

    public class EntityFrameworkEntitySet<TEntity> : IEntitySet<TEntity>
        where TEntity : class
    {
        #region Fields

        private readonly DbSet<TEntity> dbSet;

        #endregion Fields

        #region Constructors

        public EntityFrameworkEntitySet(DbContext context)
        {
            this.dbSet = context.Set<TEntity>();
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
                return ((IQueryable)this.dbSet).Expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return ((IQueryable)this.dbSet).Provider;
            }
        }

        #endregion Properties

        #region Methods

        public void AddObject(TEntity entity)
        {
            this.dbSet.Add(entity);
        }

        public void Attach(TEntity entity)
        {
            this.dbSet.Attach(entity);
        }

        public IEntitySet<TEntity> Cacheable()
        {
            return this;
        }

        public IEntitySet<TEntity> Cacheable(string cacheRegion)
        {
            return this;
        }

        public void DeleteObject(TEntity entity)
        {
            this.dbSet.Remove(entity);
        }

        public void Detach(TEntity entity)
        {
        }

        public IList<TEntity> ExecuteDatabaseQuery(string queryName, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public IList<T> ExecuteDatabaseQuery<T>(string queryName, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return ((IEnumerable<TEntity>)this.dbSet).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<TEntity>)this.dbSet).GetEnumerator();
        }

        public IEntitySet<TEntity> Include(Expression<Func<TEntity, object>> path)
        {
            throw new NotImplementedException();
        }

        public IEntitySet<TEntity> Include(Expression<Func<TEntity, object>> path,
            Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public IEntitySet<TEntity> Include<S>(Expression<Func<TEntity, object>> path,
            Expression<Func<TEntity, bool>> filter,
            Expression<Func<TEntity, S>> orderByExpression)
        {
            throw new NotImplementedException();
        }

        public void ModifyObject(TEntity entity)
        {
        }

        #endregion Methods
    }
}