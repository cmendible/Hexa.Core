//----------------------------------------------------------------------------------------------
// <copyright file="AuditableEntity.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    public abstract class AuditableEntity<TEntity, TKey> : BaseEntity<TEntity, TKey>, IAuditableEntity
        where TEntity : BaseEntity<TEntity, TKey>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the date on which object was created.
        /// </summary>
        /// <value>The creation date.</value>
        public virtual DateTime CreatedAt
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public virtual string CreatedBy
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the updated at.
        /// </summary>
        /// <value>
        /// The updated at.
        /// </value>
        public virtual DateTime UpdatedAt
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the updated by.
        /// </summary>
        /// <value>
        /// The updated by.
        /// </value>
        public virtual string UpdatedBy
        {
            get;
            set;
        }
    }

    public abstract class AuditableEntity<TEntity> : AuditableEntity<TEntity, Guid>
        where TEntity : BaseEntity<TEntity, Guid>
    { 
    
    }
}