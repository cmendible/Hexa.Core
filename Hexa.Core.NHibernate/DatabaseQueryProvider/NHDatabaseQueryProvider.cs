using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;
using NHibernate;

namespace Hexa.Core.Domain
{
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
