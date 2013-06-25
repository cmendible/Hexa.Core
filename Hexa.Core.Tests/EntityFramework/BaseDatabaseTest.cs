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

namespace Hexa.Core.Tests.EntityFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition.Hosting;
    using System.Configuration;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading;

    using Core.Data;
    using Core.Domain;

    using Data;

    using Domain;

    using Hexa.Core.Tests.Sql;

    using Logging;

    using Microsoft.Practices.Unity;

    using NUnit.Framework;

    using Security;

    using Validation;

    [TestFixture]
    public class SqlTest
    {
        #region Fields

        UnitOfWorkPerTestLifeTimeManager unitOfWorkPerTestLifeTimeManager = new UnitOfWorkPerTestLifeTimeManager();
        UnityContainer unityContainer;

        #endregion Fields

        #region Methods

        [Test]
        public void Add_EntityA()
        {
            EntityA entityA = this._Add_EntityA();

            Assert.IsNotNull(entityA);
            Assert.IsNotNull(entityA.Version);
            Assert.IsFalse(entityA.UniqueId == Guid.Empty);
            Assert.AreEqual("Martin", entityA.Name);
        }

        public void Commit()
        {
            IUnitOfWork unitOfWork = unityContainer.Resolve<IUnitOfWork>();
            unitOfWork.Commit();
        }

        [Test]
        public void Delete_EntityA()
        {
            EntityA entityA = this._Add_EntityA();

            IEntityARepository repo = unityContainer.Resolve<IEntityARepository>();
            IEnumerable<EntityA> results = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId);
            Assert.IsTrue(results.Count() > 0);

            EntityA entityA2Delete = results.First();

            repo.Remove(entityA2Delete);

            Commit();
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

            // Context Factory
            EntityFrameworkOfWorkFactory<DomainContext> ctxFactory = new EntityFrameworkOfWorkFactory<DomainContext>(this.ConnectionString());

            if (!ctxFactory.DatabaseExists())
            {
                ctxFactory.CreateDatabase();
            }

            ctxFactory.ValidateDatabaseSchema();

            unityContainer.RegisterType<DbContext, DomainContext>(new InjectionConstructor(this.ConnectionString()));
            unityContainer.RegisterInstance<IDatabaseManager>(ctxFactory);

            unityContainer.RegisterType<IUnitOfWork, EntityFrameworkUnitOfWork>(unitOfWorkPerTestLifeTimeManager);

            // Repositories
            unityContainer.RegisterType<IEntityARepository, EntityARepository>(new PerResolveLifetimeManager());
            unityContainer.RegisterType<IEntityBRepository, EntityBRepository>(new PerResolveLifetimeManager());

            ApplicationContext.User =
                new CorePrincipal(new CoreIdentity("cmendible", "hexa.auth", "cmendible@gmail.com"), new string[] { });
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            try
            {
                var dbManager = unityContainer.Resolve<IDatabaseManager>();
                dbManager.DeleteDatabase();
            }
            finally
            {
            }
        }

        [Test]
        public void Query_EntityA()
        {
            EntityA entityA = this._Add_EntityA();

            var repo = unityContainer.Resolve<IEntityARepository>();
            IEnumerable<EntityA> results = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId);
            Assert.IsTrue(results.Count() > 0);
        }

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

        [Test]
        public void Update_EntityA()
        {
            EntityA entityA = this._Add_EntityA();

            Thread.Sleep(1000);

            var repo = unityContainer.Resolve<IEntityARepository>();
            IEnumerable<EntityA> results = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId);
            Assert.IsTrue(results.Count() > 0);

            EntityA entityA2Update = results.First();
            entityA2Update.Name = "Maria";
            repo.Modify(entityA2Update);

            Commit();

            repo = unityContainer.Resolve<IEntityARepository>();
            entityA = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId).Single();
            Assert.AreEqual("Maria", entityA.Name);
            Assert.Greater(entityA.UpdatedAt, entityA.CreatedAt);
        }

        protected virtual string ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Sql.Connection"].ConnectionString;
        }

        protected virtual NHibernateUnitOfWorkFactory CreateNHContextFactory()
        {
            return new NHibernateUnitOfWorkFactory(DbProvider.MsSqlProvider, ConnectionString(), string.Empty, typeof(Entity).Assembly);
        }

        private EntityA _Add_EntityA()
        {
            var entityA = new EntityA();
            entityA.Name = "Martin";

            var repo = unityContainer.Resolve<IEntityARepository>();
            repo.Add(entityA);

            Commit();

            return entityA;
        }

        #endregion Methods
    }
}