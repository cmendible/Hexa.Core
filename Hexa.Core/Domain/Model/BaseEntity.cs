//----------------------------------------------------------------------------------------------
// <copyright file="BaseEntity.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    /// <summary>
    /// Base entity with an abstract key which implements IEquatable.
    /// </summary>
    /// <remarks>
    /// Derived from SharpArch.Core.EntityWithTypedId.
    /// For a discussion of this object, see
    /// http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx
    /// </remarks>
    [Serializable]
    public abstract class BaseEntity<TEntity, TKey> : ValidatableObject<TEntity>, IEquatable<TEntity>, IEntity<TKey>
        where TEntity : BaseEntity<TEntity, TKey>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// To help ensure hashcode uniqueness, a carefully selected random number multiplier
        /// is used within the calculation.  Goodrich and Tamassia's Data Structures and
        /// Algorithms in Java asserts that 31, 33, 37, 39 and 41 will produce the fewest number
        /// of collissions.  See http://computinglife.wordpress.com/2008/11/20/why-do-hash-functions-use-prime-numbers/
        /// for more information.
        /// </summary>
        private const int HASH_MULTIPLIER = 31;

        private int? cachedHashcode;

        /// <summary>
        /// Id may be of type string, int, custom type, etc.
        /// Setter is protected to allow unit tests to set this property via reflection and to allow
        /// domain objects more flexibility in setting this for those objects with assigned Ids.
        /// It's virtual to allow NHibernate-backed objects to be lazily loaded.
        ///
        /// This is ignored for XML serialization because it does not have a public setter (which is very much by design).
        /// See the FAQ within the documentation if you'd like to have the Id XML serialized.
        /// </summary>
        public virtual TKey Id
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        /// <value>The timestamp.</value>
        public virtual string Version
        {
            get;
            protected set;
        }

        /// <summary>
        /// Equalses the specified compare to.
        /// </summary>
        /// <param name="other">The compare to.</param>
        /// <returns></returns>
        public virtual bool Equals(TEntity other)
        {
            return base.Equals(other);
        }

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
            var compareTo = obj as BaseEntity<TEntity, TKey>;

            if (object.ReferenceEquals(this, compareTo))
            {
                return true;
            }

            if (compareTo == null || compareTo is TEntity == false)
            {
                return false;
            }

            if (this.IsTransient())
            {
                return false;
            }

            return HasSameNonDefaultIdAs(compareTo);
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
            if (this.cachedHashcode.HasValue)
            {
                return this.cachedHashcode.Value;
            }

            if (IsTransient())
            {
                this.cachedHashcode = base.GetHashCode();
            }
            else
            {
                unchecked
                {
                    // It's possible for two objects to return the same hash code based on
                    // identically valued properties, even if they're of two different types,
                    // so we include the object's type in the hash calculation
                    int hashCode = this.GetType().GetHashCode();
                    this.cachedHashcode = (hashCode * HASH_MULTIPLIER) ^ this.Id.GetHashCode();
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
            return this.Id.Equals(default(TKey));
        }

        protected virtual Type TypeWithoutProxy()
        {
            return this.GetType();
        }

        /// <summary>
        /// Returns true if self and the provided entity have the same Id values
        /// and the Ids are not of the default Id value
        /// </summary>
        private bool HasSameNonDefaultIdAs(BaseEntity<TEntity, TKey> compareTo)
        {
            return !this.IsTransient() &&
                   !compareTo.IsTransient() &&
                   this.Id.Equals(compareTo.Id);
        }
    }
}