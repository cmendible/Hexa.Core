//----------------------------------------------------------------------------------------------
// <copyright file="EntityARepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using System;
    using Core.Domain;
    using Domain;
    using Hexa.Core.Orm.Tests.EF;
    using Microsoft.Extensions.Logging;

    public class EntityAEFRepository : EFRepository<EntityA, Guid>, IEntityARepository
    {
        public EntityAEFRepository(DomainContext dbContext, ILogger<BaseRepository<EntityA, Guid>> logger)
            : base(dbContext, logger)
        {
        }
    }
}