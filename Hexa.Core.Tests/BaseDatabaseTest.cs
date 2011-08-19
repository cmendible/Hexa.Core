
using Hexa.Core.Database;
using Hexa.Core.Domain;
using Hexa.Core.Logging;
using Hexa.Core.Tests.Data;
using Hexa.Core.Tests.Domain;
using Hexa.Core.Validation;
using MbUnit.Framework;

namespace Hexa.Core.Tests.Sql
{
    public abstract class BaseDatabaseTest
    {
        protected abstract NHContextFactory CreateNHContextFactory();
        protected abstract string ConnectionString(); 

        [FixtureSetUp]
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
        }

        [FixtureTearDown]
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
        
    }
}
