//Copyright (c) 2009, Codai, Inc.
//All rights reserved.
namespace Hexa.Core.Domain
{
    using System;

    /// <summary>
    /// Base entity with an abstract key.
    /// </summary>
    /// <remarks>
    /// Derived from SharpArch.Core.EntityWithTypedId.
    /// For a discussion of this object, see
    /// http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx
    /// </remarks>
    [Serializable]
    public abstract class BaseEntity<TKey> : ValidatableObject<BaseEntity<TKey>>
        where TKey : IEquatable<TKey>
    {
        #region Fields

        /// <summary>
        /// To help ensure hashcode uniqueness, a carefully selected random number multiplier
        /// is used within the calculation.  Goodrich and Tamassia's Data Structures and
        /// Algorithms in Java asserts that 31, 33, 37, 39 and 41 will produce the fewest number
        /// of collissions.  See http://computinglife.wordpress.com/2008/11/20/why-do-hash-functions-use-prime-numbers/
        /// for more information.
        /// </summary>
        private const int HASH_MULTIPLIER = 31;

        private int? cachedHashcode;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Id may be of type string, int, custom type, etc.
        /// Setter is protected to allow unit tests to set this property via reflection and to allow
        /// domain objects more flexibility in setting this for those objects with assigned Ids.
        /// It's virtual to allow NHibernate-backed objects to be lazily loaded.
        ///
        /// This is ignored for XML serialization because it does not have a public setter (which is very much by design).
        /// See the FAQ within the documentation if you'd like to have the Id XML serialized.
        /// </summary>
        protected virtual TKey EntityId
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            var compareTo = obj as BaseEntity<TKey>;

            if (ReferenceEquals(this, compareTo))
            {
                return true;
            }

            if (compareTo == null || !GetType().Equals(compareTo.TypeWithoutProxy()))
            {
                return false;
            }

            if (HasSameNonDefaultIdAs(compareTo))
            {
                return true;
            }

            // Since the Ids aren't the same, both of them must be transient to
            // compare domain signatures; because if one is transient and the
            // other is a persisted entity, then they cannot be the same object.
            return IsTransient() && compareTo.IsTransient(); //&& HasSameObjectSignatureAs(compareTo);
        }

        /// <summary>
        /// This is used to provide the hashcode identifier of an object using the signature
        /// properties of the object; although it's necessary for NHibernate's use, this can
        /// also be useful for business logic purposes and has been included in this base
        /// class, accordingly.  Since it is recommended that GetHashCode change infrequently,
        /// if at all, in an object's lifetime, it's important that properties are carefully
        /// selected which truly represent the signature of an object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // Once we have a hash code we'll never change it
            if (cachedHashcode.HasValue)
            {
                return cachedHashcode.Value;
            }

            if (IsTransient())
            {
                cachedHashcode = base.GetHashCode();
            }
            else
            {
                unchecked
                {
                    // It's possible for two objects to return the same hash code based on
                    // identically valued properties, even if they're of two different types,
                    // so we include the object's type in the hash calculation
                    int hashCode = GetType().GetHashCode();
                    cachedHashcode = (hashCode*HASH_MULTIPLIER) ^ EntityId.GetHashCode();
                }
            }

            return cachedHashcode.Value;
        }

        /// <summary>
        /// Transient objects are not associated with an item already in storage.  For instance,
        /// a Customer is transient if its Id is 0.  It's virtual to allow NHibernate-backed
        /// objects to be lazily loaded.
        /// </summary>
        public virtual bool IsTransient()
        {
            return EntityId == null || EntityId.Equals(default(TKey));
        }

        protected virtual Type TypeWithoutProxy()
        {
            return GetType();
        }

        /// <summary>
        /// Returns true if self and the provided entity have the same Id values
        /// and the Ids are not of the default Id value
        /// </summary>
        private bool HasSameNonDefaultIdAs(BaseEntity<TKey> compareTo)
        {
            return !IsTransient() &&
                   !compareTo.IsTransient() &&
                   EntityId.Equals(compareTo.EntityId);
        }

        #endregion Methods
    }

    /// <summary>
    /// Base entity with an abstract key which implements IEquatable.
    /// </summary>
    /// <remarks>
    /// Derived from SharpArch.Core.EntityWithTypedId.
    /// For a discussion of this object, see
    /// http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx
    /// </remarks>
    [Serializable]
    public abstract class BaseEntity<TEntity, TKey> : BaseEntity<TKey>, IEquatable<TEntity>
        where TKey : IEquatable<TKey>
    {
        #region Methods

        /// <summary>
        /// Equalses the specified compare to.
        /// </summary>
        /// <param name="other">The compare to.</param>
        /// <returns></returns>
        public virtual bool Equals(TEntity other)
        {
            return base.Equals(other);
        }

        #endregion Methods
    }

    /// <summary>
    /// BaseEntity with a long Primary Id.
    /// </summary>
    [Serializable]
    public abstract class Entity<TEntity> : BaseEntity<TEntity, long>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Entity's primary Id.
        /// Setter is protected to allow unit tests to set this property via reflection and to allow
        /// domain objects more flexibility in setting this for those objects with assigned Ids.
        /// It's virtual to allow NHibernate-backed objects to be lazily loaded.
        /// This is ignored for XML serialization because it does not have a public setter (which is very much by design).
        /// See the FAQ within the documentation if you'd like to have the Id XML serialized.
        /// </summary>
        /// <value></value>
        public virtual long Id
        {
            get
            {
                return base.EntityId;
            }
            protected set
            {
                base.EntityId = value;
            }
        }

        #endregion Properties
    }

    /// <summary>
    /// BaseEntity with an Guid/UniqueId Primary Id.
    /// </summary>
    [Serializable]
    public abstract class EntityWithUniqueId<TEntity> : BaseEntity<TEntity, Guid>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Entity's primary Id.
        /// Setter is protected to allow unit tests to set this property via reflection and to allow
        /// domain objects more flexibility in setting this for those objects with assigned Ids.
        /// It's virtual to allow NHibernate-backed objects to be lazily loaded.
        /// This is ignored for XML serialization because it does not have a public setter (which is very much by design).
        /// See the FAQ within the documentation if you'd like to have the Id XML serialized.
        /// </summary>
        /// <value></value>
        public virtual Guid UniqueId
        {
            get
            {
                return base.EntityId;
            }
            protected set
            {
                base.EntityId = value;
            }
        }

        #endregion Properties
    }
}