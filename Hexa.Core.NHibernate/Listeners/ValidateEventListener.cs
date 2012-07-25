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

namespace Hexa.Core.Domain
{
    using NHibernate.Cfg;
    using NHibernate.Event;

    using Validation;

    public sealed class ValidateEventListener : IPreInsertEventListener, IPreUpdateEventListener, IInitializable
    {
        #region Fields

        private bool isInitialized;

        #endregion Fields

        #region Methods

        public void Initialize(Configuration cfg)
        {
            if (!isInitialized && (cfg != null))
            {
                this.isInitialized = true;
            }
        }

        public bool OnPreInsert(PreInsertEvent @event)
        {
            Validate(@event.Entity);
            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            Validate(@event.Entity);
            return false;
        }

        private static void Validate(object entity)
        {
            IValidatable validatable = entity as IValidatable;
            if (validatable != null)
            {
                validatable.AssertValidation();
            }
        }

        #endregion Methods
    }
}