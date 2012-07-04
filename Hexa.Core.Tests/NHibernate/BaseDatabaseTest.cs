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

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (IUnitOfWork ctx = repo.UnitOfWork)
            {
                IEnumerable<Human> results = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId);
                Assert.IsTrue(results.Count() > 0);

                Human human2Delete = results.First();

                repo.Remove(human2Delete);

                ctx.Commit();
            }

            repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (IUnitOfWork ctx = repo.UnitOfWork)
            {
                Assert.AreEqual(0, repo.GetFilteredElements(u => u.UniqueId == human.UniqueId).Count());
            }
        }

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            ApplicationContext.Start(this.ConnectionString());

            // Validator and TraceManager
            IoCContainer container = ApplicationContext.Container;
            container.RegisterInstance<ILoggerFactory>(new Log4NetLoggerFactory());
            container.RegisterType<IValidator, DataAnnotationsValidator>();

            // Context Factory
            NHContextFactory ctxFactory = this.CreateNHContextFactory();

            container.RegisterInstance<IUnitOfWorkFactory>(ctxFactory);
            container.RegisterInstance<IDatabaseManager>(ctxFactory);

            // Repositories
            container.RegisterType<IHumanRepository, HumanRepository>();

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
                dbManager.DeleteDatabase();
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

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (IUnitOfWork ctx = repo.UnitOfWork)
            {
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

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (IUnitOfWork ctx = repo.UnitOfWork)
            {
                IEnumerable<Human> results = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId);
                Assert.IsTrue(results.Count() > 0);

                Human human2Update = results.First();
                human2Update.Name = "Maria";
                repo.Modify(human2Update);

                ctx.Commit();
            }

            repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (IUnitOfWork ctx = repo.UnitOfWork)
            {
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

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (IUnitOfWork ctx = repo.UnitOfWork)
            {
                repo.Modify(human);
                ctx.Commit();
            }

            repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (IUnitOfWork ctx = repo.UnitOfWork)
            {
                human = repo.GetFilteredElements(u => u.UniqueId == human.UniqueId).Single();
                Assert.AreEqual("Maria", human.Name);
                Assert.Greater(human.UpdatedAt, human.CreatedAt);
            }
        }

        protected abstract string ConnectionString();

        protected abstract NHContextFactory CreateNHContextFactory();

        private Human _Add_Human()
        {
            var human = new Human();
            human.Name = "Martin";
            human.isMale = true;

            using (IUnitOfWork uow = UnitOfWorkScope.Start())
            {
                var repo = ServiceLocator.GetInstance<IHumanRepository>();
                using (IUnitOfWork ctx = repo.UnitOfWork)
                {
                    repo.Add(human);
                    ctx.Commit();
                }
                uow.Commit();
            }

            return human;
        }

        #endregion Methods
    }
}
