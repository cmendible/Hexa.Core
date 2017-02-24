//----------------------------------------------------------------------------------------------
// <copyright file="AuditableEntityConfiguration.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class AuditableEntityConfiguration<TEntity> : EntityConfiguration<TEntity, Guid>
        where TEntity : AuditableEntity<TEntity>
    {
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            base.Map(builder);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy);
            builder.Property(x => x.UpdatedBy);
        }
    }

    public class TenantScopedEntityConfiguration<TEntity> : EntityConfiguration<TEntity, Guid>
        where TEntity : TenantScopedEntity<TEntity>
    {
        public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            base.Map(builder);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.UpdatedAt)
                .IsRequired();

            builder.Property(x => x.CreatedBy);
            builder.Property(x => x.UpdatedBy);

            builder.Property(x => x.TenantId);
        }
    }
}