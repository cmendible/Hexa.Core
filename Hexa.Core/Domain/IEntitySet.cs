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
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IEntitySet<TEntity> : IQueryable<TEntity>
        where TEntity : class
    {
        #region Methods

        void AddObject(TEntity entity);

        void Attach(TEntity entity);

        IEntitySet<TEntity> Cacheable();

        IEntitySet<TEntity> Cacheable(string cacheRegion);

        void DeleteObject(TEntity entity);

        IList<TEntity> ExecuteDatabaseQuery(string queryName, IDictionary<string, object> parameters);

        IList<T> ExecuteDatabaseQuery<T>(string queryName, IDictionary<string, object> parameters);

        IEntitySet<TEntity> Include(Expression<Func<TEntity, object>> path);

        IEntitySet<TEntity> Include(Expression<Func<TEntity, object>> path, Expression<Func<TEntity, bool>> filter);

        IEntitySet<TEntity> Include<S>(
            Expression<Func<TEntity, object>> path, 
            Expression<Func<TEntity, bool>> filter,
            Expression<Func<TEntity, S>> orderByExpression);

        void ModifyObject(TEntity entity);

        #endregion Methods
    }
}
