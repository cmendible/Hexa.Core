//----------------------------------------------------------------------------------------------
// <copyright file="EntityBMap.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using Core.Domain;

    using Domain;

    public class EntityBMap : AuditableEntityMap<EntityB>
    {
        public EntityBMap()
        {
            Map(h => h.Name);

            HasManyToMany(h => h.EntitiesOfA)
            .Access.CamelCaseField()
            .Table("EntityA_EntityB")
            .ParentKeyColumn("EntityBUniqueId")
            .ChildKeyColumn("EntityAUniqueId");
        }
    }
}