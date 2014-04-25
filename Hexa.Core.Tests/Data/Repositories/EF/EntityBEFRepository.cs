//----------------------------------------------------------------------------------------------
// <copyright file="EntityBRepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using System;
    using System.Data.Entity;
    using Core.Domain;
    using Domain;

    public class EntityBEFRepository : EFRepository<EntityB, Guid>, IEntityBRepository
    {
        public EntityBEFRepository(DbContext dbContext)
            : base(dbContext)
        {
        }
    }
}