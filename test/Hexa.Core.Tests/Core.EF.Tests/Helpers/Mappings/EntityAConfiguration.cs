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

    public class EntityAConfiguration : TenantScopedEntityConfiguration<EntityA>
    {
        public override void Map(EntityTypeBuilder<EntityA> builder)
        {
            base.Map(builder);

            builder.Property(h => h.Name);

            builder.HasMany<EntityB>(h => h.EntitiesOfB);
            
            //     .WithMany(h => h.EntitiesOfA).Map((c) =>
            //     {
            //         c.ToTable("EntityA_EntityB");
            //         c.MapLeftKey("EntityAUniqueId");
            //         c.MapRightKey("EntityBUniqueId");
            //     });
        }
    }
}