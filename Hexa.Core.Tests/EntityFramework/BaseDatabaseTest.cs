//----------------------------------------------------------------------------------------------
// <copyright file="SqlTest.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.EntityFramework
{
    using System;
    using System.Collections.Generic;
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
        UnitOfWorkPerTestLifeTimeManager unitOfWorkPerTestLifeTimeManager = new UnitOfWorkPerTestLifeTimeManager();
        UnityContainer unityContainer;

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
            IUnitOfWork unitOfWork = this.unityContainer.Resolve<IUnitOfWork>();
            unitOfWork.Commit();
        }

        [Test]
        public void Delete_EntityA()
        {
            EntityA entityA = this._Add_EntityA();

            IEntityARepository repo = this.unityContainer.Resolve<IEntityARepository>();
            IEnumerable<EntityA> results = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId);
            Assert.IsTrue(results.Count() > 0);

            EntityA entityA2Delete = results.First();

            repo.Remove(entityA2Delete);

            this.Commit();
        }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            this.unityContainer = new UnityContainer();
            ServiceLocator.Initialize(
                (x, y) => this.unityContainer.RegisterType(x, y),
                (x, y) => this.unityContainer.RegisterInstance(x, y),
                (x) => { return unityContainer.Resolve(x); },
                (x) => { return unityContainer.ResolveAll(x); });

            // Context Factory
            EntityFrameworkUnitOfWorkFactory<DomainContext> ctxFactory = new EntityFrameworkUnitOfWorkFactory<DomainContext>(this.ConnectionString());

            if (!ctxFactory.DatabaseExists())
            {
                ctxFactory.CreateDatabase();
            }

            ctxFactory.ValidateDatabaseSchema();

            this.unityContainer.RegisterType<DbContext, DomainContext>(new InjectionConstructor(this.ConnectionString()));
            this.unityContainer.RegisterInstance<IDatabaseManager>(ctxFactory);

            this.unityContainer.RegisterType<IUnitOfWork, EntityFrameworkUnitOfWork>(this.unitOfWorkPerTestLifeTimeManager);

            // Repositories
            this.unityContainer.RegisterType<IEntityARepository, EntityARepository>(new PerResolveLifetimeManager());
            this.unityContainer.RegisterType<IEntityBRepository, EntityBRepository>(new PerResolveLifetimeManager());

            ApplicationContext.User =
                new CorePrincipal(new CoreIdentity("cmendible", "hexa.auth", "cmendible@gmail.com"), new string[] { });
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            try
            {
                var dbManager = this.unityContainer.Resolve<IDatabaseManager>();
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

            var repo = this.unityContainer.Resolve<IEntityARepository>();
            IEnumerable<EntityA> results = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId);
            Assert.IsTrue(results.Count() > 0);
        }

        [NUnit.Framework.SetUp]
        public void Setup()
        {
            IUnitOfWork unitOfWork = this.unityContainer.Resolve<IUnitOfWork>();
            unitOfWork.Start();
        }

        [TearDown]
        public void TearDown()
        {
            IUnitOfWork unitOfWork = this.unityContainer.Resolve<IUnitOfWork>();
            unitOfWork.Dispose();
            this.unitOfWorkPerTestLifeTimeManager.RemoveValue();
        }

        [Test]
        public void Update_EntityA()
        {
            EntityA entityA = this._Add_EntityA();

            Thread.Sleep(1000);

            var repo = this.unityContainer.Resolve<IEntityARepository>();
            IEnumerable<EntityA> results = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId);
            Assert.IsTrue(results.Count() > 0);

            EntityA entityA2Update = results.First();
            entityA2Update.Name = "Maria";
            repo.Modify(entityA2Update);

            this.Commit();

            repo = this.unityContainer.Resolve<IEntityARepository>();
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
            return new NHibernateUnitOfWorkFactory(DbProvider.MsSqlProvider, this.ConnectionString(), string.Empty, typeof(Entity).Assembly);
        }

        private EntityA _Add_EntityA()
        {
            var entityA = new EntityA();
            entityA.Name = "Martin";

            var repo = this.unityContainer.Resolve<IEntityARepository>();
            repo.Add(entityA);

            this.Commit();

            return entityA;
        }
    }
}