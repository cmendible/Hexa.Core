using System;
using System.Linq;
using System.Linq.Expressions;


namespace Hexa.Core.Domain.Specification
{

    public enum OrderDirection
    {
        Ascending,
        Descending
    }

    public class OrderBySpecification<TEntity> : IOrderBySpecification<TEntity> where TEntity : class
    {
        #region fields

        private readonly Expression<Func<TEntity, object>> _predicate;
        private readonly Expression<Func<TEntity, object>> _predicate2;
        private bool _descending;
        private bool _descending2;

        #endregion

        #region properties

        public OrderDirection Direction
        {
            get { return _descending ? OrderDirection.Descending : OrderDirection.Ascending; }
            set { _descending = (value == OrderDirection.Descending) ? true : false; }
        }

        public OrderDirection ThenByDirection
        {
            get { return _descending2 ? OrderDirection.Descending : OrderDirection.Ascending; }
            set { _descending2 = (value == OrderDirection.Descending) ? true : false; }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="Specification{TEntity}"/> instnace with the
        /// provided predicate expression.
        /// </summary>
        /// <param name="orderBy">A predicate that can be used to check entities that
        /// satisfy the specification.</param>
        /// <param name="descending">if set to <c>true</c> [descending] order will be used.</param>
        /// <param name="thenBy">The then by.</param>
        /// <param name="thenByDescending">if set to <c>true</c> [then by descending].</param>
        public OrderBySpecification(Expression<Func<TEntity, object>> orderBy, bool descending, Expression<Func<TEntity, object>> thenBy, bool thenByDescending)
        {
            Guard.Against<ArgumentNullException>(orderBy == null, "Expected a non null expression as a predicate for the specification.");
            _predicate = orderBy;
            _descending = descending;
            _predicate2 = thenBy;
            _descending2 = thenByDescending;
        }

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="Specification{TEntity}"/> instnace with the
        /// provided predicate expression.
        /// </summary>
        /// <param name="orderBy">A predicate that can be used to check entities that
        /// satisfy the specification.</param>
        /// <param name="descending">if set to <c>true</c> [descending] order will be used.</param>
        /// <param name="thenBy">The then by.</param>
        public OrderBySpecification(Expression<Func<TEntity, object>> orderBy, bool descending, Expression<Func<TEntity, object>> thenBy)
            : this(orderBy, descending, thenBy, false)
        {
        }

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="Specification{TEntity}"/> instnace with the
        /// provided predicate expression.
        /// </summary>
        /// <param name="orderBy">A predicate that can be used to check entities that
        /// satisfy the specification.</param>
        /// <param name="thenBy">The then by.</param>
        public OrderBySpecification(Expression<Func<TEntity, object>> orderBy, Expression<Func<TEntity, object>> thenBy)
            : this(orderBy, false, thenBy, false)
        {
        }

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="Specification{TEntity}"/> instnace with the
        /// provided predicate expression.
        /// </summary>
        /// <param name="predicate">A predicate that can be used to check entities that
        /// satisfy the specification.</param>
        /// <param name="descending">if set to <c>true</c> [descending] order will be used.</param>
        public OrderBySpecification(Expression<Func<TEntity, object>> predicate, bool descending)
            : this(predicate, descending, null, false)
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="Specification{TEntity}"/> instnace with the
        /// provided predicate expression, with ascending order.
        /// </summary>
        /// <param name="predicate">A predicate that can be used to check entities that
        /// satisfy the specification.</param>
        public OrderBySpecification(Expression<Func<TEntity, object>> predicate)
            : this(predicate, false)
        {
        }

        #endregion

        #region IOrderBySpecification<TEntity> Members

        public IOrderedQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> query)
        {
            var ret = _descending ? query.OrderByDescending(_predicate) : query.OrderBy(_predicate);

            if (_predicate2 != null)
                ret = _descending2 ? ret.ThenByDescending(_predicate2) : ret.ThenBy(_predicate2);

            return ret;
        }

        #endregion
    }
}
