namespace Hexa.Core.Mono.Tests
{
    using Data;

    using Domain;

    using Logging;

    using NUnit.Framework;

    using Validation;

    [TestFixture]
    public class PostgreSQLTest
    {
        #region Methods

        [SetUp]
        public void FixtureSetup()
        {
            string cnnString =
                "Server=127.0.0.1;Port=5432;Database=HexaCorePostgreSqlTest;User Id=postgres;Password=password;";

            ApplicationContext.Start(cnnString);

            // Validator and TraceManager
            IoCContainer container = ApplicationContext.Container;
            container.RegisterInstance<ILoggerFactory>(new Log4NetLoggerFactory());
            container.RegisterType<IValidator, DataAnnotationsValidator>();

            // Context Factory
            var ctxFactory = new NHContextFactory(DbProvider.PostgreSQLProvider,
                                                  cnnString, string.Empty, typeof(PostgreSQLTest).Assembly,
                                                  ApplicationContext.Container);

            container.RegisterInstance<IUnitOfWorkFactory>(ctxFactory);
            container.RegisterInstance<IDatabaseManager>(ctxFactory);

            // Repositories
            //container.RegisterType<IHumanRepository, HumanRepository>();

            // Services

            if (!ctxFactory.DatabaseExists())
            {
                ctxFactory.CreateDatabase();
            }

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

        #endregion Methods
    }
}