#if !MONO

namespace Hexa.Core.Tests.Sql
{
    using System.Configuration;

    using Hexa.Core.Data;
    using Hexa.Core.Domain;

    using NUnit.Framework;

    [TestFixture]
    public class SqlCeTests : BaseDatabaseTest
    {
        #region Methods

        protected override string ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["SqlCe.Connection"].ConnectionString;
        }

        protected override NHibernateUnitOfWorkFactory CreateNHContextFactory()
        {
            return new NHibernateUnitOfWorkFactory(DbProvider.SqlCe, ConnectionString(), string.Empty, typeof(Entity).Assembly, ApplicationContext.Container);
        }

        #endregion Methods
    }
}

#endif