//----------------------------------------------------------------------------------------------
// <copyright file="EntityConfiguration.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.ModelConfiguration;

    public class EntityConfiguration<TEntity, TKey> : EntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity<TEntity, TKey>
        where TKey : struct, IEquatable<TKey>
    {
        public EntityConfiguration()
        {
            this.HasKey(x => x.UniqueId);

            this.Property(x => x.UniqueId)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            this.Property(x => x.Version)
                .IsConcurrencyToken();

            this.ToTable(Inflector.Underscore(typeof(TEntity).Name).ToUpper(), string.Empty);
        }
    }
}