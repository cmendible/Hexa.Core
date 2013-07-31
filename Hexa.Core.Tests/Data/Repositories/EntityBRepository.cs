//----------------------------------------------------------------------------------------------
// <copyright file="EntityBRepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using System.ComponentModel.Composition;

    using Core.Domain;

    using Domain;

    using Logging;

    [Export(typeof(IEntityBRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EntityBRepository : BaseRepository<EntityB>, IEntityBRepository
    {
        [ImportingConstructor]
        public EntityBRepository(IUnitOfWork unitOfWork)
        : base(unitOfWork)
        {
        }
    }
}