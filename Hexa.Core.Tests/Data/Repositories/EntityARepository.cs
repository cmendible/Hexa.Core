//----------------------------------------------------------------------------------------------
// <copyright file="EntityARepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using System.ComponentModel.Composition;

    using Core.Domain;

    using Domain;

    using Logging;

    [Export(typeof(IEntityARepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EntityARepository : BaseRepository<EntityA>, IEntityARepository
    {
        [ImportingConstructor]
        public EntityARepository(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
        }
    }
}