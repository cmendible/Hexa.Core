using System;
using NUnit.Framework;
using Hexa.Core.Database;
using Hexa.Core.Domain;
using Hexa.Core.Logging;
using Hexa.Core.Validation;

namespace Hexa.Core.Mono.Tests
{
	[TestFixture()]
	public class Test
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
				cnnString, string.Empty, typeof(Test).Assembly, ApplicationContext.Container);

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
//                ApplicationContext.Stop();
//            }
        }
		
		[Test]
		public void SimpleTest()
		{
			Assert.IsTrue(true);
		}
	}
}

