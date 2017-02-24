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
    using Microsoft.Extensions.Logging;
    using Moq;
    using Xunit;

    /// <summary>
    /// This is a Fact class for RepositoryFact and is intended
    /// to contain all common RepositoryFact Unit Facts
    /// </summary>
    public class BaseRepositoryFacts
    {
        ILogger<BaseRepository<Entity, int>> logger = new Mock<ILogger<BaseRepository<Entity, int>>>().Object;

        /// <summary>
        /// A Fact for Add
        /// </summary>
        [Fact]
        public void AddFact()
        {
            // Act
            var target = new ListRepository(logger);
            var entity = new Entity
            {
                Id = 4,
                SampleProperty = "Sample 4"
            };

            // Act
            target.Add(entity);
            IEnumerable<Entity> result = target.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count() == 2);
            Assert.True(result.Contains(entity));
        }

        /// <summary>
        /// A Fact for Add
        /// </summary>
        [Fact]
        public void Add_NullItemThrowArgumentNullException_Fact()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                var target = new ListRepository(logger);
                Entity entity = null;

                // Act
                target.Add(entity);
            });
        }

        [Fact]
        public void ApplyChanges_NullEntityThrowNewArgumentNullException_Fact()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                var target = new ListRepository(logger);

                // Assert
                target.Modify(null);
            });
        }

        [Fact]
        public void ApplyChanges_Fact()
        {
            // Act
            var target = new ListRepository(logger);
            Entity item = target.GetAll().First();

            // Assert
            target.Modify(item);
        }

        [Fact]
        public void Attach_NullItem_Fact()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                var target = new ListRepository(logger);

                // Act
                target.Attach(null);
            });
        }

        [Fact]
        public void Attach_Fact()
        {
            // Act
            var target = new ListRepository(logger);
            var entity = new Entity
            {
                Id = 5,
                SampleProperty = "Sample 5"
            };

            // Act
            target.Attach(entity);

            // Assert
            Assert.True(target.GetFiltered(t => t.Id == 5).Count() == 1);
        }

        /// <summary>
        /// A Fact for Delete
        /// </summary>
        [Fact]
        public void DeleteFact()
        {
            // Act
            var target = new ListRepository(logger);

            // Act
            IEnumerable<Entity> result = target.GetAll();

            Entity firstEntity = result.First();
            target.Remove(firstEntity);

            IEnumerable<Entity> postResult = target.GetAll();

            // Assert
            Assert.NotNull(postResult);
            Assert.False(postResult.Contains(firstEntity));
        }

        /// <summary>
        /// A Fact for Delete
        /// </summary>
        [Fact]
        public void Delete_NullItem_Fact()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                var target = new ListRepository(logger);
                Entity entity = null;

                // Act
                target.Remove(entity);
            });
        }

        /// <summary>
        /// A Fact for GetAll
        /// </summary>
        [Fact]
        public void GetAllFact()
        {
            // Act
            var target = new ListRepository(logger);

            // Act
            IEnumerable<Entity> result = target.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count() == 1);
        }

        /// <summary>
        /// A Fact for GetFiltered
        /// </summary>
        [Fact]
        public void GetFilteredAndOrderedAndPagedElements_InvalidPageCountThrowArgumentException_Fact()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                var target = new ListRepository(logger);
                int pageIndex = 0;
                int pageCount = 0;

                // Act
                PagedElements<Entity> result = target.GetPaged(
                                                   pageIndex,
                                                   pageCount,
                                                   e => e.Id == 1,
                                                   null,
                                                   false);
            });
        }

        /// <summary>
        /// A Fact for GetFiltered
        /// </summary>
        [Fact]
        public void GetFilteredAndOrderedAndPagedElements_InvalidPageIndexThrowArgumentException_Fact()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                // Act
                var target = new ListRepository(logger);
                int pageIndex = -1;
                int pageCount = 1;

                // Act
                PagedElements<Entity> result = target.GetPaged(
                                                   pageIndex,
                                                   pageCount,
                                                   e => e.Id == 1,
                                                   null,
                                                   false);
            });
        }

        /// <summary>
        /// A Fact for GetFiltered
        /// </summary>
        [Fact]
        public void GetFilteredFact()
        {
            // Act
            var target = new ListRepository(logger);

            // Act
            IEnumerable<Entity> result = target.GetFiltered(e => e.Id == 1);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count() == 1);
            Assert.True(result.First().Id == 1);
        }

        /// <summary>
        /// A Fact for GetFiltered
        /// </summary>
        [Fact]
        public void GetFiltered_FilterNullThrowArgumentNullException_Fact()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                // Act
                var target = new ListRepository(logger);

                // Act
                target.GetFiltered(null);
            });

        }

        /// <summary>
        /// A Fact for GetFiltered
        /// </summary>
        [Fact]
        public void GetFiltered_SpecificKOrder_AscendingOrderAndFilterNullThrowArgumentNullException_Fact()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {

                // Act
                var target = new ListRepository(logger);

                // Act
                target.GetFiltered(null, t => t.Id, true);
            });

        }

        /// <summary>
        /// A Fact for GetFiltered
        /// </summary>
        [Fact]
        public void GetFiltered_SpecificKOrder_AscendingOrder_Fact()
        {
            // Act
            var target = new ListRepository(logger);

            // Act
            target.GetFiltered(e => e.Id == 1, t => t.Id, true);
        }

        /// <summary>
        /// A Fact for GetFiltered
        /// </summary>
        [Fact]
        public void GetFiltered_SpecificKOrder_DescendingOrderAndFilterNullThrowArgumentNullException_Fact()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                    // Act
                    var target = new ListRepository(logger);

                    // Act
                    target.GetFiltered(null, t => t.Id, false);
            });
        }

        /// <summary>
        /// A Fact for GetFiltered
        /// </summary>
        [Fact]
        public void GetFiltered_SpecificKOrder_DescendingOrder_Fact()
        {
            // Act
            var target = new ListRepository(logger);

            // Act
            target.GetFiltered(e => e.Id == 1, t => t.Id, false);
        }

        /// <summary>
        /// A Fact for GetFiltered
        /// </summary>
        [Fact]
        public void GetFiltered_WithAscendingOrderedAndPagedElements_Fact()
        {
            // Act
            var target = new ListRepository(logger);
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
            Assert.True(result != null);
            Assert.True(result.TotalElements == 1);
        }

        /// <summary>
        /// A Fact for GetFiltered
        /// </summary>
        [Fact]
        public void GetFiltered_WithDescendingOrderedAndPagedElements_Fact()
        {
            // Act
            var target = new ListRepository(logger);
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
            Assert.True(result != null);
            Assert.True(result.TotalElements == 1);
        }

        /// <summary>
        /// A Fact for GetPaged
        /// </summary>
        [Fact]
        public void GetPaged_AscendingOrder_Fact()
        {
            // Act
            var target = new ListRepository(logger);
            int pageIndex = 0;
            int pageCount = 1;

            // Act
            PagedElements<Entity> result = target.GetPaged(pageIndex, pageCount, e => true, e => e.Id, true);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalElements);
        }

        /// <summary>
        /// A Fact for GetPaged
        /// </summary>
        [Fact]
        public void GetPaged_DescendingOrder_Fact()
        {
            // Act
            var target = new ListRepository(logger);
            int pageIndex = 0;
            int pageCount = 1;

            // Act
            PagedElements<Entity> result = target.GetPaged(pageIndex, pageCount, e => true, e => e.Id, false);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalElements);
        }

        /// <summary>
        /// A Fact for Container
        /// </summary>
        public void unitOfWorkTestHelper<T>()
        where T : class
        {
            // Act
            var target = new ListRepository(logger);
        }

        public void UoW_Creation_Test()
        {
            this.unitOfWorkTestHelper<Entity>();
        }
    }
}