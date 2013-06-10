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
namespace Hexa.Core.Domain.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using Logging;

    using Moq;

    using NUnit.Framework;

    using Specification;

    /// <summary>
    /// This is a base class for testing repositories. This base class
    /// implement all method defined in IRepository for any type. Inherit
    /// of this class to add test implementation in a concrete repository.
    /// For view all tests correctly in a TestView windows please add column Full Class Name
    /// </summary>
    /// <typeparam name="TEntity">Inner type of respository</typeparam>
    public abstract class RepositoryTestsBase<TEntity>
        where TEntity : class
    {
        #region Properties

        /// <summary>
        /// Specification of filter expression for a particular type
        /// </summary>
        public abstract Expression<Func<TEntity, bool>> FilterExpression
        {
            get;
        }

        /// <summary>
        /// Specification of order by expression for a particular type
        /// </summary>
        public abstract Expression<Func<TEntity, string>> OrderByExpression
        {
            get;
        }

        #endregion Properties

        #region Methods

        public abstract TEntity CreateEntity();

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GenericRepositoryApplyChangesWithNullItemThrowNewArgumentNullException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            // Act
            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            repository.Modify(null);
        }

        [Test]
        public void GenericrRepository_GetPagedElementsAscending_Invoke_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            int pageIndex = 0;
            int pageCount = 1;

            // Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, this.OrderByExpression, true);

            //Assert
            Assert.IsNotNull(entities);
        }

        [Test]
        public virtual void GetAllTest()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);

            // Act
            IEnumerable<TEntity> entities = repository.GetAll();
        }

        public ILoggerFactory GetLoggerFactory()
        {
            var loggerMock = new Mock<ILogger>();
            var loggerFactoryMock = new Mock<ILoggerFactory>();
            loggerFactoryMock.Setup(l => l.Create(It.IsAny<Type>()))
            .Returns(loggerMock.Object);

            return loggerFactoryMock.Object;
        }

        public IUnitOfWork GetUnitOfWork()
        {
            var list = new List<TEntity>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(w => w.Query<TEntity>())
            .Returns(list.AsQueryable());

            unitOfWorkMock.Setup(w => w.Add<TEntity>(It.IsAny<TEntity>()))
            .Callback((TEntity e) => { list.Add(e); });

            unitOfWorkMock.Setup(w => w.Attach<TEntity>(It.IsAny<TEntity>()))
            .Callback((TEntity e) => { list.Add(e); });

            unitOfWorkMock.Setup(w => w.Delete<TEntity>(It.IsAny<TEntity>()))
            .Callback((TEntity e) => { list.Remove(e); });

            return unitOfWorkMock.Object;
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void Repository_AddNullItemThrowArgumentNullException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();
            // Act
            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            repository.Add(null);
        }

        [Test]
        public virtual void Repository_AddValidItem_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            // Act
            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            TEntity item = this.CreateEntity();
            repository.Add(item);
        }

        [Test]
        public virtual void Repository_ApplyChangesValidItem_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            // Act
            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            TEntity item = this.CreateEntity();
            repository.Modify(item);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void Repository_DeleteNullItemThrowNewArgumentNullException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            // Act
            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            repository.Remove(null);
        }

        [Test]
        public virtual void Repository_DeleteValidItemTest()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            // Act
            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            TEntity item = this.CreateEntity();
            repository.Remove(item);
        }

        [Test]
        public void Repository_GetBySpecDirectSpec_Invoke_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            ISpecification<TEntity> specification = new DirectSpecification<TEntity>(this.FilterExpression);

            // Act
            IEnumerable<TEntity> result = repository.GetBySpec(specification);

            //Assert
            Assert.IsNotNull(result);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetBySpecWithNullSpecThrowArgumentNullException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);

            // Act
            repository.GetBySpec(null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetFilteredElementsWithFilterNullAndOrderNotNullThrowNewArgumentNullException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);

            // Act
            IEnumerable<TEntity> entities = repository.GetFilteredElements(null, this.OrderByExpression, false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetFilteredElementsWithNullFilterThrowNewArgumentNullException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);

            // Act
            IEnumerable<TEntity> entities = repository.GetFilteredElements(null);
        }

        [Test]
        public void Repository_GetFilteredElementsWithOrderAscending_Invoke_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);

            // Act
            IEnumerable<TEntity> entities = repository.GetFilteredElements(this.FilterExpression, OrderByExpression,
                                            true);

            //Assert
            Assert.IsNotNull(entities);
        }

        [Test]
        public void Repository_GetFilteredElementsWithOrderDescending_Invoke_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);

            // Act
            IEnumerable<TEntity> entities = repository.GetFilteredElements(this.FilterExpression, OrderByExpression,
                                            false);

            //Assert
            Assert.IsNotNull(entities);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetFilteredElementsWithOrderNullThrowNewArgumentNullException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);

            // Act
            IEnumerable<TEntity> entities = repository.GetFilteredElements<int>(this.FilterExpression, null, false);
        }

        [Test]
        public void Repository_GetFilteredElementsWithPaggingAscending_Invoke_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            int pageIndex = 0;
            int pageCount = 1;

            // Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, this.OrderByExpression,
                                              this.FilterExpression, true);

            //Assert
            Assert.IsNotNull(entities);
        }

        [Test]
        public void Repository_GetFilteredElementsWithPaggingDescending_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            int pageIndex = 0;
            int pageCount = 1;

            // Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, this.OrderByExpression,
                                              this.FilterExpression, false);

            //Assert
            Assert.IsNotNull(entities);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Repository_GetFilteredElementsWithPaggingInvalidPageCountThrowNewArgumentException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            int pageIndex = 1;
            int pageCount = 0;

            // Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, this.OrderByExpression,
                                              this.FilterExpression, false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Repository_GetFilteredElementsWithPaggingInvalidPageIndexThrowNewArgumentException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            int pageIndex = -1;
            int pageCount = 1;

            // Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, this.OrderByExpression,
                                              this.FilterExpression, false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetFilteredElementsWithPaggingNullFilterExpressionThrowNewArgumentNullException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            int pageIndex = 1;
            int pageCount = 1;

            // Act
            Expression<Func<TEntity, bool>> filter = null;
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, this.OrderByExpression,
                                              filter, false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetFilteredElementsWithPaggingNullOrderExpressionThrowNewArgumentNullException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            int pageIndex = 1;
            int pageCount = 1;

            // Act
            PagedElements<TEntity> entities = repository.GetPagedElements<int>(pageIndex, pageCount, null,
                                              this.FilterExpression, false);
        }

        [Test]
        public virtual void Repository_GetFilteredElements_Invoke_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);

            // Act
            IEnumerable<TEntity> entities = repository.GetFilteredElements(this.FilterExpression);

            //Assert
            Assert.IsNotNull(entities);
        }

        [Test]
        public void Repository_GetPagedElementsDescending_Invoke_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            int pageIndex = 0;
            int pageCount = 1;
            bool ascending = false;

            // Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, this.OrderByExpression,
                                              ascending);

            //Assert
            Assert.IsNotNull(entities);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetPagedElementsInvalidOrderByExpressionThrowNewArgumentNullException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            int pageIndex = 0;
            int pageCount = 1;

            // Act
            PagedElements<TEntity> entities = repository.GetPagedElements<int>(pageIndex, pageCount, null, false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Repository_GetPagedElementsInvalidPageCount_ThrowNewArgumentException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            int pageIndex = 0;
            int pageCount = 0;

            // Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, this.OrderByExpression, false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Repository_GetPagedElementsInvalidPageIndexThrowNewArgumentException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            int pageIndex = -1;
            int pageCount = 1;

            // Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, this.OrderByExpression, false);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetPagedElementsWithNullSpecThrowNewArgumentNullException_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();
            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            int pageIndex = 0;
            int pageCount = 10;

            // Act
            ISpecification<TEntity> spec = null;
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, this.OrderByExpression, spec,
                                              false);
        }

        [Test]
        public void Repository_GetPagedElementsWithSpec_Invoke_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);
            ISpecification<TEntity> spec = new TrueSpecification<TEntity>();

            int pageIndex = 0;
            int pageCount = 10;

            // Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, this.OrderByExpression, spec,
                                              false);
        }

        [Test]
        public virtual void Repository_InvokeConstructor_Test()
        {
            // Arrange
            IUnitOfWork unitOfWork = this.GetUnitOfWork();
            ILoggerFactory traceManager = this.GetLoggerFactory();

            // Act
            var repository = new BaseRepository<TEntity>(unitOfWork , traceManager);

            //Assert
            Assert.IsNotNull(repository);
        }

        #endregion Methods
    }
}