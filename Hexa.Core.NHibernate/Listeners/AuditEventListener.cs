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
using System.Linq;
using System.Collections.Generic;

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

            var name = (ApplicationContext.User != null && !string.IsNullOrEmpty(ApplicationContext.User.Identity.Name)) ? ApplicationContext.User.Identity.Name : "Unknown";

            Set(e.Persister, e.State, "CreatedBy", name);
            Set(e.Persister, e.State, "UpdatedBy", name);
            Set(e.Persister, e.State, "UpdatedAt", DateTime.UtcNow);

            auditable.CreatedBy = name;
            auditable.UpdatedBy = name;

            return false;
        }

        public bool OnPreUpdate(PreUpdateEvent e)
        {
            var auditable = e.Entity as IAuditableEntity;
            if (auditable == null)
                return false;

            var logger = ServiceLocator.TryGetInstance<IAuditableEntityLogger>();
            if (logger != null)
            {
                var changedPropertiesIdx = e.Persister.FindDirty(e.State, e.OldState, e.Entity, e.Session.GetSessionImplementation());
                foreach (var idx in changedPropertiesIdx)
                {
                    var tableName = e.Persister.EntityName;
                    var propertyName = e.Persister.PropertyNames[idx];
                    var oldValue = e.OldState[idx];
                    var newValue = e.State[idx];

                    logger.Log(tableName, propertyName, oldValue, newValue);
                }
            }

            var name = (ApplicationContext.User != null && !string.IsNullOrEmpty(ApplicationContext.User.Identity.Name)) ? ApplicationContext.User.Identity.Name : "Unknown";
            var date = DateTime.UtcNow; 

            Set(e.Persister, e.State, "UpdatedBy", name);
            Set(e.Persister, e.State, "UpdatedAt", date);
            auditable.UpdatedBy = name;
            auditable.UpdatedAt = date;

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
                 .Concat(GetAdditionalDirtyProperties(e)).ToArray();
        }

        static IEnumerable<int> GetAdditionalDirtyProperties(FlushEntityEvent @event)
        {
            yield return Array.IndexOf(@event.EntityEntry.Persister.PropertyNames,
                                       "UpdatedAt");
            yield return Array.IndexOf(@event.EntityEntry.Persister.PropertyNames,
                                       "UpdatedBy");
            yield return Array.IndexOf(@event.EntityEntry.Persister.PropertyNames,
                                       "CreatedBy");
            //You can add any additional properties here.
            //Some of my entities do not track the user, for example.
        }
    }
}
