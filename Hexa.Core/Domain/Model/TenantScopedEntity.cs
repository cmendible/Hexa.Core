//----------------------------------------------------------------------------------------------
// <copyright file="TenantScopedEntity.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    [Serializable]
    public abstract class TenantScopedEntity<TEntity, TKey> : AuditableEntity<TEntity, TKey>, IAuditableEntity, ITenantScopedEntity
        where TEntity : AuditableEntity<TEntity, TKey>
        where TKey : struct, IEquatable<TKey>
    {
        public virtual Guid? TenantId
        {
            get;
            protected set;
        }

        public virtual void SetTenantId(Guid? tenantId)
        {
            this.TenantId = tenantId;
        }
    }

    [Serializable]
    public abstract class TenantScopedEntity<TEntity> : TenantScopedEntity<TEntity, Guid>
        where TEntity : AuditableEntity<TEntity, Guid>
    {

    }
}