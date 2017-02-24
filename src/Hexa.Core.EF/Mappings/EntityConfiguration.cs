//----------------------------------------------------------------------------------------------
// <copyright file="EntityConfiguration.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class EntityConfiguration<TEntity, TKey> : EntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity<TEntity, TKey>
        where TKey : struct, IEquatable<TKey>
    {
       public override void Map(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Version)
                .IsRowVersion();

            builder.ToTable(Inflector.Underscore(typeof(TEntity).Name).ToUpper());
        }
    }
}