using System;
using System.Configuration;
using System.Linq;
using Hexa.Core.Database;
using Hexa.Core.Domain;
using Hexa.Core.Tests.Domain;
using MbUnit.Framework;

namespace Hexa.Core.Tests.Sql
{
    [TestFixture]
    public class FirebirdTests : BaseDatabaseTest
    {
        protected override NHContextFactory CreateNHContextFactory()
        {
            return new NHContextFactory(DbProvider.Firebird, ConnectionString(), string.Empty, typeof(Entity).Assembly, ApplicationContext.Container);
        }

        protected override string ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Firebird.Connection"].ConnectionString;
        }

        [Test]
        [Rollback]
        public Guid Add_Human()
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

            Assert.IsNotNull(human);
            Assert.IsNotNull(human.Version);
            Assert.IsFalse(human.UniqueId == Guid.Empty);
            Assert.AreEqual("Martin", human.Name);

            return human.UniqueId;
        }

        [Test]
        [Rollback]
        public void Query_Human()
        {
            var uniqueId = Add_Human();

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                var results = repo.GetFilteredElements(u => u.UniqueId == uniqueId);
                Assert.IsTrue(results.Count() > 0);

                results = repo.GetFilteredElements(u => u.isMale);
                Assert.IsTrue(results.Count() > 0);
            }
        }

        [Test]
        [Rollback]
        public void Update_Human()
        {
            var uniqueId = Add_Human();

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                var results = repo.GetFilteredElements(u => u.UniqueId == uniqueId);
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
                var human = repo.GetFilteredElements(u => u.UniqueId == uniqueId).Single();
                Assert.AreEqual("Maria", human.Name);
                Assert.GreaterThan(human.UpdatedAt, human.CreatedAt);
            }
        }

        [Test]
        [Rollback]
        public void Delete_Human()
        {
            var uniqueId = Add_Human();

            var repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                var results = repo.GetFilteredElements(u => u.UniqueId == uniqueId);
                Assert.IsTrue(results.Count() > 0);

                var human2Delete = results.First();

                repo.Remove(human2Delete);

                ctx.Commit();
            }

            repo = ServiceLocator.GetInstance<IHumanRepository>();
            using (var ctx = repo.UnitOfWork)
            {
                Assert.AreEqual(0, repo.GetFilteredElements(u => u.UniqueId == uniqueId).Count());
            }
        }
    }
}
