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

namespace Hexa.Core.Tests.Sql
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

    using Security;

    using Validation;
    using System.ComponentModel.Composition.Hosting;
    using Microsoft.Practices.Unity;
    using System.Configuration;

    public class UnitOfWorkPerTestLifeTimeManager : LifetimeManager
    {
        IUnitOfWork unitOfWork;

        public override object GetValue()
        {
            return unitOfWork;
        }

        public override void RemoveValue()
        {
            unitOfWork = null;
        }

        public override void SetValue(object newValue)
        {
            unitOfWork = newValue as IUnitOfWork;
        }
    }

    [TestFixture]
    public class SqlTest
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

        [Test]
        public void Add_EntityA()
        {
            EntityA entityA = this._Add_EntityA();

            Assert.IsNotNull(entityA);
            Assert.IsNotNull(entityA.Version);
            Assert.IsFalse(entityA.UniqueId == Guid.Empty);
            Assert.AreEqual("Martin", entityA.Name);
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
        }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            //AggregateCatalog catalog = new AggregateCatalog();
            //AssemblyCatalog thisAssembly = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            //catalog.Catalogs.Add(thisAssembly);
            //catalog.Catalogs.Add(new DirectoryCatalog(@"C:\Dev\hexa\Hexa.Core\Hexa.Core.Tests\bin\Release"));

            //CompositionContainer compositionContainer = new CompositionContainer(catalog);

            //Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(() => new Microsoft.Mef.CommonServiceLocator.MefServiceLocator(compositionContainer));

            //IoCContainer containerWrapper = new IoCContainer(
            //    (x, y) => { },
            //    (x, y) => { }
            //);

            //ApplicationContext.Start(containerWrapper, this.ConnectionString());

            unityContainer = new UnityContainer();
            ServiceLocator.Initialize(
                        (x, y) => unityContainer.RegisterType(x, y),
                        (x, y) => unityContainer.RegisterInstance(x, y),
                        (x) => { return unityContainer.Resolve(x); },
                        (x) => { return unityContainer.ResolveAll(x); }
                    );

            unityContainer.RegisterInstance<ILoggerFactory>(new Log4NetLoggerFactory());

            // Context Factory
            NHibernateUnitOfWorkFactory ctxFactory = this.CreateNHContextFactory();

            if (!ctxFactory.DatabaseExists())
            {
                ctxFactory.CreateDatabase();
            }

            ctxFactory.ValidateDatabaseSchema();

            NHibernate.ISessionFactory sessionFactory = ctxFactory.Create();

            unityContainer.RegisterInstance<NHibernate.ISessionFactory>(sessionFactory);
            unityContainer.RegisterInstance<IDatabaseManager>(ctxFactory);

            unityContainer.RegisterType<IUnitOfWork, NHibernateUnitOfWork>(unitOfWorkPerTestLifeTimeManager);

            // Repositories
            unityContainer.RegisterType<IEntityARepository, EntityARepository>(new PerResolveLifetimeManager());
            unityContainer.RegisterType<IEntityBRepository, EntityBRepository>(new PerResolveLifetimeManager());

            // Services

            //ctxFactory.RegisterSessionFactory(container);

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

            repo = unityContainer.Resolve<IEntityARepository>();
            entityA = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId).Single();
            Assert.AreEqual("Maria", entityA.Name);
            Assert.Greater(entityA.UpdatedAt, entityA.CreatedAt);
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
            EntityA a = this._Create_EntityA_EntityB_And_Many_To_Many_Relation();

            //now update a simple property of EntityA, due to this the
            //MyPostUpdateListener will be called, which iterates through all
            //properties of EntityA (and also the Collection of the m:n relation)
            //--> org.hibernate.AssertionFailure: collection
            //was not processed by flush()
            var repo = unityContainer.Resolve<IEntityARepository>();
            a = repo.GetFilteredElements(u => u.UniqueId == a.UniqueId).Single();

            a.Name = "AA";
            repo.Modify(a);
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

            return entityA;
        }

        private EntityA _Create_EntityA_EntityB_And_Many_To_Many_Relation()
        {
            var a = new EntityA();
            a.Name = "A";

            var b = new EntityB();
            b.Name = "B";

            a.AddB(b);

            var repoA = unityContainer.Resolve<IEntityARepository>();
            var repoB = unityContainer.Resolve<IEntityBRepository>();

            repoB.Add(b);
            repoA.Add(a);

            return a;
        }

        #endregion Methods
    }
}