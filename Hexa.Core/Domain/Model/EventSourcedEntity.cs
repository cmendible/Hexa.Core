namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using Hexa.Core.DynamicExtensions;

    public abstract class EventSourcedEntity
    {
        private readonly List<Event> _changes = new List<Event>();

        public Guid Id
        {
            get;
            protected set;
        }
        public int Version
        {
            get;
            internal set;
        }

        public IEnumerable<Event> GetUncommittedChanges()
        {
            return this._changes;
        }

        public void MarkChangesAsCommitted()
        {
            this._changes.Clear();
        }

        public void LoadsFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history)
            {
                this.ApplyChange(e, false);
                this.Version = e.Version;
            }
        }

        protected void ApplyChange(Event @event)
        {
            this.ApplyChange(@event, true);
        }

        private void ApplyChange(Event @event, bool isNew)
        {
            this.AsDynamic().Apply(@event);
            if (isNew)
            {
                this._changes.Add(@event);
            }
        }
    }

}
