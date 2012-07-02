using System;
using System.Linq;
using Hexa.Core.Domain;
using Hexa.Core.Logging;
using Hexa.Core.Security;
using Hexa.Core.Tests.Data;
using Hexa.Core.Tests.Domain;
using Hexa.Core.Validation;
using NUnit.Framework;
using Hexa.Core.Data;

namespace Hexa.Core.Tests.Sql
{
    public abstract class BaseDatabaseTest
    {
        protected abstract NHContextFactory CreateNHContextFactory();
        protected abstract string ConnectionString();

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            ApplicationContext.Start(ConnectionString());

            // Validator and TraceManager
            var container = ApplicationContext.Container;
            container.RegisterInstance<ILoggerFactory>(new Log4NetLoggerFactory());
            container.RegisterType<IValidator, DataAnnotationsValidator>();

            // Context Factory
            var ctxFactory = CreateNHContextFactory();

            container.RegisterInstance<IUnitOfWorkFactory>(ctxFactory);
            container.RegisterInstance<IDatabaseManager>(ctxFactory);

            // Repositories
            container.RegisterType<IHumanRepository, HumanRepository>();

            // Services

            if (!ctxFactory.DatabaseExists())
                ctxFactory.CreateDatabase();

            ctxFactory.ValidateDatabaseSchema();

            ctxFactory.RegisterSessionFactory(container);

            ApplicationContext.User = new CorePrincipal(new CoreIdentity("cmendible", "hexa.auth", "cmendible@gmail.com"), new string[] { });
        }

        [TestFixtureTearDown]
        public void FixtureTearDown()
        {
            try
                {
                    var dbManager = ServiceLocator.GetInstance<IDatabaseManager>();
                    dbManager.DeleteDatabase();
                }
            finally
                {
                    ApplicationContext.Stop();
                }
        }

        private Human _Add_Human()
        {
            Human human = new Human();
            human.Name = "Martin";
            human.isMale = true;

            using (var uow = UnitOfWorkScope.Start())
            {
                var repo = ServiceLocator.GetInstance<IHumanRepository>();
                using (var ctx = repo.UnitOfWork)
                {
                    repo.Add(human);
                    ctx.Commit();
                }
                uow.Commit();
            }

            return human;
        }

        [Test]
        public void Add_Human()
        {
            Human human = _Add_Human();

            Assert.IsNotNull(human);
            Assert.IsNotNull(human.Version);
            Assert.IsFalse(human.UniqueId == Guid.Empty);
            Assert.AreEqual("Martin", human.Name);
        }

        [Test]
        public void Query_Human()
        {
            var human = _Add_Human();

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
            var human = _Add_Human();

            System.Threading.Thread.Sleep(1000);

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                var results = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId);
                Assert.IsTrue(results.Count() > 0);

                var human2Update = results.First();
                human2Update.Name = "Maria";
                repo.Modify(human2Update);

                ctx.Commit();
            }

            repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                human = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId).Single();
                Assert.AreEqual("Maria", human.Name);
                Assert.Greater(human.UpdatedAt, human.CreatedAt);
            }
        }

        [Test]
        public void Update_Human_From_Another_Session()
        {
            var human = _Add_Human();

            System.Threading.Thread.Sleep(1000);

            human.Name = "Maria";

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                repo.Modify(human);
                ctx.Commit();
            }

            repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                human = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId).Single();
                Assert.AreEqual("Maria", human.Name);
                Assert.Greater(human.UpdatedAt, human.CreatedAt);
            }
        }

        [Test]
        public void Delete_Human()
        {
            var human = _Add_Human();

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
