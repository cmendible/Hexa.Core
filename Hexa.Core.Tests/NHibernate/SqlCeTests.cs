using System;
using System.Configuration;
using System.Linq;
using Hexa.Core.Database;
using Hexa.Core.Domain;
using Hexa.Core.Tests.Domain;
using NUnit.Framework;

namespace Hexa.Core.Tests.Sql
{
    [TestFixture]
    public class SqlCeTests : BaseDatabaseTest
    {
        protected override NHContextFactory CreateNHContextFactory()
        {
            return new NHContextFactory(DbProvider.SqlCe, ConnectionString(), string.Empty, typeof(Entity).Assembly, ApplicationContext.Container);
        }

        protected override string ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["SqlCe.Connection"].ConnectionString;
        }

    }
}
