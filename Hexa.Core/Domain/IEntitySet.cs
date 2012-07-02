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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Hexa.Core.Domain
{
    public interface IEntitySet<TEntity> : IQueryable<TEntity> where TEntity : class
    {
        void Attach(TEntity entity);
        void AddObject(TEntity entity);
        void DeleteObject(TEntity entity);
        void ModifyObject(TEntity entity);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IEntitySet<TEntity> Include(Expression<Func<TEntity, object>> path);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IEntitySet<TEntity> Include(Expression<Func<TEntity, object>> path, Expression<Func<TEntity, bool>> filter);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        IEntitySet<TEntity> Include<S>(Expression<Func<TEntity, object>> path, Expression<Func<TEntity, bool>> filter, Expression<Func<TEntity, S>> orderByExpression);

        IEntitySet<TEntity> Cacheable();
        IEntitySet<TEntity> Cacheable(string cacheRegion);

        IList<TEntity> ExecuteDatabaseQuery(string queryName, IDictionary<string, object> parameters);
        IList<T> ExecuteDatabaseQuery<T>(string queryName, IDictionary<string, object> parameters);
    }
}
