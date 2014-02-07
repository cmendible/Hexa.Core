//----------------------------------------------------------------------------------------------
// <copyright file="EntityA.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Core.Domain;

    [Serializable]
    public class EntityA : AuditableEntity<EntityA>
    {
        private IList<EntityB> entitiesOfB;

        public EntityA()
        {
            this.entitiesOfB = new List<EntityB>();
        }

        public virtual ICollection<EntityB> EntitiesOfB
        {
            get
            {
                return entitiesOfB;
            }
        }

        [Required]
        public virtual string Name
        {
            get;
            set;
        }

        public virtual void AddB(EntityB b)
        {
            this.entitiesOfB.Add(b);
        }
    }
}