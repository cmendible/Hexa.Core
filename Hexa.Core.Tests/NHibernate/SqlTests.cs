using System.Configuration;
using Hexa.Core.Data;
using Hexa.Core.Domain;
using NUnit.Framework;

namespace Hexa.Core.Tests.Sql
{
    [TestFixture]
    public class SqlTests : BaseDatabaseTest
    {
        protected override NHContextFactory CreateNHContextFactory()
        {
            return new NHContextFactory(DbProvider.MsSqlProvider, ConnectionString(), string.Empty, typeof(Entity).Assembly, ApplicationContext.Container);
        }

        protected override string ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Sql.Connection"].ConnectionString;
        }

    }
}
