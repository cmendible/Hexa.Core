//----------------------------------------------------------------------------------------------
// <copyright file="AuditableEntityConfiguration.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Data.Entity.ModelConfiguration;

    public class AuditableEntityConfiguration<TEntity> : AuditableEntityConfiguration<TEntity, string>
        where TEntity : AuditableEntity<TEntity>
    {
    }

    public class AuditableEntityConfiguration<TEntity, TUserKey> : EntityConfiguration<TEntity, Guid>
        where TEntity : AuditableEntity<TEntity>
    {
        public AuditableEntityConfiguration()
        {
            this.Property(x => x.CreatedAt)
            .IsRequired();

            this.Property(x => x.UpdatedAt)
            .IsRequired();

            Type keyType = typeof(TUserKey);
            if (keyType.Equals(typeof(string)))
            {
                this.Property(x => x.CreatedBy);
                this.Property(x => x.UpdatedBy);
            }
            else if (keyType.Equals(typeof(Guid)))
            {
                this.Property(x => x.CreatedBy)
                .HasColumnType("UniqueIdentifier");
                this.Property(x => x.UpdatedBy)
                .HasColumnType("UniqueIdentifier");
            }
            else if (keyType.Equals(typeof(int)))
            {
                this.Property(x => x.CreatedBy)
                .HasColumnType("int");
                this.Property(x => x.UpdatedBy)
                .HasColumnType("int");
            }
        }
    }
}