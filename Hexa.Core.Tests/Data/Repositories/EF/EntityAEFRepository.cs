//----------------------------------------------------------------------------------------------
// <copyright file="EntityARepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using System;
    using System.Data.Entity;
    using Core.Domain;
    using Domain;

    public class EntityAEFRepository : EFRepository<EntityA, Guid>, IEntityARepository
    {
        public EntityAEFRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}