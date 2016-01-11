#if !MONO

//----------------------------------------------------------------------------------------------
// <copyright file="RavenTests.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.RavenDb.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Core.Data;
    using Core.Domain;
    using Hexa.Core.Tests.Data;
    using Hexa.Core.Tests.Domain;
    using Microsoft.Practices.Unity;
    using NUnit.Framework;
    using Raven.Client;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This is a Test")]
    [TestFixture]
    public class RavenTests
    {
        UnityContainer unityContainer;

        [Test]
        public void Add_EntityA()
        {
            EntityA entityA = this.AddEntityA();

            Assert.IsNotNull(entityA);
            //Assert.IsNotNull(entityA.Version);
            Assert.IsFalse(entityA.Id == Guid.Empty);
            Assert.AreEqual("Martin", entityA.Name);
        }

        [Test]
        public void Delete_EntityA()
        {
            EntityA entityA = this.AddEntityA();

            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                var repo = IoC.GetInstance<IEntityARepository>();
                IEnumerable<EntityA> results = repo.GetFiltered(u => u.Id == entityA.Id);
                Assert.IsTrue(results.Count() > 0);

                EntityA entityA2Delete = results.First();

                repo.Remove(entityA2Delete);

                unitOfWork.Commit();
            }

            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                var repo = IoC.GetInstance<IEntityARepository>();
                Assert.AreEqual(0, repo.GetFiltered(u => u.Id == entityA.Id).Count());
            }
        }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            this.unityContainer = new UnityContainer();
            IoC.Initialize(
                (x, y) => this.unityContainer.RegisterType(x, y),
                (x, y) => this.unityContainer.RegisterInstance(x, y),
                (x) => { return unityContainer.Resolve(x); },
                (x) => { return unityContainer.ResolveAll(x); });

            // Context Factory
            RavenUnitOfWorkFactory ctxFactory = new RavenUnitOfWorkFactory();
            IDocumentStore sessionFactory = ctxFactory.CreateDocumentStore();

            IoC.RegisterInstance<IDatabaseManager>(ctxFactory);
            IoC.RegisterInstance<IUnitOfWorkFactory>(ctxFactory);

            this.unityContainer.RegisterType<IDocumentSession, IDocumentSession>(new InjectionFactory((c) =>
            {
                return ctxFactory.CurrentDocumentSession;
            }));

            // Repositories
            this.unityContainer.RegisterType<IEntityARepository, EntityARavenRepository>();

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
            var dbManager = IoC.GetInstance<IDatabaseManager>();
            dbManager.DeleteDatabase();
        }

        [Test]
        public void Query_EntityA()
        {
            EntityA entityA = this.AddEntityA();

            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                var repo = IoC.GetInstance<IEntityARepository>();
                IEnumerable<EntityA> results = repo.GetFiltered(u => u.Id == entityA.Id);
                Assert.IsTrue(results.Count() > 0);
            }
        }

        [Test]
        public void Update_EntityA()
        {
            EntityA entityA = this.AddEntityA();

            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                var repo = IoC.GetInstance<IEntityARepository>();
                IEnumerable<EntityA> results = repo.GetFiltered(u => u.Id == entityA.Id);
                Assert.IsTrue(results.Count() > 0);

                EntityA entityA2Update = results.First();
                entityA2Update.Name = "Maria";
                repo.Modify(entityA2Update);

                unitOfWork.Commit();
            }

            Thread.Sleep(1000);

            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                var repo = IoC.GetInstance<IEntityARepository>();
                EntityA entityA2 = repo.GetFiltered(u => u.Id == entityA.Id).Single();
                Assert.AreEqual("Maria", entityA2.Name);

                //Assert.Greater(entityA2.UpdatedAt, entityA2.CreatedAt);
            }
        }

        private EntityA AddEntityA()
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                var entityA = new EntityA();
                entityA.Name = "Martin";

                var repo = IoC.GetInstance<IEntityARepository>();
                repo.Add(entityA);

                unitOfWork.Commit();

                return entityA;
            }
        }
    }
}

#endif