//----------------------------------------------------------------------------------------------
// <copyright file="BaseRepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;

    using Logging;

    using Specification;

    /// <summary>
    /// Default base class for repostories. This generic repository
    /// is a default implementation of <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
    /// and your specific repositories can inherit from this base class so automatically will get default implementation.
    /// IMPORTANT: Using this Base Repository class IS NOT mandatory. It is just a useful base class:
    /// You could also decide that you do not want to use this base Repository class, because sometimes you don't want a
    /// specific Repository getting all these features and it might be wrong for a specific Repository.
    /// For instance, you could want just read-only data methods for your Repository, etc.
    /// in that case, just simply do not use this base class on your Repository.
    /// </summary>
    /// <typeparam name="TEntity">Type of elements in repostory</typeparam>
    public abstract class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        private readonly ILogger logger;

        /// <summary>
        /// Default constructor for GenericRepository
        /// </summary>
        /// <param name="traceManager">Trace Manager dependency</param>
        /// <param name="context">A context for this repository</param>
        protected BaseRepository()
        {
            this.logger = LoggerManager.GetLogger(GetType());
            this.logger.Debug(string.Format(CultureInfo.InvariantCulture, "Created repository for type: {0}", typeof(TEntity).Name));
        }

        protected ILogger Logger
        {
            get
            {
                return this.logger;
            }
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="entity"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        public virtual void Add(TEntity entity)
        {
            Guard.IsNotNull(entity, "entity");

            this.InternalAdd(entity);

            this.logger.Debug(string.Format(CultureInfo.InvariantCulture, "Added a {0} entity", typeof(TEntity).Name));
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="entity"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        public void Attach(TEntity entity)
        {
            Guard.IsNotNull(entity, "entity");

            this.InternalAttach(entity);

            this.logger.Debug(string.Format(CultureInfo.InvariantCulture, "Attached {0} to context", typeof(TEntity).Name));
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <returns><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></returns>
        public IEnumerable<TEntity> GetAll()
        {
            return this.Query().ToList();
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="specification"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <returns><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></returns>
        public IEnumerable<TEntity> GetBySpec(ISpecification<TEntity> specification)
        {
            Guard.IsNotNull(specification, "specification");

            this.logger.Debug(string.Format(CultureInfo.InvariantCulture, "Getting {0} by specification", typeof(TEntity).Name));

            return (this.Query()
                    .Where(specification.SatisfiedBy()))
                    .ToList();
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="filter"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <returns><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></returns>b
        public IEnumerable<TEntity> GetFilteredElements(Expression<Func<TEntity, bool>> filter)
        {
            // checking query arguments
            Guard.IsNotNull(filter, "filter");

            this.logger.Debug(string.Format(CultureInfo.InvariantCulture, "Getting filtered elements {0} with filer: {1}", typeof(TEntity).Name, filter.ToString()));

            // Create IObjectSet and perform query
            return this.Query()
                   .Where(filter)
                   .ToList();
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="filter"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <param name="orderByExpression"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <param name="ascending"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <returns><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></returns>
        public IEnumerable<TEntity> GetFilteredElements<S>(
            Expression<Func<TEntity, bool>> filter,
            Expression<Func<TEntity, S>> orderByExpression,
            bool ascending = true)
        {
            // Checking query arguments
            Guard.IsNotNull(filter, "filter");
            Guard.IsNotNull(orderByExpression, "orderByExpression");

            this.logger.Debug(string.Format(CultureInfo.InvariantCulture, "Getting filtered elements {0} with filter: {1}", typeof(TEntity).Name, filter.ToString()));

            // Create IObjectSet for this type and perform query
            var objectSet = this.Query();

            return ascending
                   ? objectSet
                   .Where(filter)
                   .OrderBy(orderByExpression)
                   .ToList()
                   : objectSet
                   .Where(filter)
                   .OrderByDescending(orderByExpression)
                   .ToList();
        }

        public PagedElements<TEntity> GetPagedElements<S>(
            int pageIndex,
            int pageSize,
            Expression<Func<TEntity, bool>> filter, 
            Expression<Func<TEntity, S>> orderByExpression,
            bool ascending = true)
        {
            // checking arguments for this query
            Guard.Against<ArgumentException>(pageIndex < 0, "pageIndex");
            Guard.Against<ArgumentException>(pageSize <= 0, "pageSize");
            Guard.IsNotNull(orderByExpression, "orderByExpression");
            Guard.IsNotNull(filter, "filter");

            this.logger.Debug(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Getting paged elements {0}, pageIndex: {1}, pageSize {2}, oderBy {3}",
                    typeof(TEntity).Name,
                    pageIndex,
                    pageSize,
                    orderByExpression.ToString()));

            var objectSet = this.Query();

            IQueryable<TEntity> query = objectSet.Where(filter);
            int total = query.Count();

            return ascending
                   ? new PagedElements<TEntity>(
                       query.OrderBy(orderByExpression)
                       .Skip(pageIndex * pageSize)
                       .Take(pageSize)
                       .ToList(),
                       total)
                   : new PagedElements<TEntity>(
                       query.OrderByDescending(orderByExpression)
                       .Skip(pageIndex * pageSize)
                       .Take(pageSize)
                       .ToList(),
                       total);
        }

        public PagedElements<TEntity> GetPagedElements(
            int pageIndex,
            int pageSize,
            ISpecification<TEntity> specification,
            IOrderBySpecification<TEntity> orderBySpecification
            )
        {
            // checking arguments for this query
            Guard.Against<ArgumentException>(pageIndex < 0, "pageIndex");
            Guard.Against<ArgumentException>(pageSize <= 0, "pageSize");
            Guard.IsNotNull(orderBySpecification, "orderBySpecification");
            Guard.IsNotNull(specification, "specification");

            this.logger.Debug(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Getting paged elements {0}, pageIndex: {1}, pageSize {2}, oderBy {3}",
                    typeof(TEntity).Name,
                    pageIndex,
                    pageSize,
                    orderBySpecification.ToString()));

            // Create associated IObjectSet and perform query
            var objectSet = this.Query();

            IQueryable<TEntity> query = objectSet.Where(specification.SatisfiedBy());
            int total = query.Count();

            return new PagedElements<TEntity>(
                       query
                       .OrderBySpecification(orderBySpecification)
                       .Skip(pageIndex * pageSize)
                       .Take(pageSize)
                       .ToList(),
                       total);
        }

        public virtual void Modify(TEntity entity)
        {
            Guard.IsNotNull(entity, "entity");

            this.InternalModify(entity);

            this.logger.Info(string.Format(CultureInfo.InvariantCulture, "Applied changes to: {0}", typeof(TEntity).Name));
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="entity"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        public virtual void Remove(TEntity entity)
        {
            // check entity
            Guard.IsNotNull(entity, "entity");

            this.InternalRemove(entity);

            this.logger.Debug(string.Format(CultureInfo.InvariantCulture, "Deleted a {0} entity", typeof(TEntity).Name));
        }

        protected abstract void InternalAdd(TEntity entity);

        protected abstract void InternalAttach(TEntity entity);

        protected abstract void InternalModify(TEntity entity);

        protected abstract void InternalRemove(TEntity entity);

        protected abstract IQueryable<TEntity> Query();

    }
}