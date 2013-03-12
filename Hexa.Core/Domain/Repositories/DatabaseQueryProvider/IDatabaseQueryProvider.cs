using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Hexa.Core.Domain
{
    public interface IDatabaseQueryProvider
    {
        IList<TEntity> ExecuteQuery<TEntity>(string queryName, IDictionary<string, object> parameters);
    }
}