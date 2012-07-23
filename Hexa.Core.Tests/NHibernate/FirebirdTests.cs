#if !MONO 
namespace Hexa.Core.Tests.Sql
{
    using System.Configuration;

    using Core.Data;
    using Core.Domain;

    using NUnit.Framework;

    [TestFixture]
    public class FirebirdTests : BaseDatabaseTest
    {
        #region Methods

        protected override string ConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["Firebird.Connection"].ConnectionString;
        }

        protected override NHibernateUnitOfWorkFactory CreateNHContextFactory()
        {
            return new NHibernateUnitOfWorkFactory(DbProvider.Firebird, ConnectionString(), string.Empty, typeof(Entity).Assembly,
                                        ApplicationContext.Container);
        }

        #endregion Methods
    }
}
#endif