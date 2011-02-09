#region License

//===================================================================================
//Copyright 2010 HexaSystems Corporation
//===================================================================================
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//===================================================================================
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//===================================================================================

#endregion

using Hexa.Core.Validation;
using NHibernate.Cfg;
using NHibernate.Event;

namespace Hexa.Core.Domain
{
    public sealed class ValidateEventListener : IPreInsertEventListener, IPreUpdateEventListener, IInitializable
    {
        // Fields
        private bool isInitialized;
        private static readonly object padlock = new object();
        private static IValidator validator;

        // Properties
        private static IValidator Validator
        {
            get
            {
                lock (padlock)
                {
                    if (validator == null)
                    {
                        validator = ServiceLocator.GetInstance<IValidator>();
                    }
                }
                return validator;
            }
            set
            {
                lock (padlock)
                {
                    validator = value;
                }
            }
        }

        // Methods
        private static void Validate(object entity)
        {
            if (entity != null)
            {
                Validator.AssertValidation(entity);
            }
        }

        public void Initialize(Configuration cfg)
        {
            if (!this.isInitialized && (cfg != null))
                this.isInitialized = true;
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
    }

}
