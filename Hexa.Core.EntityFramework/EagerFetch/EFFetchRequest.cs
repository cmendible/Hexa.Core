using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace Hexa.Core.Domain
{
    public class EFFetchRequest<TQueried, TFetch> : IFetchRequest<TQueried, TFetch>
        where TQueried : class
    {
        private IQueryable<TQueried> query;

        public IEnumerator<TQueried> GetEnumerator()
        {
            return this.query.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.query.GetEnumerator();
        }

        public Type ElementType
        {
            get
            {
                return this.query.ElementType;
            }
        }

        public Expression Expression
        {
            get
            {
                return this.query.Expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this.query.Provider;
            }
        }

        public EFFetchRequest(IQueryable<TQueried> query)
        {
            this.query = query;
        }

    }
}