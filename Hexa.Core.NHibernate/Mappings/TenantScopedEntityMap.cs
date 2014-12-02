//----------------------------------------------------------------------------------------------
// <copyright file="TenantScopedEntityMap.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    public class TenantScopedEntityMap<TEntity, TKey> : AuditableEntityMap<TEntity, TKey>
        where TEntity : TenantScopedEntity<TEntity, TKey>
        where TKey : struct, IEquatable<TKey>
    {
        public TenantScopedEntityMap()
        {
            this.Map(x => x.TenantId)
                .Nullable();
        }
    }

    public class TenantScopedEntityMap<TEntity> : TenantScopedEntityMap<TEntity, Guid>
        where TEntity : TenantScopedEntity<TEntity, Guid>
    {

    }
}