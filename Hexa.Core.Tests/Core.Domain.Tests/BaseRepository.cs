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

    using Core.Tests;

    using Logging;

    using Microsoft.Practices.Unity;

    using Moq;

    using NUnit.Framework;

    using Specification;

    /// <summary>
    /// This is a test class for RepositoryTest and is intended
    /// to contain all common RepositoryTest Unit Tests
    /// </summary>
    [TestFixture]
    public class BaseRepositoryTests
    {
        /// <summary>
        /// A test for Add
        /// </summary>
        [Test]
        public void AddTest()
        {
            // Act
            var target = new ListRepository();
            var entity = new Entity
            {
                Id = 4,
                SampleProperty = "Sample 4"
            };

            // Act
            target.Add(entity);
            IEnumerable<Entity> result = target.GetAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 2);
            Assert.IsTrue(result.Contains(entity));
        }

        /// <summary>
        /// A test for Add
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Add_NullItemThrowArgumentNullException_Test()
        {
            // Act
            var target = new ListRepository();
            Entity entity = null;

            // Act
            target.Add(entity);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ApplyChanges_NullEntityThrowNewArgumentNullException_Test()
        {
            // Act
            var target = new ListRepository();

            // Assert
            target.Modify(null);
        }

        [Test]
        public void ApplyChanges_Test()
        {
            // Act
            var target = new ListRepository();
            Entity item = target.GetAll().First();

            // Assert
            target.Modify(item);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Attach_NullItem_Test()
        {
            // Act
            var target = new ListRepository();

            // Act
            target.Attach(null);
        }

        [Test]
        public void Attach_Test()
        {
            // Act
            var target = new ListRepository();
            var entity = new Entity
            {
                Id = 5,
                SampleProperty = "Sample 5"
            };

            // Act
            target.Attach(entity);

            // Assert
            Assert.IsTrue(target.GetFiltered(t => t.Id == 5).Count() == 1);
        }

        /// <summary>
        /// A test for Delete
        /// </summary>
        [Test]
        public void DeleteTest()
        {
            // Act
            var target = new ListRepository();

            // Act
            IEnumerable<Entity> result = target.GetAll();

            Entity firstEntity = result.First();
            target.Remove(firstEntity);

            IEnumerable<Entity> postResult = target.GetAll();

            // Assert
            Assert.IsNotNull(postResult);
            Assert.IsFalse(postResult.Contains(firstEntity));
        }

        /// <summary>
        /// A test for Delete
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Delete_NullItem_Test()
        {
            // Act
            var target = new ListRepository();
            Entity entity = null;

            // Act
            target.Remove(entity);
        }

        /// <summary>
        /// A test for GetAll
        /// </summary>
        [Test]
        public void GetAllTest()
        {
            // Act
            var target = new ListRepository();

            // Act
            IEnumerable<Entity> result = target.GetAll();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 1);
        }

        /// <summary>
        /// A test for GetFiltered
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFilteredAndOrderedAndPagedElements_InvalidPageCountThrowArgumentException_Test()
        {
            // Act
            var target = new ListRepository();
            int pageIndex = 0;
            int pageCount = 0;

            // Act
            PagedElements<Entity> result = target.GetPaged(
                                               pageIndex,
                                               pageCount,
                                               e => e.Id == 1,
                                               null,
                                               false);
        }

        /// <summary>
        /// A test for GetFiltered
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void GetFilteredAndOrderedAndPagedElements_InvalidPageIndexThrowArgumentException_Test()
        {
            // Act
            var target = new ListRepository();
            int pageIndex = -1;
            int pageCount = 1;

            // Act
            PagedElements<Entity> result = target.GetPaged(
                                               pageIndex,
                                               pageCount,
                                               e => e.Id == 1,
                                               null,
                                               false);
        }

        /// <summary>
        /// A test for GetFiltered
        /// </summary>
        [Test]
        public void GetFilteredTest()
        {
            // Act
            var target = new ListRepository();

            // Act
            IEnumerable<Entity> result = target.GetFiltered(e => e.Id == 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count() == 1);
            Assert.IsTrue(result.First().Id == 1);
        }

        /// <summary>
        /// A test for GetFiltered
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFiltered_FilterNullThrowArgumentNullException_Test()
        {
            // Act
            var target = new ListRepository();

            // Act
            target.GetFiltered(null);
        }

        /// <summary>
        /// A test for GetFiltered
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFiltered_SpecificKOrder_AscendingOrderAndFilterNullThrowArgumentNullException_Test()
        {
            // Act
            var target = new ListRepository();

            // Act
            target.GetFiltered(null, t => t.Id, true);
        }

        /// <summary>
        /// A test for GetFiltered
        /// </summary>
        [Test]
        public void GetFiltered_SpecificKOrder_AscendingOrder_Test()
        {
            // Act
            var target = new ListRepository();

            // Act
            target.GetFiltered(e => e.Id == 1, t => t.Id, true);
        }

        /// <summary>
        /// A test for GetFiltered
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetFiltered_SpecificKOrder_DescendingOrderAndFilterNullThrowArgumentNullException_Test()
        {
            // Act
            var target = new ListRepository();

            // Act
            target.GetFiltered(null, t => t.Id, false);
        }

        /// <summary>
        /// A test for GetFiltered
        /// </summary>
        [Test]
        public void GetFiltered_SpecificKOrder_DescendingOrder_Test()
        {
            // Act
            var target = new ListRepository();

            // Act
            target.GetFiltered(e => e.Id == 1, t => t.Id, false);
        }

        /// <summary>
        /// A test for GetFiltered
        /// </summary>
        [Test]
        public void GetFiltered_WithAscendingOrderedAndPagedElements_Test()
        {
            // Act
            var target = new ListRepository();
            int pageIndex = 0;
            int pageCount = 1;

            // Act
            PagedElements<Entity> result = target.GetPaged(
                                               pageIndex,
                                               pageCount,
                                               e => e.Id == 1,
                                               e => e.Id,
                                               true);

            // Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.TotalElements == 1);
        }

        /// <summary>
        /// A test for GetFiltered
        /// </summary>
        [Test]
        public void GetFiltered_WithDescendingOrderedAndPagedElements_Test()
        {
            // Act
            var target = new ListRepository();
            int pageIndex = 0;
            int pageCount = 1;

            // Act
            PagedElements<Entity> result = target.GetPaged(
                                               pageIndex,
                                               pageCount,
                                               e => e.Id == 1,
                                               e => e.Id,
                                               false);

            // Assert
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.TotalElements == 1);
        }

        /// <summary>
        /// A test for GetPaged
        /// </summary>
        [Test]
        public void GetPaged_AscendingOrder_Test()
        {
            // Act
            var target = new ListRepository();
            int pageIndex = 0;
            int pageCount = 1;

            // Act
            PagedElements<Entity> result = target.GetPaged(pageIndex, pageCount, e => true, e => e.Id, true);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalElements);
        }

        /// <summary>
        /// A test for GetPaged
        /// </summary>
        [Test]
        public void GetPaged_DescendingOrder_Test()
        {
            // Act
            var target = new ListRepository();
            int pageIndex = 0;
            int pageCount = 1;

            // Act
            PagedElements<Entity> result = target.GetPaged(pageIndex, pageCount, e => true, e => e.Id, false);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalElements);
        }

        /// <summary>
        /// A test for Container
        /// </summary>
        public void unitOfWorkTestHelper<T>()
        where T : class
        {
            // Act
            var target = new ListRepository();
        }

        [Test]
        public void UoW_Creation_Test()
        {
            this.unitOfWorkTestHelper<Entity>();
        }
    }
}