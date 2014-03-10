//// ===================================================================================
//// Microsoft Developer & Platform Evangelism
//// ===================================================================================
//// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
//// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES
//// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//// ===================================================================================
//// Copyright (c) Microsoft Corporation.  All Rights Reserved.
//// This code is released under the terms of the MS-LPL license,
//// http://microsoftnlayerapp.codeplex.com/license
//// ===================================================================================
//namespace Hexa.Core.Domain.Tests
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Linq;

//    using Core.Tests;

//    using Logging;

//    using Microsoft.Practices.Unity;

//    using Moq;

//    using NUnit.Framework;

//    using Specification;

//    /// <summary>
//    /// This is a test class for RepositoryTest and is intended
//    /// to contain all common RepositoryTest Unit Tests
//    /// </summary>
//    [TestFixture]
//    public class BaseRepositoryTests
//    {
//        /// <summary>
//        /// A test for Add
//        /// </summary>
//        [Test]
//        public void AddTest()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            var entity = new Entity
//            {
//                Id = 4,
//                SampleProperty = "Sample 4"
//            };

//            // Act
//            target.Add(entity);
//            IEnumerable<Entity> result = target.GetAll();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.IsTrue(result.Count() == 4);
//            Assert.IsTrue(result.Contains(entity));
//        }

//        /// <summary>
//        /// A test for Add
//        /// </summary>
//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void Add_NullItemThrowArgumentNullException_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            Entity entity = null;

//            // Act
//            target.Add(entity);
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void ApplyChanges_NullEntityThrowNewArgumentNullException_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());

//            // Assert
//            target.Modify(null);
//        }

//        [Test]
//        public void ApplyChanges_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            Entity item = target.GetAll().First();

//            // Assert
//            target.Modify(item);
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void Attach_NullItem_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());

//            // Act
//            target.Attach(null);
//        }

//        [Test]
//        public void Attach_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            var entity = new Entity
//            {
//                Id = 5,
//                SampleProperty = "Sample 5"
//            };

//            // Act
//            target.Attach(entity);

//            // Assert
//            Assert.IsTrue(target.GetFilteredElements(t => t.Id == 5).Count() == 1);
//        }

//        /// <summary>
//        /// A test for Delete
//        /// </summary>
//        [Test]
//        public void DeleteTest()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());

//            // Act
//            IEnumerable<Entity> result = target.GetAll();

//            Entity firstEntity = result.First();
//            target.Remove(firstEntity);

//            IEnumerable<Entity> postResult = target.GetAll();

//            // Assert
//            Assert.IsNotNull(postResult);
//            Assert.IsFalse(postResult.Contains(firstEntity));
//        }

//        /// <summary>
//        /// A test for Delete
//        /// </summary>
//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void Delete_NullItem_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            Entity entity = null;

//            // Act
//            target.Remove(entity);
//        }

//        /// <summary>
//        /// A test for GetAll
//        /// </summary>
//        [Test]
//        public void GetAllTest()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());

//            // Act
//            IEnumerable<Entity> result = target.GetAll();

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.IsTrue(result.Count() == 3);
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void GetBySpec_NullSpecThrowArgumentNullException_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            ISpecification<Entity> spec = new DirectSpecification<Entity>(t => t.Id == 1);

//            // Act
//            target.GetBySpec(null);
//        }

//        [Test]
//        public void GetBySpec_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            ISpecification<Entity> spec = new DirectSpecification<Entity>(t => t.Id == 1);

//            // Act
//            IEnumerable<Entity> result = target.GetBySpec(spec);

//            // Assert
//            Assert.IsTrue(result.Count() == 1);
//        }

//        /// <summary>
//        /// A test for GetFilteredElements
//        /// </summary>
//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void GetFilteredAndOrderedAndPagedElements_InvalidOrderByExpressionThrowArgumentNullException_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            int pageIndex = 0;
//            int pageCount = 1;

//            // Act
//            PagedElements<Entity> result = target.GetPagedElements<int>(
//                                               pageIndex,
//                                               pageCount,
//                                               e => e.Id == 1,
//                                               null,
//                                               false);
//        }

//        /// <summary>
//        /// A test for GetFilteredElements
//        /// </summary>
//        [Test]
//        [ExpectedException(typeof(ArgumentException))]
//        public void GetFilteredAndOrderedAndPagedElements_InvalidPageCountThrowArgumentException_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            int pageIndex = 0;
//            int pageCount = 0;

//            // Act
//            PagedElements<Entity> result = target.GetPagedElements<int>(
//                                               pageIndex,
//                                               pageCount,
//                                               e => e.Id == 1,
//                                               null,
//                                               false);
//        }

//        /// <summary>
//        /// A test for GetFilteredElements
//        /// </summary>
//        [Test]
//        [ExpectedException(typeof(ArgumentException))]
//        public void GetFilteredAndOrderedAndPagedElements_InvalidPageIndexThrowArgumentException_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            int pageIndex = -1;
//            int pageCount = 1;

//            // Act
//            PagedElements<Entity> result = target.GetPagedElements<int>(
//                                               pageIndex,
//                                               pageCount,
//                                               e => e.Id == 1,
//                                               null,
//                                               false);
//        }

//        /// <summary>
//        /// A test for GetFilteredElements
//        /// </summary>
//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void GetFilteredAndOrderedElements_InvalidOrderByExpressionThrowArgumentNullException_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());

//            // Act
//            IEnumerable<Entity> result = target.GetFilteredElements<int>(e => e.Id == 1, null, false);

