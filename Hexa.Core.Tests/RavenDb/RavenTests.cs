//using System;
//using System.Linq;
//using Hexa.Core.Database;
//using Hexa.Core.Domain;
//using Hexa.Core.Logging;
//using Hexa.Core.Tests.Data;
//using Hexa.Core.Tests.Domain;
//using Hexa.Core.Validation;
//using MbUnit.Framework;

//namespace Hexa.Core.Tests.Sql
//{
//    [TestFixture]
//    public class RavenTests 
//    {
//        [FixtureSetUp]
//        public void FixtureSetup()
//        {
//            ApplicationContext.Start("Data");

//            // Validator and TraceManager
//            var container = ApplicationContext.Container;
//            container.RegisterInstance<ILoggerFactory>(new Log4NetLoggerFactory());
//            container.RegisterType<IValidator, DataAnnotationsValidator>();

//            // Context Factory
//            var ctxFactory = new RavenContextFactory();

//            container.RegisterInstance<IUnitOfWorkFactory>(ctxFactory);
//            container.RegisterInstance<IDatabaseManager>(ctxFactory);

//            // Repositories
//            container.RegisterType<IHumanRepository, HumanRepository>();

//            // Services

//            if (!ctxFactory.DatabaseExists())
//                ctxFactory.CreateDatabase();

//            ctxFactory.ValidateDatabaseSchema();

//            ctxFactory.RegisterSessionFactory(container);
//        }

//        [FixtureTearDown]
//        public void FixtureTearDown()
//        {
//            var dbManager = ServiceLocator.GetInstance<IDatabaseManager>();
//            dbManager.DeleteDatabase();

//            ApplicationContext.Stop();
//        }

//        [Test]
//        [Rollback]
//        public Guid Add_Human()
//        {
//            Human human = new Human();
//            human.Name = "Martin";
//            human.isMale = true;

//            var repo = ServiceLocator.GetInstance<IHumanRepository>();
//            using (var ctx = repo.UnitOfWork)
//            {
//                repo.Add(human);
//                ctx.Commit();
//            }

//            Assert.IsNotNull(human);
//            Assert.IsNotNull(human.Version);
//            Assert.IsFalse(human.Id == Guid.Empty);
//            Assert.AreEqual("Martin", human.Name);

//            return human.Id;
//        }

//        [Test]
//        [Rollback]
//        public void Query_Human()
//        {
//            var uniqueId = Add_Human();

//            var repo = ServiceLocator.GetInstance<IHumanRepository>();
//            using (var ctx = repo.UnitOfWork)
//            {
//                var results = repo.GetFilteredElements(u => u.Id == uniqueId);
//                Assert.IsTrue(results.Count() > 0);

//                results = repo.GetFilteredElements(u => u.isMale);
//                Assert.IsTrue(results.Count() > 0);
//            }
//        }

//        [Test]
//        [Rollback]
//        public void Update_Human()
//        {
//            var uniqueId = Add_Human();

//            var repo = ServiceLocator.GetInstance<IHumanRepository>();
//            using (var ctx = repo.UnitOfWork)
//            {
//                var results = repo.GetFilteredElements(u => u.Id == uniqueId);
//                Assert.IsTrue(results.Count() > 0);

//                var human2Update = results.First();
//                human2Update.Name = "Maria";
//                repo.Modify(human2Update);

//                System.Threading.Thread.Sleep(1000);

//                ctx.Commit();
//            }

//            repo = ServiceLocator.GetInstance<IHumanRepository>();
//            using (var ctx = repo.UnitOfWork)
//            {
//                var human = repo.GetFilteredElements(u => u.Id == uniqueId).Single();
//                Assert.AreEqual("Maria", human.Name);
//                Assert.GreaterThan(human.UpdatedAt, human.CreatedAt);
//            }
//        }

//        [Test]
//        [Rollback]
//        public void Delete_Human()
//        {
//            var uniqueId = Add_Human();

//            var repo = ServiceLocator.GetInstance<IHumanRepository>();
//            using (var ctx = repo.UnitOfWork)
//            {
//                var results = repo.GetFilteredElements(u => u.Id == uniqueId);
//                Assert.IsTrue(results.Count() > 0);

//                var human2Delete = results.First();

//                repo.Remove(human2Delete);

//                ctx.Commit();
//            }

//            repo = ServiceLocator.GetInstance<IHumanRepository>();
//            using (var ctx = repo.UnitOfWork)
//            {
//                Assert.AreEqual(0, repo.GetFilteredElements(u => u.Id == uniqueId).Count());
//            }
//        }
//    }
//}
