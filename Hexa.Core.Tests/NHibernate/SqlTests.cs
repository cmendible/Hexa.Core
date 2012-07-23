#if !MONO

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

        protected override NHibernateUnitOfWorkFactory CreateNHContextFactory()
        {
            return new NHibernateUnitOfWorkFactory(DbProvider.MsSqlProvider, ConnectionString(), string.Empty,
                                                   typeof(Entity).Assembly, ApplicationContext.Container);
        }

        #endregion Methods
    }
}

#endif