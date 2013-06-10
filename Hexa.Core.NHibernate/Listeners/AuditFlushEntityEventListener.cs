// -----------------------------------------------------------------------
// <copyright file="AuditFlushEntityEventListener.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using NHibernate;
    using NHibernate.Event;
    using NHibernate.Event.Default;
    using NHibernate.Persister.Entity;

    /// <summary>
    ///
    /// </summary>
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