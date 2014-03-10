//----------------------------------------------------------------------------------------------
// <copyright file="EventSourcedEntity.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using Hexa.Core.DynamicExtensions;

    [Serializable]
    public class Event
    {
        public int Version
        {
            get;
            set;
        }
    }

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

        public void LoadsFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history)
            {
                this.ApplyChange(e, false);
                this.Version = e.Version;
            }
        }

        public void MarkChangesAsCommitted()
        {
            this._changes.Clear();
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