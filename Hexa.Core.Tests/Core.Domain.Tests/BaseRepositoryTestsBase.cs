//===================================================================================
// Microsoft Developer & Platform Evangelism
//=================================================================================== 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
// This code is released under the terms of the MS-LPL license, 
// http://microsoftnlayerapp.codeplex.com/license
//===================================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

using MbUnit.Framework;
using Rhino.Mocks;
using SL = Microsoft.Practices.ServiceLocation;

using Hexa.Core;
using Hexa.Core.Logging;
using Hexa.Core.Domain.Specification;
using Hexa.Core.Tests;

namespace Hexa.Core.Domain.Tests
{
    /// <summary>
    /// This is a base class for testing repositories. This base class
    /// implement all method defined in IRepository for any type. Inherit
    /// of this class to add test implementation in a concrete repository.
    /// For view all tests correctly in a TestView windows please add column Full Class Name
    /// </summary>
    /// <typeparam name="TEntity">Inner type of respository</typeparam>
    [TestFixture]
    public abstract class RepositoryTestsBase<TEntity>
        where TEntity : class
    {

        #region Virtual and abstract elements for inheritance unit tests

        /// <summary>
        /// Specification of filter expression for a particular type
        /// </summary>
        public abstract Expression<Func<TEntity, bool>> FilterExpression { get; }

        /// <summary>
        /// Specification of order by expression for a particular type
        /// </summary>
        public abstract Expression<Func<TEntity, string>> OrderByExpression { get; }

        public abstract TEntity CreateEntity();

        #endregion

        #region Test Helper for testing purposes

        public ILoggerFactory GetLoggerFactory()
        {
            return ServiceLocator.GetInstance<ILoggerFactory>();
        }

        public IQueryableUnitOfWork GetUnitOfWork()
        {
            return UnitOfWorkContext.Current as IQueryableUnitOfWork;
        }

        #endregion

        #region Test Methods

        [SetUp]
        public void Setup()
        {
            UnitOfWorkContext.Start();
        }

        [TearDown]
        public void TearDown()
        {
            UnitOfWorkContext.RemoveCurrent();
        }

        [Test()]
        public virtual void Repository_InvokeConstructor_Test()
        {
            //Arrange
            var unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            //Act
            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            IUnitOfWork actual = repository.UnitOfWork;

            //Assret
            Assert.AreEqual(unitOfWork, actual);
        }
        
        [Test()]
        public virtual void Repository_AddValidItem_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            //Act
            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            TEntity item = CreateEntity();
            repository.Add(item);
        }
       
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void Repository_AddNullItemThrowArgumentNullException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();
            //Act
            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            repository.Add(null);
        }
        
        [Test()]
        public virtual void Repository_DeleteValidItemTest()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            //Act
            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            TEntity item = CreateEntity();
            repository.Remove(item);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void Repository_DeleteNullItemThrowNewArgumentNullException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            //Act
            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            repository.Remove(null);
        }

        [Test()]
        public virtual void Repository_ApplyChangesValidItem_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            //Act
            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            TEntity item = CreateEntity();
            repository.Modify(item);
            
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public virtual void GenericRepositoryApplyChangesWithNullItemThrowNewArgumentNullException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            //Act
            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            repository.Modify((TEntity)null);
        }

        [Test()]
        public virtual void GetAllTest()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);

            //Act
            IEnumerable<TEntity> entities = repository.GetAll();
        }

        [Test()]
        public virtual void Repository_GetFilteredElements_Invoke_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);

            //Act
            IEnumerable<TEntity> entities = repository.GetFilteredElements(FilterExpression);

            //Assert
            Assert.IsNotNull(entities);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetFilteredElementsWithNullFilterThrowNewArgumentNullException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);

            //Act
            IEnumerable<TEntity> entities = repository.GetFilteredElements(null);
        }

        [Test()]
        public void Repository_GetFilteredElementsWithOrderAscending_Invoke_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);

            //Act
            IEnumerable<TEntity> entities = repository.GetFilteredElements<string>(FilterExpression, OrderByExpression,true);

            //Assert
            Assert.IsNotNull(entities);
        }
        
        [Test()]
        public void Repository_GetFilteredElementsWithOrderDescending_Invoke_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);

            //Act
            IEnumerable<TEntity> entities = repository.GetFilteredElements<string>(FilterExpression, OrderByExpression, false);

            //Assert
            Assert.IsNotNull(entities);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetFilteredElementsWithOrderNullThrowNewArgumentNullException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);

            //Act
            IEnumerable<TEntity> entities = repository.GetFilteredElements<int>(FilterExpression, null,false);
        }
        
        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetFilteredElementsWithFilterNullAndOrderNotNullThrowNewArgumentNullException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);

            //Act
            IEnumerable<TEntity> entities = repository.GetFilteredElements<string>(null, OrderByExpression, false);
        }
       
        [Test()]
        public void Repository_GetFilteredElementsWithPaggingAscending_Invoke_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            int pageIndex = 0;
            int pageCount = 1;

            //Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, OrderByExpression, FilterExpression, true);

            //Assert
            Assert.IsNotNull(entities);
        }

        [Test()]
        public void Repository_GetFilteredElementsWithPaggingDescending_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            int pageIndex = 0;
            int pageCount = 1;

            //Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, OrderByExpression, FilterExpression, false);

            //Assert
            Assert.IsNotNull(entities);

        }

        [Test()]
        [ExpectedException(typeof(ArgumentException))]
        public void Repository_GetFilteredElementsWithPaggingInvalidPageIndexThrowNewArgumentException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            int pageIndex = -1;
            int pageCount = 1;

            //Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, OrderByExpression, FilterExpression, false);

        }

        [Test()]
        [ExpectedException(typeof(ArgumentException))]
        public void Repository_GetFilteredElementsWithPaggingInvalidPageCountThrowNewArgumentException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            int pageIndex = 1;
            int pageCount = 0;

            //Act
            PagedElements<TEntity> entities = repository.GetPagedElements(pageIndex, pageCount, OrderByExpression, FilterExpression, false);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetFilteredElementsWithPaggingNullOrderExpressionThrowNewArgumentNullException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            int pageIndex = 1;
            int pageCount = 1;

            //Act
            PagedElements<TEntity> entities = repository.GetPagedElements<int>(pageIndex, pageCount, null, FilterExpression, false);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetFilteredElementsWithPaggingNullFilterExpressionThrowNewArgumentNullException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            int pageIndex = 1;
            int pageCount = 1;

            //Act
            Expression<Func<TEntity, bool>> filter = null;
            var entities = repository.GetPagedElements<string>(pageIndex, pageCount, OrderByExpression, filter, false);
        }

        [Test()]
        public void GenericrRepository_GetPagedElementsAscending_Invoke_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            int pageIndex = 0;
            int pageCount = 1;

            //Act
            var entities = repository.GetPagedElements<string>(pageIndex, pageCount, OrderByExpression, true);

            //Assert
            Assert.IsNotNull(entities);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentException))]
        public void Repository_GetPagedElementsInvalidPageIndexThrowNewArgumentException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            int pageIndex = -1;
            int pageCount = 1;

            //Act
            var entities = repository.GetPagedElements<string>(pageIndex, pageCount, OrderByExpression, false);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentException))]
        public void Repository_GetPagedElementsInvalidPageCount_ThrowNewArgumentException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            int pageIndex = 0;
            int pageCount = 0;

            //Act
            var entities = repository.GetPagedElements<string>(pageIndex, pageCount, OrderByExpression, false);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetPagedElementsInvalidOrderByExpressionThrowNewArgumentNullException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            int pageIndex = 0;
            int pageCount = 1;

            //Act
            var entities = repository.GetPagedElements<int>(pageIndex, pageCount, null,false);
        }

        [Test()]
        public void Repository_GetPagedElementsDescending_Invoke_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            int pageIndex = 0;
            int pageCount = 1;
            bool ascending = false;

            //Act
            var entities = repository.GetPagedElements<string>(pageIndex, pageCount, OrderByExpression, ascending);

            //Assert
            Assert.IsNotNull(entities);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetPagedElementsWithNullSpecThrowNewArgumentNullException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();
            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            int pageIndex = 0;
            int pageCount = 10;

            //Act
            ISpecification<TEntity> spec = null;
            var entities = repository.GetPagedElements<string>(pageIndex, pageCount, OrderByExpression, spec, false);
        }

        [Test()]
        public void Repository_GetPagedElementsWithSpec_Invoke_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            ISpecification<TEntity> spec = new TrueSpecification<TEntity>();

            int pageIndex = 0;
            int pageCount = 10;

            //Act
            var entities = repository.GetPagedElements<string>(pageIndex, pageCount, OrderByExpression, spec, false);
        }

        [Test()]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Repository_GetBySpecWithNullSpecThrowArgumentNullException_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);

            //Act
            repository.GetBySpec((ISpecification<TEntity>)null);
        }

        [Test()]
        public void Repository_GetBySpecDirectSpec_Invoke_Test()
        {
            //Arrange
            IQueryableUnitOfWork unitOfWork = GetUnitOfWork();
            var traceManager = this.GetLoggerFactory();

            BaseRepository<TEntity> repository = new BaseRepository<TEntity>(traceManager);
            ISpecification<TEntity> specification = new DirectSpecification<TEntity>(this.FilterExpression);

            //Act
            IEnumerable<TEntity> result = repository.GetBySpec(specification);

            //Assert
            Assert.IsNotNull(result);
        }

        #endregion
    }
}
