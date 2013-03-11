using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NHibernate.Linq;

namespace Hexa.Core.Domain
{
    public class NHFetchRequest<TQueried, TFetch> : IFetchRequest<TQueried, TFetch>
    {
        private INhFetchRequest<TQueried, TFetch> fetchRequest;

        public IEnumerator<TQueried> GetEnumerator()
        {
            return this.fetchRequest.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.fetchRequest.GetEnumerator();
        }

        public Type ElementType
        {
            get
            {
                return this.fetchRequest.ElementType;
            }
        }

        public Expression Expression
        {
            get
            {
                return this.fetchRequest.Expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this.fetchRequest.Provider;
            }
        }

        public NHFetchRequest(INhFetchRequest<TQueried, TFetch> nhFetchRequest)
        {
            this.fetchRequest = nhFetchRequest;
        }

        public INhFetchRequest<TQueried, TFetch> FetchRequest
        {
            get
            {
                return this.fetchRequest;
            }
        }

    }
}