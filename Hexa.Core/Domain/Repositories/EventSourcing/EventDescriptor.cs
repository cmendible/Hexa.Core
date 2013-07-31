namespace Hexa.Core.Domain
{
    using System;

    public struct EventDescriptor
    {
        public readonly Event EventData;
        public readonly Guid Id;
        public readonly int Version;

        public EventDescriptor(Guid id, Event eventData, int version)
        {
            this.EventData = eventData;
            this.Version = version;
            this.Id = id;
        }
    }
}