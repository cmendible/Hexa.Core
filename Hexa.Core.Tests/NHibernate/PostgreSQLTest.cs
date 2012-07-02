using Hexa.Core.Data;
using Hexa.Core.Domain;
using Hexa.Core.Logging;
using Hexa.Core.Validation;
using NUnit.Framework;

namespace Hexa.Core.Mono.Tests
{
    [TestFixture()]
    public class PostgreSQLTest
    {
        [SetUp]
        public void FixtureSetup()
        {
            var cnnString = "Server=127.0.0.1;Port=5432;Database=HexaCorePostgreSqlTest;User Id=postgres;Password=password;";

            ApplicationContext.Start(cnnString);

            // Validator and TraceManager
            var container = ApplicationContext.Container;
            container.RegisterInstance<ILoggerFactory>(new Log4NetLoggerFactory());
            container.RegisterType<IValidator, DataAnnotationsValidator>();

            // Context Factory
            var ctxFactory = new NHContextFactory(DbProvider.PostgreSQLProvider,
                                                  cnnString, string.Empty, typeof(PostgreSQLTest).Assembly, ApplicationContext.Container);

            container.RegisterInstance<IUnitOfWorkFactory>(ctxFactory);
            container.RegisterInstance<IDatabaseManager>(ctxFactory);

            // Repositories
            //container.RegisterType<IHumanRepository, HumanRepository>();

            // Services

            if (!ctxFactory.DatabaseExists())
                ctxFactory.CreateDatabase();

            ctxFactory.ValidateDatabaseSchema();

            ctxFactory.RegisterSessionFactory(container);
        }

        [TearDown]
        public void FixtureTearDown()
        {
            //            try
            //            {
            //                var dbManager = ServiceLocator.GetInstance<IDatabaseManager>();
            //                dbManager.DeleteDatabase();
            //            }
            //            finally
            //            {
            //
            //            }
            ApplicationContext.Stop();
        }

        [Test]
        [Ignore]
        public void SimpleTest()
        {
            Assert.IsTrue(true);
        }
    }
}

