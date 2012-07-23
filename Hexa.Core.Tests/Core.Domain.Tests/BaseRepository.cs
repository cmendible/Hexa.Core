#region Header

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

#endregion Header

namespace Hexa.Core.Domain.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Core.Tests;

    using Logging;

    using NUnit.Framework;

    using Rhino.Mocks;

    using Specification;

    using SL = Microsoft.Practices.ServiceLocation;

    /// <summary>
    ///This is a test class for RepositoryTest and is intended
    ///to contain all common RepositoryTest Unit Tests
    ///</summary>
    [TestFixture]
    public class BaseRepositoryTests
    {
        #region Fields

        private IoCContainer _container;
        private DictionaryServicesContainer _dictionaryContainer;

        #endregion Fields

        #region Methods

        /// <summary>
        ///A test for Add
        ///</summary>
        [Test]
        public void AddTest()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            var entity = new Entity
            {
                Id = 4,
                SampleProperty = "Sample 4"
            };

            //Act
            target.Add(entity);
            IEnumerable<Entity> result = target.GetAll();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 4);
            Assert.IsTrue(result.Contains(entity));
        }

        /// <summary>
        ///A test for Add
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_NullItemThrowArgumentNullException_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            Entity entity = null;

            //Act
            target.Add(entity);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ApplyChanges_NullEntityThrowNewArgumentNullException_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);

            //Assert
            target.Modify(null);
        }

        [Test]
        public void ApplyChanges_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            Entity item = target.GetAll().First();

            //Assert
            target.Modify(item);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Attach_NullItem_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);

            //Act
            target.Attach(null);
        }

        [Test]
        public void Attach_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            var entity = new Entity
            {
                Id = 5,
                SampleProperty = "Sample 5"
            };

            //Act
            target.Attach(entity);

            //Assert
            Assert.IsTrue(target.GetFilteredElements(t => t.Id == 5).Count() == 1);
        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        [Test]
        public void DeleteTest()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);

            //Act
            IEnumerable<Entity> result = target.GetAll();

            Entity firstEntity = result.First();
            target.Remove(firstEntity);

            IEnumerable<Entity> postResult = target.GetAll();

            //Assert
            Assert.IsNotNull(postResult);
            Assert.IsFalse(postResult.Contains(firstEntity));
        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Delete_NullItem_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            Entity entity = null;

            //Act
            target.Remove(entity);
        }

        [TestFixtureSetUp]
        public void FixtureSetUp()
        {
            this._dictionaryContainer = new DictionaryServicesContainer();

            SL.ServiceLocator.SetLocatorProvider(() => this._dictionaryContainer);

            this._container = new IoCContainer(
                (x, y) => this._dictionaryContainer.RegisterType(x, y),
                (x, y) => this._dictionaryContainer.RegisterInstance(x, y)
            );
        }

        /// <summary>
        ///A test for GetAll
        ///</summary>
        [Test]
        public void GetAllTest()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);

            //Act
            IEnumerable<Entity> result = target.GetAll();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 3);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetBySpec_NullSpecThrowArgumentNullException_Test()
        {
            //Arrange
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            ISpecification<Entity> spec = new DirectSpecification<Entity>(t => t.Id == 1);

            //Act
            target.GetBySpec(null);
        }

        [Test]
        public void GetBySpec_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            ISpecification<Entity> spec = new DirectSpecification<Entity>(t => t.Id == 1);

            //Act
            IEnumerable<Entity> result = target.GetBySpec(spec);

            //Assert
            Assert.IsTrue(result.Count() == 1);
        }

        /// <summary>
        ///A test for GetFilteredElements
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFilteredAndOrderedAndPagedElements_InvalidOrderByExpressionThrowArgumentNullException_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            int pageIndex = 0;
            int pageCount = 1;

            //Act
            PagedElements<Entity> result = target.GetPagedElements<int>(pageIndex, pageCount, null, e => e.Id == 1,
                                           false);
        }

        /// <summary>
        ///A test for GetFilteredElements
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFilteredAndOrderedAndPagedElements_InvalidPageCountThrowArgumentException_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            int pageIndex = 0;
            int pageCount = 0;

            //Act
            PagedElements<Entity> result = target.GetPagedElements<int>(pageIndex, pageCount, null, e => e.Id == 1,
                                           false);
        }

        /// <summary>
        ///A test for GetFilteredElements
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFilteredAndOrderedAndPagedElements_InvalidPageIndexThrowArgumentException_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            int pageIndex = -1;
            int pageCount = 1;

            //Act
            PagedElements<Entity> result = target.GetPagedElements<int>(pageIndex, pageCount, null, e => e.Id == 1,
                                           false);
        }

        /// <summary>
        ///A test for GetFilteredElements
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFilteredAndOrderedElements_InvalidOrderByExpressionThrowArgumentNullException_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);

            //Act
            IEnumerable<Entity> result = target.GetFilteredElements<int>(e => e.Id == 1, null, false);

            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Count() == 1);
        }

        /// <summary>
        ///A test for GetFilteredElements
        ///</summary>
        [Test]
        public void GetFilteredElementsTest()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);

            //Act
            IEnumerable<Entity> result = target.GetFilteredElements(e => e.Id == 1);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.First().Id == 1);
        }

        /// <summary>
        ///A test for GetFilteredElements
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFilteredElements_FilterNullThrowArgumentNullException_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);

            //Act
            target.GetFilteredElements(null);
        }

        /// <summary>
        ///A test for GetFilteredElements
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFilteredElements_SpecificKOrder_AscendingOrderAndFilterNullThrowArgumentNullException_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);

            //Act
            target.GetFilteredElements(null, t => t.Id, true);
        }

        /// <summary>
        ///A test for GetFilteredElements
        ///</summary>
        [Test]
        public void GetFilteredElements_SpecificKOrder_AscendingOrder_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);

            //Act
            target.GetFilteredElements(e => e.Id == 1, t => t.Id, true);
        }

        /// <summary>
        ///A test for GetFilteredElements
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFilteredElements_SpecificKOrder_DescendingOrderAndFilterNullThrowArgumentNullException_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);

            //Act
            target.GetFilteredElements(null, t => t.Id, false);
        }

        /// <summary>
        ///A test for GetFilteredElements
        ///</summary>
        [Test]
        public void GetFilteredElements_SpecificKOrder_DescendingOrder_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);

            //Act
            target.GetFilteredElements(e => e.Id == 1, t => t.Id, false);
        }

        /// <summary>
        ///A test for GetFilteredElements
        ///</summary>
        [Test]
        public void GetFiltered_WithAscendingOrderedAndPagedElements_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            int pageIndex = 0;
            int pageCount = 1;

            //Act
            PagedElements<Entity> result = target.GetPagedElements(pageIndex, pageCount, e => e.Id, e => e.Id == 1,
                                           true);

            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.TotalElements == 1);
        }

        /// <summary>
        ///A test for GetFilteredElements
        ///</summary>
        [Test]
        public void GetFiltered_WithDescendingOrderedAndPagedElements_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            int pageIndex = 0;
            int pageCount = 1;

            //Act
            PagedElements<Entity> result = target.GetPagedElements(pageIndex, pageCount, e => e.Id, e => e.Id == 1,
                                           false);

            //Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.TotalElements == 1);
        }

        /// <summary>
        ///A test for GetPagedElements
        ///</summary>
        [Test]
        public void GetPagedElements_AscendingOrder_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            int pageIndex = 0;
            int pageCount = 1;

            //Act
            PagedElements<Entity> result = target.GetPagedElements(pageIndex, pageCount, e => e.Id, true);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.TotalElements);
        }

        /// <summary>
        ///A test for GetPagedElements
        ///</summary>
        [Test]
        public void GetPagedElements_DescendingOrder_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            int pageIndex = 0;
            int pageCount = 1;

            //Act
            PagedElements<Entity> result = target.GetPagedElements(pageIndex, pageCount, e => e.Id, false);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.TotalElements);
        }

        /// <summary>
        ///A test for GetPagedElements
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetPagedElements_InvalidOrderExpressionThrowArgumentNullException_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            int pageIndex = 0;
            int pageCount = 1;

            target.GetPagedElements<int>(pageIndex, pageCount, null, false);
        }

        /// <summary>
        ///A test for GetPagedElements
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPagedElements_InvalidPageCountThrowArgumentException_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            int pageIndex = 0;
            int pageCount = 0;

            target.GetPagedElements<int>(pageIndex, pageCount, null, false);
        }

        /// <summary>
        ///A test for GetPagedElements
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetPagedElements_InvalidPageIndexThrowArgumentException_Test()
        {
            //Arrange
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<Entity>(loggerFactory);
            int pageIndex = -1;
            int pageCount = 0;

            target.GetPagedElements<int>(pageIndex, pageCount, null, false);
        }

        [TearDown]
        public void TearDown()
        {
            UnitOfWorkScope.DisposeCurrent();
        }

        /// <summary>
        ///A test for Container
        ///</summary>
        public void unitOfWorkTestHelper<T>()
            where T : class
        {
            IUnitOfWork actual = this._MockUnitOfWork();
            ILoggerFactory loggerFactory = this._MockLoggerFactory();

            //Act
            var target = new BaseRepository<T>(loggerFactory);

            //Assert
            IUnitOfWork expected;
            expected = target.UnitOfWork;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void UoW_Creation_NullLoggerFactoryThrowArgumentNullException_Test()
        {
            var repository = new BaseRepository<Entity>(null);
        }

        [Test]
        public void UoW_Creation_Test()
        {
            this.unitOfWorkTestHelper<Entity>();
        }

        private ILoggerFactory _MockLoggerFactory()
        {
            var logger = MockRepository.GenerateMock<ILogger>();
            var loggerFactory = MockRepository.GenerateMock<ILoggerFactory>();
            loggerFactory.Expect(l => l.Create(GetType()))
            .IgnoreArguments()
            .Return(logger);

            return loggerFactory;
        }

        private IUnitOfWork _MockUnitOfWork()
        {
            var list = new List<Entity>
            {
                new Entity {Id = 1, SampleProperty = "Sample 1"},
                new Entity {Id = 2, SampleProperty = "Sample 2"},
                new Entity
                {
                    Id = 3,
                    SampleProperty = "Sample 3"
                }
            };
            var set = new MemorySet<Entity>(list);

            var actual = MockRepository.GenerateMock<IUnitOfWork>();
            actual.Expect(w => w.CreateSet<Entity>())
            .Return(set);

            var factory = MockRepository.GenerateMock<IUnitOfWorkFactory>();
            factory.Expect(f => f.Create()).Return(actual);
            this._dictionaryContainer.OverrideInstance<IUnitOfWorkFactory>(factory);

            return actual;
        }

        #endregion Methods
    }
}