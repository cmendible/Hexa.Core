namespace Hexa.Core.Domain.Specification
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    #region Enumerations

    public enum OrderDirection
    {
        Ascending,
        Descending
    }

    #endregion Enumerations

    public class OrderBySpecification<TEntity> : IOrderBySpecification<TEntity>
        where TEntity : class
    {
        #region Fields

        private readonly Expression<Func<TEntity, object>> _predicate;
        private readonly Expression<Func<TEntity, object>> _predicate2;

        private bool _descending;
        private bool _descending2;

        #endregion Fields

        #region Constructors

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
        public OrderBySpecification(Expression<Func<TEntity, object>> orderBy, bool descending,
            Expression<Func<TEntity, object>> thenBy, bool thenByDescending)
        {
            Guard.Against<ArgumentNullException>(orderBy == null,
                                                 "Expected a non null expression as a predicate for the specification.");
            this._predicate = orderBy;
            this._descending = descending;
            this._predicate2 = thenBy;
            this._descending2 = thenByDescending;
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
        public OrderBySpecification(Expression<Func<TEntity, object>> orderBy, bool descending,
            Expression<Func<TEntity, object>> thenBy)
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

        #endregion Constructors

        #region Properties

        public OrderDirection Direction
        {
            get
            {
                return this._descending ? OrderDirection.Descending : OrderDirection.Ascending;
            }
            set
            {
                this._descending = (value == OrderDirection.Descending) ? true : false;
            }
        }

        public OrderDirection ThenByDirection
        {
            get
            {
                return this._descending2 ? OrderDirection.Descending : OrderDirection.Ascending;
            }
            set
            {
                this._descending2 = (value == OrderDirection.Descending) ? true : false;
            }
        }

        #endregion Properties

        #region Methods

        public IOrderedQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> query)
        {
            IOrderedQueryable<TEntity> ret = this._descending
                                             ? query.OrderByDescending(this._predicate)
                                             : query.OrderBy(this._predicate);

            if (this._predicate2 != null)
            {
                ret = this._descending2 ? ret.ThenByDescending(this._predicate2) : ret.ThenBy(this._predicate2);
            }

            return ret;
        }

        #endregion Methods
    }
}