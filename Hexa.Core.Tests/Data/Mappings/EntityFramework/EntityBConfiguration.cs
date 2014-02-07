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

    public class EntityBConfiguration : AuditableEntityConfiguration<EntityB>
    {
        public EntityBConfiguration()
        {
            this.Property(h => h.Name);
        }
    }
}