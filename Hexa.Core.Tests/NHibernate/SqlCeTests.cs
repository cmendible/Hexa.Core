using System.Configuration;
using Hexa.Core.Data;
using Hexa.Core.Domain;
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
