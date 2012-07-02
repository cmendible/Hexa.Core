#region License

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

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Hexa.Core.Security;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;

namespace Hexa.Core.Domain
{
    public class AuditEventListener : IPreUpdateEventListener, IPreInsertEventListener
    {
        public bool OnPreInsert(PreInsertEvent e)
        {
            var auditable = e.Entity as IAuditableEntity;
            if (auditable == null)
                return false;

            var identity = ApplicationContext.User.Identity as ICoreIdentity;
            Guard.IsNotNull(identity, "No ICoreIdentity found in context.");
            var userUniqueId = identity.Id;

            var createdAt = DateTime.Now;

            _Set(e.Persister, e.State, "CreatedBy", userUniqueId);
            _Set(e.Persister, e.State, "UpdatedBy", userUniqueId);
            _Set(e.Persister, e.State, "CreatedAt", createdAt);
            _Set(e.Persister, e.State, "UpdatedAt", createdAt);

            auditable.CreatedBy = userUniqueId;
            auditable.UpdatedBy = userUniqueId;
            auditable.CreatedAt = createdAt;
            auditable.UpdatedAt = createdAt;

            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent e)
        {
            var auditable = e.Entity as IAuditableEntity;
            if (auditable == null)
                return false;

            var identity = ApplicationContext.User.Identity as ICoreIdentity;
            Guard.IsNotNull(identity, "No ICoreIdentity found in context.");
            var userUniqueId = identity.Id;

            var updatedAt = DateTime.Now;

            var auditTrailFactory = ServiceLocator.TryGetInstance<IAuditTrailFactory>();
            if (auditTrailFactory != null && auditTrailFactory.IsEntityRegistered(e.Persister.EntityName))
                {
                    var tableName = e.Persister.EntityName;
                    var changedPropertiesIdx = e.Persister.FindDirty(e.State, e.OldState, e.Entity, e.Session.GetSessionImplementation());
                    foreach (var idx in changedPropertiesIdx)
                        {
                            var propertyName = e.Persister.PropertyNames[idx];
                            var oldValue = e.OldState[idx];
                            var newValue = e.State[idx];

                            var auditTrail = auditTrailFactory.CreateAuditTrail(tableName, e.Id.ToString(),
                                             propertyName, oldValue, newValue, userUniqueId, updatedAt);

                            e.Session.Save(auditTrail);
                        }
                }

            _Set(e.Persister, e.State, "UpdatedBy", userUniqueId);
            _Set(e.Persister, e.State, "UpdatedAt", updatedAt);
            auditable.UpdatedBy = userUniqueId;
            auditable.UpdatedAt = updatedAt;

            return false;
        }

        private void _Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
    }

    //http://stackoverflow.com/questions/5087888/ipreupdateeventlistener-and-dynamic-update-true
    public class AuditFlushEntityEventListener : DefaultFlushEntityEventListener
    {
        protected override void DirtyCheck(FlushEntityEvent e)
        {
            base.DirtyCheck(e);
            if (e.DirtyProperties != null &&
                    e.DirtyProperties.Any() &&
                    //IAuditableEntity is my inteface for audited entities
                    e.Entity is IAuditableEntity)
                e.DirtyProperties = e.DirtyProperties
                                    .Concat(_GetAdditionalDirtyProperties(e)).ToArray();
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
    }
}
