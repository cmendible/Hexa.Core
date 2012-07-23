// ===================================================================================
// Microsoft Developer & Platform Evangelism
// ===================================================================================
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ===================================================================================
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
// This code is released under the terms of the MS-LPL license,
// http://microsoftnlayerapp.codeplex.com/license
// ===================================================================================
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    /// <summary>
    /// In memory IObjectSet. This class is intended only
    /// for testing purposes.
    /// </summary>
    /// <typeparam name="TEntity">Type of elements in objectSet</typeparam>
    public sealed class MemorySet<TEntity> : IEntitySet<TEntity>
        where TEntity : class
    {
        #region Fields

        private readonly List<string> _IncludePaths;
        private readonly List<TEntity> _InnerList;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="innerList">A List{T} with inner values of this IObjectSet</param>
        public MemorySet(List<TEntity> innerList)
        {
            if (innerList == null)
            {
                throw new ArgumentNullException("innerList");
            }

            this._InnerList = innerList;
            this._IncludePaths = new List<string>();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// <see cref="System.Linq.IQueryable{T}"/>
        /// </summary>
        public Type ElementType
        {
            get
            {
                return typeof(TEntity);
            }
        }

        /// <summary>
        /// <see cref="System.Linq.IQueryable{T}"/>
        /// </summary>
        public Expression Expression
        {
            get
            {
                return this._InnerList.AsQueryable().Expression;
            }
        }

        /// <summary>
        /// <see cref="System.Linq.IQueryable{T}"/>
        /// </summary>
        public IQueryProvider Provider
        {
            get
            {
                return this._InnerList.AsQueryable().Provider;
            }
        }

        #endregion Properties

        #region Methods

        public void AddObject(TEntity entity)
        {
            if (entity != null)
            {
                this._InnerList.Add(entity);
            }
        }

        public void Attach(TEntity entity)
        {
            if (entity != null && !this._InnerList.Contains(entity))
            {
                this._InnerList.Add(entity);
            }
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
            if (entity != null)
            {
                this._InnerList.Remove(entity);
            }
        }

        public void Detach(TEntity entity)
        {
            if (entity != null)
            {
                this._InnerList.Remove(entity);
            }
        }

        [SuppressMessage("Microsoft.Design",
                         "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IList<TEntity> ExecuteDatabaseQuery(string queryName, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Design",
                         "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IList<T> ExecuteDatabaseQuery<T>(string queryName, IDictionary<string, object> parameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// <see cref="System.Collections.IEnumerable.GetEnumerator"/>
        /// </summary>
        /// <returns><see cref="System.Collections.IEnumerable.GetEnumerator"/></returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            foreach (TEntity item in this._InnerList)
            {
                yield return item;
            }
        }

        /// <summary>
        /// <see cref="System.Collections.IEnumerable.GetEnumerator"/>
        /// </summary>
        /// <returns><see cref="System.Collections.IEnumerable.GetEnumerator"/></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <summary>
        /// Include path in query objects
        /// </summary>
        /// <param name="path">Path to include</param>
        /// <returns>IObjectSet with include path</returns>
        public MemorySet<TEntity> Include(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            this._IncludePaths.Add(path);

            return this;
        }

        [SuppressMessage("Microsoft.Design",
                         "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEntitySet<TEntity> Include(Expression<Func<TEntity, object>> path)
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Design",
                         "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public IEntitySet<TEntity> Include(Expression<Func<TEntity, object>> path,
            Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        [SuppressMessage("Microsoft.Design",
                         "CA1006:DoNotNestGenericTypesInMemberSignatures")]
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