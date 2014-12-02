//----------------------------------------------------------------------------------------------
// <copyright file="EntityAConfiguration.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using System.Data.Entity.ModelConfiguration.Configuration;

    using Core.Domain;

    using Domain;

    public class EntityAConfiguration : TenantScopedEntityConfiguration<EntityA>
    {
        public EntityAConfiguration()
        {
            this.Property(h => h.Name);

            this.HasMany<EntityB>(h => h.EntitiesOfB)
                .WithMany(h => h.EntitiesOfA).Map((c) =>
                {
                    c.ToTable("EntityA_EntityB");
                    c.MapLeftKey("EntityAUniqueId");
                    c.MapRightKey("EntityBUniqueId");
                });
        }
    }
}