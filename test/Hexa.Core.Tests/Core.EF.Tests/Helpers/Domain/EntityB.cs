//----------------------------------------------------------------------------------------------
// <copyright file="EntityB.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Tests.Domain
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Core.Domain;

    public class EntityB : AuditableEntity<EntityB>
    {
        private IList<EntityA> entitiesOfA;

        public EntityB()
        {
            this.entitiesOfA = new List<EntityA>();
        }

        public virtual ICollection<EntityA> EntitiesOfA
        {
            get
            {
                return entitiesOfA;
            }
        }

        [Required]
        public virtual string Name
        {
            get;
            set;
        }
    }
}