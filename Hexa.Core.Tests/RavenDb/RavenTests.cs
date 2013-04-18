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

#if !MONO

namespace Hexa.Core.Tests.RavenTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Core.Data;
    using Core.Domain;

    using Data;

    using Domain;

    using Logging;

    using NUnit.Framework;

    using Validation;
    using Microsoft.Practices.Unity;
    using Hexa.Core.Tests.Sql;
    using Raven.Client.Document;

    [TestFixture]
    public class RavenTests
    {
        UnityContainer unityContainer;
        UnitOfWorkPerTestLifeTimeManager unitOfWorkPerTestLifeTimeManager = new UnitOfWorkPerTestLifeTimeManager();

        #region Methods

        [NUnit.Framework.SetUp]
        public void Setup()
        {
            IUnitOfWork unitOfWork = unityContainer.Resolve<IUnitOfWork>();
            unitOfWork.Start();
        }

        [TearDown]
        public void TearDown()
        {
            IUnitOfWork unitOfWork = unityContainer.Resolve<IUnitOfWork>();
            unitOfWork.Dispose();
            unitOfWorkPerTestLifeTimeManager.RemoveValue();
        }

        public void Commit()
        {
            IUnitOfWork unitOfWork = unityContainer.Resolve<IUnitOfWork>();
            unitOfWork.Commit();
        }

        [Test]
        public void Add_EntityA()
        {
            EntityA entityA = this._Add_EntityA();

            Assert.IsNotNull(entityA);
            //Assert.IsNotNull(entityA.Version);
            Assert.IsFalse(entityA.UniqueId == Guid.Empty);
            Assert.AreEqual("Martin", entityA.Name);
        }

        [Test]
        public void Delete_EntityA()
        {
            EntityA entityA = this._Add_EntityA();

            var repo = ServiceLocator.GetInstance<IEntityARepository>();
            IEnumerable<EntityA> results = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId);
            Assert.IsTrue(results.Count() > 0);

            EntityA entityA2Delete = results.First();

            repo.Remove(entityA2Delete);

            Commit();

            repo = ServiceLocator.GetInstance<IEntityARepository>();
            Assert.AreEqual(0, repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId).Count());
        }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            unityContainer = new UnityContainer();
            ServiceLocator.Initialize(
                        (x, y) => unityContainer.RegisterType(x, y),
                        (x, y) => unityContainer.RegisterInstance(x, y),
                        (x) => { return unityContainer.Resolve(x); },
                        (x) => { return unityContainer.ResolveAll(x); }
                    );

            unityContainer.RegisterInstance<ILoggerFactory>(new Log4NetLoggerFactory());

            // Context Factory
            RavenUnitOfWorkFactory ctxFactory = new RavenUnitOfWorkFactory();
            Raven.Client.Document.DocumentStore sessionFactory = ctxFactory.Create();

            unityContainer.RegisterInstance<Raven.Client.Document.DocumentStore>(sessionFactory);
            ServiceLocator.RegisterInstance<IDatabaseManager>(ctxFactory);

            unityContainer.RegisterType<IUnitOfWork, RavenUnitOfWork>(unitOfWorkPerTestLifeTimeManager);

            // Repositories
            unityContainer.RegisterType<IEntityARepository, EntityARepository>();

            // Services

            if (!ctxFactory.DatabaseExists())
            {
                ctxFactory.CreateDatabase();
            }

            ctxFactory.ValidateDatabaseSchema();
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            var dbManager = ServiceLocator.GetInstance<IDatabaseManager>();
            dbManager.DeleteDatabase();
        }

        [Test]
        public void Query_EntityA()
        {
            EntityA entityA = this._Add_EntityA();

            var repo = ServiceLocator.GetInstance<IEntityARepository>();
            IEnumerable<EntityA> results = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId);
            Assert.IsTrue(results.Count() > 0);
        }

        [Test]
        public void Update_EntityA()
        {
            EntityA entityA = this._Add_EntityA();

            var repo = ServiceLocator.GetInstance<IEntityARepository>();
            IEnumerable<EntityA> results = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId);
            Assert.IsTrue(results.Count() > 0);

            EntityA entityA2Update = results.First();
            entityA2Update.Name = "Maria";
            repo.Modify(entityA2Update);

            Commit();

            Thread.Sleep(1000);

            repo = ServiceLocator.GetInstance<IEntityARepository>();
            EntityA entityA2 = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId).Single();
            Assert.AreEqual("Maria", entityA2.Name);
            //Assert.Greater(entityA2.UpdatedAt, entityA2.CreatedAt);
        }

        private EntityA _Add_EntityA()
        {
            var entityA = new EntityA();
            entityA.Name = "Martin";

            var repo = ServiceLocator.GetInstance<IEntityARepository>();
            repo.Add(entityA);

            Commit();

            return entityA;
        }

        #endregion Methods
    }
}

#endif