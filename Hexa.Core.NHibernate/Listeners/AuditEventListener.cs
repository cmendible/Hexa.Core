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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NHibernate.Event;
    using NHibernate.Event.Default;
    using NHibernate.Persister.Entity;

    using Security;

    public class AuditEventListener : IPreUpdateEventListener, IPreInsertEventListener
    {
        #region Methods

        public bool OnPreInsert(PreInsertEvent @event)
        {
            var auditable = @event.Entity as IAuditableEntity;
            if (auditable == null)
            {
                return false;
            }

            string userUniqueId = string.Empty;

            var user = ApplicationContext.User;
            if (user != null)
            {
                var identity = ApplicationContext.User.Identity as ICoreIdentity;
                if (identity != null)
                {
                    userUniqueId = identity.Id;
                }
            }

            DateTime createdAt = DateTime.Now;

            this._Set(@event.Persister, @event.State, "CreatedBy", userUniqueId);
            this._Set(@event.Persister, @event.State, "UpdatedBy", userUniqueId);
            this._Set(@event.Persister, @event.State, "CreatedAt", createdAt);
            this._Set(@event.Persister, @event.State, "UpdatedAt", createdAt);

            auditable.CreatedBy = userUniqueId;
            auditable.UpdatedBy = userUniqueId;
            auditable.CreatedAt = createdAt;
            auditable.UpdatedAt = createdAt;

            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent @event)
        {
            var auditable = @event.Entity as IAuditableEntity;
            if (auditable == null)
            {
                return false;
            }

            string userUniqueId = string.Empty;

            var user = ApplicationContext.User;
            if (user != null)
            {
                var identity = ApplicationContext.User.Identity as ICoreIdentity;
                if (identity != null)
                {
                    userUniqueId = identity.Id;
                }
            }

            DateTime updatedAt = DateTime.Now;

            var auditTrailFactory = ServiceLocator.TryGetInstance<IAuditTrailFactory>();
            if (auditTrailFactory != null && auditTrailFactory.IsEntityRegistered(@event.Persister.EntityName))
            {
                string tableName = @event.Persister.EntityName;
                int[] changedPropertiesIdx = @event.Persister.FindDirty(@event.State, @event.OldState, @event.Entity,
                                             @event.Session.GetSessionImplementation());
                foreach (int idx in changedPropertiesIdx)
                {
                    string propertyName = @event.Persister.PropertyNames[idx];
                    object oldValue = @event.OldState[idx];
                    object newValue = @event.State[idx];

                    IEntityAuditTrail auditTrail = auditTrailFactory.CreateAuditTrail(tableName, @event.Id.ToString(),
                                                   propertyName, oldValue, newValue,
                                                   userUniqueId,
                                                   updatedAt);

                    @event.Session.Save(auditTrail);
                }
            }

            this._Set(@event.Persister, @event.State, "UpdatedBy", userUniqueId);
            this._Set(@event.Persister, @event.State, "UpdatedAt", updatedAt);
            auditable.UpdatedBy = userUniqueId;
            auditable.UpdatedAt = updatedAt;

            return false;
        }

        private void _Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            int index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
            {
                return;
            }
            state[index] = value;
        }

        #endregion Methods
    }

    //http://stackoverflow.com/questions/5087888/ipreupdateeventlistener-and-dynamic-update-true
    public class AuditFlushEntityEventListener : DefaultFlushEntityEventListener
    {
        #region Methods

        protected override void DirtyCheck(FlushEntityEvent @event)
        {
            base.DirtyCheck(@event);
            if (@event.DirtyProperties != null &&
                @event.DirtyProperties.Any() &&
                //IAuditableEntity is my inteface for audited entities
                @event.Entity is IAuditableEntity)
                @event.DirtyProperties = @event.DirtyProperties
                                         .Concat(_GetAdditionalDirtyProperties(@event)).ToArray();
        }

        private static IEnumerable<int> _GetAdditionalDirtyProperties(FlushEntityEvent @event)
        {
            yield return Array.IndexOf(@event.EntityEntry.Persister.PropertyNames,
                                       "UpdatedAt");
            yield return Array.IndexOf(@event.EntityEntry.Persister.PropertyNames,
                                       "UpdatedBy");
            yield return Array.IndexOf(@event.EntityEntry.Persister.PropertyNames,
                                       "CreatedBy");
            yield return Array.IndexOf(@event.EntityEntry.Persister.PropertyNames,
                                       "CreatedAt");
        }

        #endregion Methods
    }
}