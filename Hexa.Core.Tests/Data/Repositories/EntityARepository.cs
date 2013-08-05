//----------------------------------------------------------------------------------------------
// <copyright file="EntityARepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using Core.Domain;
    using Domain;
    using Logging;

    public class EntityARepository : BaseRepository<EntityA>, IEntityARepository
    {
        public EntityARepository(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
        }
    }
}