//            // Assert
//            Assert.IsTrue(result != null);
//            Assert.IsTrue(result.Count() == 1);
//        }

//        /// <summary>
//        /// A test for GetFilteredElements
//        /// </summary>
//        [Test]
//        public void GetFilteredElementsTest()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());

//            // Act
//            IEnumerable<Entity> result = target.GetFilteredElements(e => e.Id == 1);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.IsTrue(result.Count() == 1);
//            Assert.IsTrue(result.First().Id == 1);
//        }

//        /// <summary>
//        /// A test for GetFilteredElements
//        /// </summary>
//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void GetFilteredElements_FilterNullThrowArgumentNullException_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());

//            // Act
//            target.GetFilteredElements(null);
//        }

//        /// <summary>
//        /// A test for GetFilteredElements
//        /// </summary>
//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void GetFilteredElements_SpecificKOrder_AscendingOrderAndFilterNullThrowArgumentNullException_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());

//            // Act
//            target.GetFilteredElements(null, t => t.Id, true);
//        }

//        /// <summary>
//        /// A test for GetFilteredElements
//        /// </summary>
//        [Test]
//        public void GetFilteredElements_SpecificKOrder_AscendingOrder_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());

//            // Act
//            target.GetFilteredElements(e => e.Id == 1, t => t.Id, true);
//        }

//        /// <summary>
//        /// A test for GetFilteredElements
//        /// </summary>
//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void GetFilteredElements_SpecificKOrder_DescendingOrderAndFilterNullThrowArgumentNullException_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());

//            // Act
//            target.GetFilteredElements(null, t => t.Id, false);
//        }

//        /// <summary>
//        /// A test for GetFilteredElements
//        /// </summary>
//        [Test]
//        public void GetFilteredElements_SpecificKOrder_DescendingOrder_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());

//            // Act
//            target.GetFilteredElements(e => e.Id == 1, t => t.Id, false);
//        }

//        /// <summary>
//        /// A test for GetFilteredElements
//        /// </summary>
//        [Test]
//        public void GetFiltered_WithAscendingOrderedAndPagedElements_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            int pageIndex = 0;
//            int pageCount = 1;

//            // Act
//            PagedElements<Entity> result = target.GetPagedElements(
//                                               pageIndex,
//                                               pageCount,
//                                               e => e.Id == 1,
//                                               e => e.Id,
//                                               true);

//            // Assert
//            Assert.IsTrue(result != null);
//            Assert.IsTrue(result.TotalElements == 1);
//        }

//        /// <summary>
//        /// A test for GetFilteredElements
//        /// </summary>
//        [Test]
//        public void GetFiltered_WithDescendingOrderedAndPagedElements_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            int pageIndex = 0;
//            int pageCount = 1;

//            // Act
//            PagedElements<Entity> result = target.GetPagedElements(
//                                               pageIndex,
//                                               pageCount,
//                                               e => e.Id == 1,
//                                               e => e.Id,
//                                               false);

//            // Assert
//            Assert.IsTrue(result != null);
//            Assert.IsTrue(result.TotalElements == 1);
//        }

//        /// <summary>
//        /// A test for GetPagedElements
//        /// </summary>
//        [Test]
//        public void GetPagedElements_AscendingOrder_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            int pageIndex = 0;
//            int pageCount = 1;

//            // Act
//            PagedElements<Entity> result = target.GetPagedElements(pageIndex, pageCount, e => true, e => e.Id, true);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(3, result.TotalElements);
//        }

//        /// <summary>
//        /// A test for GetPagedElements
//        /// </summary>
//        [Test]
//        public void GetPagedElements_DescendingOrder_Test()
//        {
//            // Act
//            var target = new BaseRepository<Entity>(this._MockUnitOfWork());
//            int pageIndex = 0;
//            int pageCount = 1;

//            // Act
//            PagedElements<Entity> result = target.GetPagedElements(pageIndex, pageCount, e => true, e => e.Id, false);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(3, result.TotalElements);
//        }

//        /// <summary>
//        /// A test for Container
//        /// </summary>
//        public void unitOfWorkTestHelper<T>()
//        where T : class
//        {
//            // Act
//            var target = new BaseRepository<T>(this._MockUnitOfWork());
//        }

//        [Test]
//        public void UoW_Creation_Test()
//        {
//            this.unitOfWorkTestHelper<Entity>();
//        }

//        private IUnitOfWork _MockUnitOfWork()
//        {
//            var list = new List<Entity>
//            {
//                new Entity { Id = 1, SampleProperty = "Sample 1" },
//                new Entity { Id = 2, SampleProperty = "Sample 2" },
//                new Entity
//                {
//                    Id = 3,
//                    SampleProperty = "Sample 3"
//                }
//            };

//            var unitOfWorkMock = new Mock<IUnitOfWork>();
//            unitOfWorkMock.Setup(w => w.Query<Entity>())
//            .Returns(list.AsQueryable());

//            unitOfWorkMock.Setup(w => w.Add<Entity>(It.IsAny<Entity>()))
//            .Callback((Entity e) => { list.Add(e); });

//            unitOfWorkMock.Setup(w => w.Attach<Entity>(It.IsAny<Entity>()))
//            .Callback((Entity e) => { list.Add(e); });

//            unitOfWorkMock.Setup(w => w.Delete<Entity>(It.IsAny<Entity>()))
//            .Callback((Entity e) => { list.Remove(e); });

//            return unitOfWorkMock.Object;
//        }
//    }
//}