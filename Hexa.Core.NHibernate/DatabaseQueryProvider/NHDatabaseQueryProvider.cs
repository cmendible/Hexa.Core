namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    using NHibernate;
    using NHibernate.Linq;

    public class NHDatabaseProvider : IDatabaseQueryProvider
    {
        public IList<TEntity> ExecuteQuery<TEntity>(string queryName, IDictionary<string, object> parameters)
        {
            INHibernateUnitOfWork unitOfWork = ServiceLocator.GetInstance<IUnitOfWork>() as INHibernateUnitOfWork;
            IQuery query = unitOfWork.Session.GetNamedQuery(queryName);
            foreach (var parameter in parameters)
            {
                query.SetParameter(parameter.Key, parameter.Value);
            }

            return query.List<TEntity>();
        }
    }
}