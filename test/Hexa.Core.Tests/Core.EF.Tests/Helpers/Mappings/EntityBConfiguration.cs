//----------------------------------------------------------------------------------------------
// <copyright file="EntityAConfiguration.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using Core.Domain;
    using Domain;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class EntityBConfiguration : AuditableEntityConfiguration<EntityB>
    {
        public override void Map(EntityTypeBuilder<EntityB> builder)
        {
            base.Map(builder);
            
            builder.Property(h => h.Name);
        }
    }
}