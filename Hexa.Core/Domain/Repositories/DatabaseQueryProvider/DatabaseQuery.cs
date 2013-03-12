using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Hexa.Core.Domain
{
    public static class DatabaseQuery
    {
        public static Func<IDatabaseQueryProvider> DatabaseQueryProvider;

        public static IList<TEntity> ExecuteQuery<TEntity>(string queryName, IDictionary<string, object> parameters) 
        {
            return DatabaseQueryProvider().ExecuteQuery<TEntity>(queryName, parameters);
        }

    }
}