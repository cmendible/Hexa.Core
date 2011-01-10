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

using System;

using NHibernate.Cfg;
using NHibernate.Event;
using NHibernate.Persister.Entity;

using Hexa.Core.Validation;

namespace Hexa.Core.Domain
{
    public class AuditEventListener : IPreUpdateEventListener, IPreInsertEventListener
    {
        public bool OnPreInsert(PreInsertEvent e)
        {
            var auditable = e.Entity as IAuditableEntity;
            if (auditable == null)
                return false;

            var name = ApplicationContext.User != null ?ApplicationContext.User.Identity.Name: "Unknown";

            Set(e.Persister, e.State, "CreatedBy", name);
            Set(e.Persister, e.State, "UpdatedBy", name);

            auditable.CreatedBy = name;
            auditable.UpdatedBy = name;

            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent e)
        {
            var auditable = e.Entity as IAuditableEntity;
            if (auditable == null)
                return false;

            var changedPropertiesIdx = e.Persister.FindDirty(e.State, e.OldState, e.Entity, e.Session.GetSessionImplementation());
            foreach (var idx in changedPropertiesIdx)
            {
                var tableName = e.Persister.EntityName;
                var properyName = e.Persister.PropertyNames[idx];
                var oldValue = e.OldState[idx];
                var newValue = e.State[idx];

                //Register this using logging or a table.
            }

            var name = ApplicationContext.User != null ? ApplicationContext.User.Identity.Name : "Unknown";

            Set(e.Persister, e.State, "UpdatedBy", name);
            auditable.UpdatedBy = name;

            return false;
        }

        private void Set(IEntityPersister persister, object[] state, string propertyName, object value)
        {
            var index = Array.IndexOf(persister.PropertyNames, propertyName);
            if (index == -1)
                return;
            state[index] = value;
        }
    }
}
