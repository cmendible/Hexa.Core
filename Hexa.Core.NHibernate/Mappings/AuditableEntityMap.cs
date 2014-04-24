//----------------------------------------------------------------------------------------------
// <copyright file="AuditableEntityMap.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    public class AuditableEntityMap<TEntity> : EntityMap<TEntity, Guid>
        where TEntity : AuditableEntity<TEntity>
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
}