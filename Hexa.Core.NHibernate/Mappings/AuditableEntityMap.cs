//----------------------------------------------------------------------------------------------
// <copyright file="AuditableEntityMap.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    public class AuditableEntityMap<TEntity, TKey> : EntityMap<TEntity, TKey>
        where TEntity : AuditableEntity<TEntity, TKey>
        where TKey : struct, IEquatable<TKey>
    {
        public AuditableEntityMap()
        {
            this.Map(x => x.CreatedAt)
                .Not.Nullable();

            this.Map(x => x.UpdatedAt)
                .Not.Nullable();

            this.Map(x => x.CreatedBy);
            this.Map(x => x.UpdatedBy);
        }
    }

    public class AuditableEntityMap<TEntity> : AuditableEntityMap<TEntity, Guid>
        where TEntity : AuditableEntity<TEntity>
    {

    }
}