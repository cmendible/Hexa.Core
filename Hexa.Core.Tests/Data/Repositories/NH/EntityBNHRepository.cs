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
    using NHibernate;

    public class EntityBNHRepository : NHRepository<EntityB, Guid>, IEntityBRepository
    {
        public EntityBNHRepository(ISession session)
            : base(session)
        {
        }
    }
}