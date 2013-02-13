#region Header

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion Header

#if MONO

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

        #endregion Methods
    }
}

#endif