#region Header

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion Header

namespace Hexa.Core.Tests.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Core.Domain;

    [Serializable]
    public class EntityA : AuditableEntity<EntityA>
    {
        #region Fields

        private IList<EntityB> entitiesOfB;

        #endregion Fields

        #region Constructors

        public EntityA()
        {
            this.entitiesOfB = new List<EntityB>();
        }

        #endregion Constructors

        #region Properties

        public virtual IEnumerable<EntityB> EntitiesOfB
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

        #endregion Properties

        #region Methods

        public virtual void AddB(EntityB b)
        {
            this.entitiesOfB.Add(b);
        }

        #endregion Methods
    }
}