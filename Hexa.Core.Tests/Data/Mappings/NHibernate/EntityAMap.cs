//----------------------------------------------------------------------------------------------
// <copyright file="EntityAMap.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using Core.Domain;

    using Domain;

    public class EntityAMap : TenantScopedEntityMap<EntityA>
    {
        public EntityAMap()
        {
            Map(h => h.Name);

            HasManyToMany(h => h.EntitiesOfB)
            .Access.CamelCaseField()
            .Table("EntityA_EntityB")
            .ParentKeyColumn("EntityAUniqueId")
            .ChildKeyColumn("EntityBUniqueId");
        }
    }
}