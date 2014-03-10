//----------------------------------------------------------------------------------------------
// <copyright file="EntityARepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using Core.Domain;
    using Domain;
    using Raven.Client;

    public class EntityARavenRepository : RavenRepository<EntityA>, IEntityARepository
    {
        public EntityARavenRepository(IDocumentSession session)
        : base(session)
        {
        }
    }
}