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
    using System.Linq;

    /// <summary>
    /// Contract for UnitOfWork pattern. For more
    /// references see http://martinfowler.com/eaaCatalog/unitOfWork.html or
    /// http://msdn.microsoft.com/en-us/magazine/dd882510.aspx
    /// In this solution sample Unit Of Work is implemented out-of-box in
    /// ADO.NET Entity Framework persistence engine. But for academic
    /// purposed and for mantein PI ( Persistence Ignorant ) in Domain
    /// this pattern is implemented.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        #region Methods

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void Add<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Attaches the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void Attach<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Commit all changes made in  a container.
        /// </summary>
        /// <remarks>
        /// If entity have fixed properties and optimistic concurrency problem exists
        /// exception is thrown
        /// </remarks>
        void Commit();

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void Delete<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Modifies the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        void Modify<TEntity>(TEntity entity)
            where TEntity : class;

        /// <summary>
        /// Returns an IQueryable
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        IQueryable<TEntity> Query<TEntity>()
            where TEntity : class;

        /// <summary>
        /// Starts this instance.
        /// </summary>
        void Start();

        #endregion Methods
    }
}