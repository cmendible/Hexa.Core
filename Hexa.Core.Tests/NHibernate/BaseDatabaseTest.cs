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

    public abstract class BaseDatabaseTest
    {
        #region Methods

        [Test]
        public void Add_Human()
        {
            Human human = this._Add_Human();

            Assert.IsNotNull(human);
            Assert.IsNotNull(human.Version);
            Assert.IsFalse(human.UniqueId == Guid.Empty);
            Assert.AreEqual("Martin", human.Name);
        }

        [Test]
        public void Delete_Human()
        {
            Human human = this._Add_Human();
            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
            {
                IHumanRepository repo = ServiceLocator.GetInstance<IHumanRepository>();
                IEnumerable<Human> results = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId);
                Assert.IsTrue(results.Count() > 0);

                Human human2Delete = results.First();

                repo.Remove(human2Delete);

                ctx.Commit();
            }

            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
            {
                IHumanRepository repo = ServiceLocator.GetInstance<IHumanRepository>();
                Assert.AreEqual(0, repo.GetFilteredElements(u => u.UniqueId == human.UniqueId).Count());
            }
        }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            //AggregateCatalog catalog = new AggregateCatalog();
            //AssemblyCatalog thisAssembly = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            //catalog.Catalogs.Add(thisAssembly);
            //catalog.Catalogs.Add(new DirectoryCatalog(@"C:\Dev\hexa\Hexa.Core\Hexa.Core.Tests\bin\Debug"));

            //CompositionContainer compositionContainer = new CompositionContainer(catalog);

            //Microsoft.Practices.ServiceLocation.ServiceLocator.SetLocatorProvider(() => new Microsoft.Mef.CommonServiceLocator.MefServiceLocator(compositionContainer));

            //IoCContainer containerWrapper = new IoCContainer(
            //    (x, y) => { },
            //    (x, y) => { }
            //);

            //ApplicationContext.Start(containerWrapper, this.ConnectionString());

            ApplicationContext.Start(this.ConnectionString());

            // Validator and TraceManager
            IoCContainer container = ApplicationContext.Container;
            container.RegisterInstance<ILoggerFactory>(new Log4NetLoggerFactory());

            // Context Factory
            NHibernateUnitOfWorkFactory ctxFactory = this.CreateNHContextFactory();

            container.RegisterInstance<IUnitOfWorkFactory>(ctxFactory);
            container.RegisterInstance<IDatabaseManager>(ctxFactory);

            // Repositories
            container.RegisterType<IHumanRepository, HumanRepository>();
            container.RegisterType<IEntityARepository, EntityARepository>();
            container.RegisterType<IEntityBRepository, EntityBRepository>();

            // Services

            if (!ctxFactory.DatabaseExists())
            {
                ctxFactory.CreateDatabase();
            }

            ctxFactory.ValidateDatabaseSchema();

            ctxFactory.RegisterSessionFactory(container);

            ApplicationContext.User =
                new CorePrincipal(new CoreIdentity("cmendible", "hexa.auth", "cmendible@gmail.com"), new string[] {});
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            try
            {
                var dbManager = ServiceLocator.GetInstance<IDatabaseManager>();
                //dbManager.DeleteDatabase();
            }
            finally
            {
                ApplicationContext.Stop();
            }
        }

        [Test]
        public void Query_Human()
        {
            Human human = this._Add_Human();

            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
            {
                var repo = ServiceLocator.GetInstance<IHumanRepository>();
                IEnumerable<Human> results = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId);
                Assert.IsTrue(results.Count() > 0);

                results = repo.GetFilteredElements(u => u.isMale);
                Assert.IsTrue(results.Count() > 0);
            }
        }

        [Test]
        public void Update_Human()
        {
            Human human = this._Add_Human();

            Thread.Sleep(1000);

            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
            {
                var repo = ServiceLocator.GetInstance<IHumanRepository>();
                IEnumerable<Human> results = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId);
                Assert.IsTrue(results.Count() > 0);

                Human human2Update = results.First();
                human2Update.Name = "Maria";
                repo.Modify(human2Update);

                ctx.Commit();
            }

            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
            {
                var repo = ServiceLocator.GetInstance<IHumanRepository>();
                human = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId).Single();
                Assert.AreEqual("Maria", human.Name);
                Assert.Greater(human.UpdatedAt, human.CreatedAt);
            }
        }

        [Test]
        public void Update_Human_From_Another_Session()
        {
            Human human = this._Add_Human();

            Thread.Sleep(1000);

            human.Name = "Maria";

            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
            {
                var repo = ServiceLocator.GetInstance<IHumanRepository>();
                repo.Modify(human);
                ctx.Commit();
            }

            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
            {
                var repo = ServiceLocator.GetInstance<IHumanRepository>();
                human = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId).Single();
                Assert.AreEqual("Maria", human.Name);
                Assert.Greater(human.UpdatedAt, human.CreatedAt);
            }
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

            // now update a simple property of EntityA, due to this the
            // MyPostUpdateListener will be called, which iterates through all
            // properties of EntityA (and also the Collection of the m:n relation)
            // --> org.hibernate.AssertionFailure: collection
            // was not processed by flush()
            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
            {
                var repo = ServiceLocator.GetInstance<IEntityARepository>();
                a = repo.GetFilteredElements(u => u.UniqueId == a.UniqueId).Single();

                a.Name = "AA";
                repo.Modify(a);
                ctx.Commit();
            }
        }

        protected abstract string ConnectionString();

        protected abstract NHibernateUnitOfWorkFactory CreateNHContextFactory();

        private Human _Add_Human()
        {
            var human = new Human();
            human.Name = "Martin";
            human.isMale = true;

            using (IUnitOfWork uow = UnitOfWorkScope.Start())
            {
                var repo = ServiceLocator.GetInstance<IHumanRepository>();
                using (IUnitOfWork ctx = UnitOfWorkScope.Start())
                {
                    repo.Add(human);
                    ctx.Commit();
                }
                uow.Commit();
            }

            return human;
        }

        private EntityA _Create_EntityA_EntityB_And_Many_To_Many_Relation()
        {
            var a = new EntityA();
            a.Name = "A";
            
            var b = new EntityB();
            b.Name = "B";

            a.AddB(b);

            using (IUnitOfWork uow = UnitOfWorkScope.Start())
            {
                var repoA = ServiceLocator.GetInstance<IEntityARepository>();
                var repoB = ServiceLocator.GetInstance<IEntityBRepository>();

                repoB.Add(b);
                repoA.Add(a);
                
                uow.Commit();
            }

            return a;
        }

        #endregion Methods
    }
}