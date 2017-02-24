//----------------------------------------------------------------------------------------------
// <copyright file="EntityBRepository.cs" company="HexaSystems Inc">
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

    public class EntityBEFRepository : EFRepository<EntityB, Guid>, IEntityBRepository
    {
        public EntityBEFRepository(DomainContext dbContext, ILogger<BaseRepository<EntityB, Guid>> logger)
            : base(dbContext, logger)
        {
        }
    }
}