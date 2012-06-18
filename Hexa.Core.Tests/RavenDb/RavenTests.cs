using System;
using System.Linq;
using Hexa.Core.Data;
using Hexa.Core.Domain;
using Hexa.Core.Logging;
using Hexa.Core.Tests.Data;
using Hexa.Core.Tests.Domain;
using Hexa.Core.Validation;
using NUnit.Framework;

namespace Hexa.Core.Tests.Raven
{
    [TestFixture]
    public class RavenTests
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            ApplicationContext.Start("Data");

            // Validator and TraceManager
            var container = ApplicationContext.Container;
            container.RegisterInstance<ILoggerFactory>(new Log4NetLoggerFactory());
            container.RegisterType<IValidator, DataAnnotationsValidator>();

            // Context Factory
            var ctxFactory = new RavenContextFactory();

            container.RegisterInstance<IUnitOfWorkFactory>(ctxFactory);
            container.RegisterInstance<IDatabaseManager>(ctxFactory);

            // Repositories
            container.RegisterType<IHumanRepository, HumanRepository>();

            // Services

            if (!ctxFactory.DatabaseExists())
                ctxFactory.CreateDatabase();

            ctxFactory.ValidateDatabaseSchema();

            ctxFactory.RegisterSessionFactory(container);
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            var dbManager = ServiceLocator.GetInstance<IDatabaseManager>();
            dbManager.DeleteDatabase();

            ApplicationContext.Stop();
        }

        private Human _Add_Human()
        {
            Human human = new Human();
            human.Name = "Martin";
            human.isMale = true;

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                repo.Add(human);
                ctx.Commit();
            }

            return human;
        }

        [Test]
        public void Add_Human()
        {
            Human human = _Add_Human();

            Assert.IsNotNull(human);
            //Assert.IsNotNull(human.Version);
            Assert.IsFalse(human.UniqueId == Guid.Empty);
            Assert.AreEqual("Martin", human.Name);

            //return human.UniqueId;
        }

        [Test]
        public void Query_Human()
        {
            Human human = _Add_Human();

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                var results = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId);
                Assert.IsTrue(results.Count() > 0);

                results = repo.GetFilteredElements(u => u.isMale);
                Assert.IsTrue(results.Count() > 0);
            }
        }

        [Test]
        public void Update_Human()
        {
            Human human = _Add_Human();

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                var results = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId);
                Assert.IsTrue(results.Count() > 0);

                var human2Update = results.First();
                human2Update.Name = "Maria";
                repo.Modify(human2Update);

                System.Threading.Thread.Sleep(1000);

                ctx.Commit();
            }

            repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                var human2 = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId).Single();
                Assert.AreEqual("Maria", human2.Name);
                //Assert.Greater(human2.UpdatedAt, human2.CreatedAt);

            }
        }

        [Test]
        public void Delete_Human()
        {
            Human human = _Add_Human();

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                var results = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId);
                Assert.IsTrue(results.Count() > 0);

                var human2Delete = results.First();

                repo.Remove(human2Delete);

                ctx.Commit();
            }

            repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                Assert.AreEqual(0, repo.GetFilteredElements(u => u.UniqueId == human.UniqueId).Count());
            }
        }
    }
}
