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
    public class BaseRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        #region Fields

        private readonly IUnitOfWork _context;
        private readonly ILogger _logger;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default constructor for GenericRepository
        /// </summary>
        /// <param name="traceManager">Trace Manager dependency</param>
        /// <param name="context">A context for this repository</param>
        public BaseRepository(ILoggerFactory loggerFactory)
        {
            Guard.IsNotNull(loggerFactory, "loggerFactory");

            IUnitOfWork context = UnitOfWorkScope.Current;

            // check preconditions
            Guard.IsNotNull(context, "No context in scope.");

            // set internal values
            this._context = context;
            this._logger = loggerFactory.Create(GetType());
            this._logger.Debug(string.Format(CultureInfo.InvariantCulture, "Created repository for type: {0}", typeof(TEntity).Name));
        }

        #endregion Constructors

        #region Properties

        protected ILogger Logger
        {
            get
            {
                return this._logger;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="item"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        public virtual void Add(TEntity item)
        {
            // check item
            Guard.IsNotNull(item, "item");

            // add object to IObjectSet for this type
            (this._context.CreateSet<TEntity>()).AddObject(item);

            this._logger.Debug(string.Format(CultureInfo.InvariantCulture, "Added a {0} entity", typeof(TEntity).Name));
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="item"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        public void Attach(TEntity item)
        {
            Guard.IsNotNull(item, "item");

            this._context.CreateSet<TEntity>().Attach(item);

            this._logger.Debug(string.Format(CultureInfo.InvariantCulture, "Attached {0} to context", typeof(TEntity).Name));
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <returns><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></returns>
        public IEnumerable<TEntity> GetAll()
        {
            this._logger.Debug(string.Format(CultureInfo.InvariantCulture, "Getting all {0}", typeof(TEntity).Name));

            // Create IObjectSet and perform query
            return (this._context.CreateSet<TEntity>()).AsEnumerable();
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="specification"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <returns><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></returns>
        public IEnumerable<TEntity> GetBySpec(ISpecification<TEntity> specification)
        {
            Guard.IsNotNull(specification, "specification");

            this._logger.Debug(string.Format(CultureInfo.InvariantCulture, "Getting {0} by specification", typeof(TEntity).Name));

            return (this._context.CreateSet<TEntity>()
                    .Where(specification.SatisfiedBy())
                    .AsEnumerable());
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

            this._logger.Debug(string.Format(CultureInfo.InvariantCulture, "Getting filtered elements {0} with filer: {1}", typeof(TEntity).Name, filter.ToString()));

            // Create IObjectSet and perform query
            return this._context.CreateSet<TEntity>()
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
        public IEnumerable<TEntity> GetFilteredElements<S>(Expression<Func<TEntity, bool>> filter,
            Expression<Func<TEntity, S>> orderByExpression,
            bool ascending)
        {
            // Checking query arguments
            Guard.IsNotNull(filter, "filter");
            Guard.IsNotNull(orderByExpression, "orderByExpression");

            this._logger.Debug(string.Format(CultureInfo.InvariantCulture, "Getting filtered elements {0} with filter: {1}", typeof(TEntity).Name, filter.ToString()));

            // Create IObjectSet for this type and perform query
            IEntitySet<TEntity> objectSet = this._context.CreateSet<TEntity>();

            return (ascending)
                   ? objectSet
                   .Where(filter)
                   .OrderBy(orderByExpression)
                   .ToList()
                   : objectSet
                   .Where(filter)
                   .OrderByDescending(orderByExpression)
                   .ToList();
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="pageIndex"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <param name="pageCount"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <param name="orderByExpression"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <param name="ascending"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <returns><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></returns>
        public PagedElements<TEntity> GetPagedElements<S>(int pageIndex, int pageCount,
            Expression<Func<TEntity, S>>
            orderByExpression, bool ascending)
        {
            // checking arguments for this query
            Guard.Against<ArgumentException>(pageIndex < 0, "pageIndex");
            Guard.Against<ArgumentException>(pageCount <= 0, "pageCount");
            Guard.IsNotNull(orderByExpression, "orderByExpression");

            this._logger.Debug(
                string.Format(CultureInfo.InvariantCulture,
                              "Getting paged elements {0}, pageIndex: {1}, pageCount {2}, oderBy {3}",
                              typeof(TEntity).Name, pageIndex, pageCount, orderByExpression.ToString()));

            // Create associated IObjectSet and perform query

            IEntitySet<TEntity> objectSet = this._context.CreateSet<TEntity>();

            int total = objectSet.Count();

            return (ascending)
                   ? new PagedElements<TEntity>(
                       objectSet.OrderBy(orderByExpression)
                       .Skip(pageIndex*pageCount)
                       .Take(pageCount)
                       .ToList(), total)
                   : new PagedElements<TEntity>(
                       objectSet.OrderByDescending(orderByExpression)
                       .Skip(pageIndex*pageCount)
                       .Take(pageCount)
                       .ToList(), total);
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <typeparam name="S"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></typeparam>
        /// <param name="pageIndex"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <param name="pageCount"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <param name="orderByExpression"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <param name="specification"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <param name="ascending"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        /// <returns><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></returns>
        public PagedElements<TEntity> GetPagedElements<S>(int pageIndex, int pageCount,
            Expression<Func<TEntity, S>> orderByExpression,
            ISpecification<TEntity> specification, bool ascending)
        {
            // checking arguments for this query
            Guard.Against<ArgumentException>(pageIndex < 0, "pageIndex");
            Guard.Against<ArgumentException>(pageCount <= 0, "pageCount");
            Guard.IsNotNull(orderByExpression, "orderByExpression");
            Guard.IsNotNull(specification, "specification");

            this._logger.Debug(
                string.Format(CultureInfo.InvariantCulture,
                              "Getting paged elements {0}, pageIndex: {1}, pageCount {2}, oderBy {3}",
                              typeof(TEntity).Name, pageIndex, pageCount, orderByExpression.ToString()));

            // Create associated IObjectSet and perform query

            IEntitySet<TEntity> objectSet = this._context.CreateSet<TEntity>();

            IQueryable<TEntity> query = objectSet.Where(specification.SatisfiedBy());
            int total = query.Count();

            return (ascending)
                   ? new PagedElements<TEntity>(
                       query.OrderBy(orderByExpression)
                       .Skip(pageIndex*pageCount)
                       .Take(pageCount)
                       .ToList(), total)
                   : new PagedElements<TEntity>(
                       query.OrderByDescending(orderByExpression)
                       .Skip(pageIndex*pageCount)
                       .Take(pageCount)
                       .ToList(), total);
        }

        public PagedElements<TEntity> GetPagedElements<S>(int pageIndex, int pageCount,
            Expression<Func<TEntity, S>> orderByExpression,
            Expression<Func<TEntity, bool>> filter, bool ascending)
        {
            // checking arguments for this query
            Guard.Against<ArgumentException>(pageIndex < 0, "pageIndex");
            Guard.Against<ArgumentException>(pageCount <= 0, "pageCount");
            Guard.IsNotNull(orderByExpression, "orderByExpression");
            Guard.IsNotNull(filter, "filter");

            this._logger.Debug(
                string.Format(CultureInfo.InvariantCulture,
                              "Getting paged elements {0}, pageIndex: {1}, pageCount {2}, oderBy {3}",
                              typeof(TEntity).Name, pageIndex, pageCount, orderByExpression.ToString()));

            // Create associated IObjectSet and perform query

            IEntitySet<TEntity> objectSet = this._context.CreateSet<TEntity>();

            IQueryable<TEntity> query = objectSet.Where(filter);
            int total = query.Count();

            return (ascending)
                   ? new PagedElements<TEntity>(
                       query.OrderBy(orderByExpression)
                       .Skip(pageIndex*pageCount)
                       .Take(pageCount)
                       .ToList(), total)
                   : new PagedElements<TEntity>(
                       query.OrderByDescending(orderByExpression)
                       .Skip(pageIndex*pageCount)
                       .Take(pageCount)
                       .ToList(), total);
        }

        public PagedElements<TEntity> GetPagedElements(int pageIndex, int pageCount,
            IOrderBySpecification<TEntity> orderBySpecification,
            ISpecification<TEntity> specification)
        {
            // checking arguments for this query
            Guard.Against<ArgumentException>(pageIndex < 0, "pageIndex");
            Guard.Against<ArgumentException>(pageCount <= 0, "pageCount");
            Guard.IsNotNull(orderBySpecification, "orderBySpecification");
            Guard.IsNotNull(specification, "specification");

            this._logger.Debug(
                string.Format(CultureInfo.InvariantCulture,
                              "Getting paged elements {0}, pageIndex: {1}, pageCount {2}, oderBy {3}",
                              typeof(TEntity).Name, pageIndex, pageCount, orderBySpecification.ToString()));

            // Create associated IObjectSet and perform query
            IEntitySet<TEntity> objectSet = this._context.CreateSet<TEntity>();

            IQueryable<TEntity> query = objectSet.Where(specification.SatisfiedBy());
            int total = query.Count();

            return new PagedElements<TEntity>(
                       query
                       .OrderBySpecification(orderBySpecification)
                       .Skip(pageIndex*pageCount)
                       .Take(pageCount)
                       .ToList(), total);
        }

        public PagedElements<TEntity> GetPagedElements(int pageIndex, int pageCount,
            IOrderBySpecification<TEntity> orderBySpecification,
            Expression<Func<TEntity, bool>> filter)
        {
            // checking arguments for this query
            Guard.Against<ArgumentException>(pageIndex < 0, "pageIndex");
            Guard.Against<ArgumentException>(pageCount <= 0, "pageCount");
            Guard.IsNotNull(orderBySpecification, "orderBySpecification");
            Guard.IsNotNull(filter, "filter");

            this._logger.Debug(
                string.Format(CultureInfo.InvariantCulture,
                              "Getting paged elements {0}, pageIndex: {1}, pageCount {2}, oderBy {3}",
                              typeof(TEntity).Name, pageIndex, pageCount, orderBySpecification.ToString()));

            // Create associated IObjectSet and perform query
            IEntitySet<TEntity> objectSet = this._context.CreateSet<TEntity>();

            IQueryable<TEntity> query = objectSet.Where(filter);
            int total = query.Count();

            return new PagedElements<TEntity>(
                       query
                       .OrderBySpecification(orderBySpecification)
                       .Skip(pageIndex*pageCount)
                       .Take(pageCount)
                       .ToList(), total);
        }

        public virtual void Modify(TEntity item)
        {
            // check arguments
            Guard.IsNotNull(item, "item");

            // apply changes for item object
            this._context.CreateSet<TEntity>().ModifyObject(item);

            this._logger.Info(string.Format(CultureInfo.InvariantCulture, "Applied changes to: {0}", typeof(TEntity).Name));
        }

        /// <summary>
        /// <see cref="Hexa.Core.Domain.IRepository{TEntity}"/>
        /// </summary>
        /// <param name="item"><see cref="Hexa.Core.Domain.IRepository{TEntity}"/></param>
        public virtual void Remove(TEntity item)
        {
            // check item
            Guard.IsNotNull(item, "item");

            IEntitySet<TEntity> objectSet = (this._context.CreateSet<TEntity>());

            // Attach object to context and delete this
            // this is valid only if T is a type in model
            objectSet.Attach(item);

            // delete object to IObjectSet for this type
            objectSet.DeleteObject(item);

            this._logger.Debug(string.Format(CultureInfo.InvariantCulture, "Deleted a {0} entity", typeof(TEntity).Name));
        }

        #endregion Methods
    }
}