namespace Hexa.Core.Tests.Sql
{
    using System.Configuration;

    using Core.Data;
    using Core.Domain;

    using NUnit.Framework;

    [TestFixture]
    public class SqlTests : BaseDatabaseTest
    {
        #region Methods

        protected override string ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Sql.Connection"].ConnectionString;
        }

        protected override NHContextFactory CreateNHContextFactory()
        {
            return new NHContextFactory(DbProvider.MsSqlProvider, ConnectionString(), string.Empty,
                                        typeof(Entity).Assembly, ApplicationContext.Container);
        }

        #endregion Methods
    }
}
