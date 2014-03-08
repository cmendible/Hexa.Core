#if MONO

//----------------------------------------------------------------------------------------------
// <copyright file="PostgreSQLTest.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Orm.Tests.NH
{
    using Data;

    using Domain;

    using Logging;

    using NUnit.Framework;

    using Validation;

    [TestFixture]
    public class PostgreSQLTest
    {
        [SetUp]
        public void FixtureSetup()
        {
            string cnnString =
                "Server=127.0.0.1;Port=5432;Database=HexaCorePostgreSqlTest;User Id=postgres;Password=password;";

            ApplicationContext.Start(cnnString);

            // Validator and TraceManager
            IoCContainer container = ApplicationContext.Container;
            container.RegisterInstance<ILoggerFactory>(new Log4NetLoggerFactory());

            // Context Factory
            var ctxFactory = new NHibernateUnitOfWorkFactory(DbProvider.PostgreSQLProvider,
                    cnnString, string.Empty, typeof(PostgreSQLTest).Assembly,
                    ApplicationContext.Container);

            container.RegisterInstance<IUnitOfWorkFactory>(ctxFactory);
            container.RegisterInstance<IDatabaseManager>(ctxFactory);

            // Repositories
            //container.RegisterType<IEntityARepository, EntityARepository>();

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
        public void SimpleTest()
        {
            Assert.IsTrue(true);
        }
    }
}

#endif