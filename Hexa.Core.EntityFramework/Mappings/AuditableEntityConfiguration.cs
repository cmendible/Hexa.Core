//----------------------------------------------------------------------------------------------
// <copyright file="AuditableEntityConfiguration.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Data.Entity.ModelConfiguration;

    public class AuditableEntityConfiguration<TEntity> : EntityConfiguration<TEntity, Guid>
        where TEntity : AuditableEntity<TEntity>
    {
        public AuditableEntityConfiguration()
        {
            this.Property(x => x.CreatedAt)
                .IsRequired();

            this.Property(x => x.UpdatedAt)
                .IsRequired();

            this.Property(x => x.CreatedBy);
            this.Property(x => x.UpdatedBy);
        }
    }
}