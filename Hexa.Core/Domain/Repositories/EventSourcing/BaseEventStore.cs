namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public abstract class BaseEventStore : IEventStore
    {
        private readonly IEventPublisher publisher;

        protected BaseEventStore(IEventPublisher publisher)
        {
            this.publisher = publisher;
        }

        public void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion)
        {
            var eventDescriptors = new List<EventDescriptor>();
            var i = expectedVersion;
            foreach (var @event in events)
            {
                i++;
                @event.Version = i;
                eventDescriptors.Add(new EventDescriptor(aggregateId, @event, i));
            }

            this.PersistEventDescriptors(eventDescriptors, aggregateId, expectedVersion);

            MethodInfo publishMethod = this.publisher.GetType().GetMethod("Publish");

            foreach (Event @event in events)
            {
                MethodInfo method = publishMethod.MakeGenericMethod(new Type[] { @event.GetType() });
                method.Invoke(this.publisher, new object[] { @event });
                // _publisher.Publish(@event);
            }
        }

        public List<Event> GetEventsForAggregate(Guid aggregateId)
        {
            IEnumerable<EventDescriptor> eventDescriptors = this.LoadEventDescriptorsForAggregate(aggregateId);
            if (null == eventDescriptors || !eventDescriptors.Any())
            {
                throw new AggregateNotFoundException();
            }
            return eventDescriptors.Select(desc => desc.EventData).ToList();
        }

        protected abstract IEnumerable<EventDescriptor> LoadEventDescriptorsForAggregate(Guid aggregateId);

        protected abstract void PersistEventDescriptors(IEnumerable<EventDescriptor> newEventDescriptors, Guid aggregateId, int expectedVersion);

    }
}
