namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class MemoryEventStore : BaseEventStore
    {
        private readonly Dictionary<Guid, List<EventDescriptor>> current = new Dictionary<Guid, List<EventDescriptor>>();

        public MemoryEventStore(IEventPublisher publisher)
        : base(publisher)
        {
        }

        protected override IEnumerable<EventDescriptor> LoadEventDescriptorsForAggregate(Guid aggregateId)
        {
            if (!this.current.ContainsKey(aggregateId))
            {
                return new EventDescriptor[] { };
            }
            return this.current[aggregateId];
        }

        protected override void PersistEventDescriptors(IEnumerable<EventDescriptor> newEventDescriptors, Guid aggregateId, int expectedVersion)
        {
            List<EventDescriptor> eventDescriptors;
            if (!this.current.TryGetValue(aggregateId, out eventDescriptors))
            {
                eventDescriptors = new List<EventDescriptor>();
                this.current.Add(aggregateId, eventDescriptors);
            }
            else if (eventDescriptors[eventDescriptors.Count - 1].Version != expectedVersion && expectedVersion != -1)
            {
                throw new ConcurrencyException();
            }

            eventDescriptors.AddRange(newEventDescriptors);
        }
    }
}