//----------------------------------------------------------------------------------------------
// <copyright file="EntityBRepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using Core.Domain;
    using Domain;
    using Logging;

    public class EntityBRepository : BaseRepository<EntityB>, IEntityBRepository
    {
        public EntityBRepository(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
        }
    }
}