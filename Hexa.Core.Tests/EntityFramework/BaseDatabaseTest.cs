//#region Header

//// ===================================================================================
//// Copyright 2010 HexaSystems Corporation
//// ===================================================================================
//// Licensed under the Apache License, Version 2.0 (the "License");
//// you may not use this file except in compliance with the License.
//// You may obtain a copy of the License at
//// http://www.apache.org/licenses/LICENSE-2.0
//// ===================================================================================
//// Unless required by applicable law or agreed to in writing, software
//// distributed under the License is distributed on an "AS IS" BASIS,
//// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//// See the License for the specific language governing permissions and
//// See the License for the specific language governing permissions and
//// ===================================================================================

//#endregion Header

//namespace Hexa.Core.Tests.EntityFramework
//{
//    using System;
//    using System.Configuration;
//    using System.Collections.Generic;
//    using System.Linq;
//    using System.Threading;

//    using Core.Data;
//    using Core.Domain;

//    using Data;

//    using Domain;

//    using Logging;

//    using NUnit.Framework;

//    using Security;

//    using Validation;

//    [TestFixture]
//    public class BaseDatabaseTest
//    {
//        #region Methods

//        [Test]
//        public void Add_EntityA()
//        {
//            EntityA entityA = this._Add_EntityA();

//            Assert.IsNotNull(entityA);
//            Assert.IsNotNull(entityA.Version);
//            Assert.IsFalse(entityA.UniqueId == Guid.Empty);
//            Assert.AreEqual("Martin", entityA.Name);
//        }

//        [Test]
//        public void Delete_EntityA()
//        {
//            EntityA entityA = this._Add_EntityA();
//            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
//            {
//                IEntityARepository repo = ServiceLocator.GetInstance<IEntityARepository>();
//                IEnumerable<EntityA> results = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId);
//                Assert.IsTrue(results.Count() > 0);

//                EntityA entityA2Delete = results.First();

//                repo.Remove(entityA2Delete);

//                ctx.Commit();
//            }

//            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
//            {
//                IEntityARepository repo = ServiceLocator.GetInstance<IEntityARepository>();
//                Assert.AreEqual(0, repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId).Count());
//            }
//        }

//        [TestFixtureSetUp]
//        public void FixtureSetup()
//        {
//            ApplicationContext.Start(this.ConnectionString());

//            // Validator and TraceManager
//            IoCContainer container = ApplicationContext.Container;
//            container.RegisterInstance<ILoggerFactory>(new Log4NetLoggerFactory());

//            // Context Factory
//            EntityFrameworkOfWorkFactory<DomainContext> ctxFactory = new EntityFrameworkOfWorkFactory<DomainContext>(this.ConnectionString());

//            container.RegisterInstance<IUnitOfWorkFactory>(ctxFactory);
//            container.RegisterInstance<IDatabaseManager>(ctxFactory);

//            // Repositories
//            container.RegisterType<IEntityARepository, EntityARepository>();

//            // Services

//            if (!ctxFactory.DatabaseExists())
//            {
//                ctxFactory.CreateDatabase();
//            }

//            ctxFactory.ValidateDatabaseSchema();

//            ctxFactory.RegisterSessionFactory(container);

//            ApplicationContext.User =
//                new CorePrincipal(new CoreIdentity("cmendible", "hexa.auth", "cmendible@gmail.com"), new string[] { });
//        }

//        [TestFixtureTearDown]
//        public void FixtureTearDown()
//        {
//            try
//            {
//                var dbManager = ServiceLocator.GetInstance<IDatabaseManager>();
//                dbManager.DeleteDatabase();
//            }
//            finally
//            {
//                ApplicationContext.Stop();
//            }
//        }

//        [Test]
//        public void Query_EntityA()
//        {
//            EntityA entityA = this._Add_EntityA();

//            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
//            {
//                var repo = ServiceLocator.GetInstance<IEntityARepository>();
//                IEnumerable<EntityA> results = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId);
//                Assert.IsTrue(results.Count() > 0);

//                results = repo.GetFilteredElements(u => u.isMale);
//                Assert.IsTrue(results.Count() > 0);
//            }
//        }

//        [Test]
//        public void Update_EntityA()
//        {
//            EntityA entityA = this._Add_EntityA();

//            Thread.Sleep(1000);

//            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
//            {
//                var repo = ServiceLocator.GetInstance<IEntityARepository>();
//                IEnumerable<EntityA> results = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId);
//                Assert.IsTrue(results.Count() > 0);

//                EntityA entityA2Update = results.First();
//                entityA2Update.Name = "Maria";
//                repo.Modify(entityA2Update);

//                ctx.Commit();
//            }

//            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
//            {
//                var repo = ServiceLocator.GetInstance<IEntityARepository>();
//                entityA = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId).Single();
//                Assert.AreEqual("Maria", entityA.Name);
//                Assert.Greater(entityA.UpdatedAt, entityA.CreatedAt);
//            }
//        }

//        [Test]
//        public void Update_EntityA_From_Another_Session()
//        {
//            EntityA entityA = this._Add_EntityA();

//            Thread.Sleep(1000);

//            entityA.Name = "Maria";

//            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
//            {
//                var repo = ServiceLocator.GetInstance<IEntityARepository>();
//                repo.Modify(entityA);
//                ctx.Commit();
//            }

//            using (IUnitOfWork ctx = UnitOfWorkScope.Start())
//            {
//                var repo = ServiceLocator.GetInstance<IEntityARepository>();
//                entityA = repo.GetFilteredElements(u => u.UniqueId == entityA.UniqueId).Single();
//                Assert.AreEqual("Maria", entityA.Name);
//                Assert.Greater(entityA.UpdatedAt, entityA.CreatedAt);
//            }
//        }

//        protected string ConnectionString()
//        {
//            return ConfigurationManager.ConnectionStrings["Sql.Connection"].ConnectionString;
//        }

//        private EntityA _Add_EntityA()
//        {
//            var entityA = new EntityA();
//            entityA.Name = "Martin";
//            entityA.isMale = true;

//            using (IUnitOfWork uow = UnitOfWorkScope.Start())
//            {
//                var repo = ServiceLocator.GetInstance<IEntityARepository>();
//                using (IUnitOfWork ctx = UnitOfWorkScope.Start())
//                {
//                    repo.Add(entityA);
//                    ctx.Commit();
//                }
//                uow.Commit();
//            }

//            return entityA;
//        }

//        #endregion Methods
//    }
//}