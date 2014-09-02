//----------------------------------------------------------------------------------------------
// <copyright file="SqlTest.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Orm.Tests.NH
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using Hexa.Core.Data;
    using Hexa.Core.Domain;
    using Hexa.Core.Security;
    using Hexa.Core.Tests.Data;
    using Hexa.Core.Tests.Domain;
    using Microsoft.Practices.Unity;
    using NHibernate;
    using NUnit.Framework;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "This is a Test")]
    [TestFixture]
    public class SqlTest
    {
        UnityContainer unityContainer;

        [Test]
        public void Add_EntityA()
        {
            EntityA entityA = this.AddEntityA();

            Assert.IsNotNull(entityA);
            Assert.IsNotNull(entityA.Version);
            Assert.IsFalse(entityA.Id == Guid.Empty);
            Assert.AreEqual("Martin", entityA.Name);
        }

        /**
        * These testcase will show, that an assertion failure "...collection xyz
        * was not processed by flush" will be thrown, if you use following
        * entity-constellation and an PostUpdateListener:
        *
        * You have entities that:
        *
        * <pre>
        * 1.) two entities are having a m:n relation AND
        * 2.) we have defined an PostUpdateListener that iterates through all properties of the entity and
        *     so also through the m:n relation (=Collection)
        * </pre>
        *
        */
        [Test]
        public void Collection_Was_Not_Processed_By_Flush()
        {
            /*
            * create an instance of entity A and an instance of entity B, then link
            * both with each other via an m:n relationship
            */
            EntityA a = this.Create_EntityA_EntityB_And_Many_To_Many_Relation();

            // now update a simple property of EntityA, due to this the
            // MyPostUpdateListener will be called, which iterates through all
            // properties of EntityA (and also the Collection of the m:n relation)
            //--> org.hibernate.AssertionFailure: collection
            // was not processed by flush()
            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                var repo = this.unityContainer.Resolve<IEntityARepository>();
                a = repo.GetFiltered(u => u.Id == a.Id).Single();

                a.Name = "AA";
                repo.Modify(a);
                unitOfWork.Commit();
            }
        }

        [Test]
        public void Delete_EntityA()
        {
            EntityA entityA = this.AddEntityA();

            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                IEntityARepository repo = this.unityContainer.Resolve<IEntityARepository>();
                IEnumerable<EntityA> results = repo.GetFiltered(u => u.Id == entityA.Id);
                Assert.IsTrue(results.Count() > 0);

                EntityA entityA2Delete = results.First();

                repo.Remove(entityA2Delete);
                unitOfWork.Commit();
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
            NHUnitOfWorkFactory ctxFactory = this.CreateNHContextFactory();

            if (!ctxFactory.DatabaseExists())
            {
                ctxFactory.CreateDatabase();
            }

            ctxFactory.ValidateDatabaseSchema();

            NHibernate.ISessionFactory sessionFactory = ctxFactory.CreateSessionFactory();

            this.unityContainer.RegisterInstance<NHibernate.ISessionFactory>(sessionFactory);
            this.unityContainer.RegisterInstance<IDatabaseManager>(ctxFactory);
            this.unityContainer.RegisterInstance<IUnitOfWorkFactory>(ctxFactory);

            this.unityContainer.RegisterType<ISession, ISession>(new InjectionFactory((c) =>
            {
                return ctxFactory.CurrentSession;
            }));

            // Repositories
            this.unityContainer.RegisterType<IEntityARepository, EntityANHRepository>(new PerResolveLifetimeManager());
            this.unityContainer.RegisterType<IEntityBRepository, EntityBNHRepository>(new PerResolveLifetimeManager());

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
        public void RequireNewAndNestedUnitOfWork()
        {
            EntityA entityA = default(EntityA);

            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                entityA = new EntityA();
                entityA.Name = "Martin";

                var repo = this.unityContainer.Resolve<IEntityARepository>();
                repo.Add(entityA);

                using (IUnitOfWork nestesUnitOfWork = UnitOfWork.Start(UnitOfWorkOption.NewAndNested))
                {
                    nestesUnitOfWork.RollbackChanges();
                }
                unitOfWork.Commit();
            }

            Assert.IsNotNull(entityA);
            Assert.IsNotNull(entityA.Version);
            Assert.IsFalse(entityA.Id == Guid.Empty);
            Assert.AreEqual("Martin", entityA.Name);
        }

        [Test]
        public void Query_EntityA()
        {
            EntityA entityA = this.AddEntityA();

            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                var repo = this.unityContainer.Resolve<IEntityARepository>();
                IEnumerable<EntityA> results = repo.GetFiltered(u => u.Id == entityA.Id);
                Assert.IsTrue(results.Count() > 0);
            }
        }

        [Test]
        public void Update_EntityA()
        {
            EntityA entityA = this.AddEntityA();
            Thread.Sleep(1000);

            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                var repo = this.unityContainer.Resolve<IEntityARepository>();
                IEnumerable<EntityA> results = repo.GetFiltered(u => u.Id == entityA.Id);
                Assert.IsTrue(results.Count() > 0);

                EntityA entityA2Update = results.First();
                entityA2Update.Name = "Maria";
                repo.Modify(entityA2Update);
                unitOfWork.Commit();
            }

            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                var repo = this.unityContainer.Resolve<IEntityARepository>();
                entityA = repo.GetFiltered(u => u.Id == entityA.Id).Single();
                Assert.AreEqual("Maria", entityA.Name);
                Assert.Greater(entityA.UpdatedAt, entityA.CreatedAt);
            }
        }

        protected virtual string ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Sql.Connection"].ConnectionString;
        }

        protected virtual NHUnitOfWorkFactory CreateNHContextFactory()
        {
            return new NHUnitOfWorkFactory(DbProvider.MsSqlProvider, this.ConnectionString(), string.Empty, new Assembly[] { Assembly.GetExecutingAssembly() });
        }

        private EntityA AddEntityA()
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                var entityA = new EntityA();
                entityA.Name = "Martin";

                var repo = this.unityContainer.Resolve<IEntityARepository>();
                repo.Add(entityA);

                unitOfWork.Commit();

                return entityA;
            }
        }

        private EntityA Create_EntityA_EntityB_And_Many_To_Many_Relation()
        {
            using (IUnitOfWork unitOfWork = UnitOfWork.Start())
            {
                var a = new EntityA();
                a.Name = "A";

                var b = new EntityB();
                b.Name = "B";

                a.AddB(b);

                var repoA = this.unityContainer.Resolve<IEntityARepository>();
                var repoB = this.unityContainer.Resolve<IEntityBRepository>();

                repoB.Add(b);
                repoA.Add(a);

                unitOfWork.Commit();

                return a;
            }
        }
    }
}