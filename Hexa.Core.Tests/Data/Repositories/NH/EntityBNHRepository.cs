//----------------------------------------------------------------------------------------------
// <copyright file="EntityBRepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Data
{
    using Core.Domain;
    using Domain;
    using NHibernate;

    public class EntityBNHRepository : NHRepository<EntityB>, IEntityBRepository
    {
        public EntityBNHRepository(ISession session)
            : base(session)
        {
        }
    }
}