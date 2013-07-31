namespace Hexa.Core.Domain
{
    using System;

    public class EventSourcedRepository<T> : IEventSourcedRepository<T>
        where T : EventSourcedEntity, new()
    {
        private readonly IEventStore storage;

        public EventSourcedRepository(IEventStore storage)
        {
            this.storage = storage;
        }

        public T GetById(Guid Id)
        {
            var obj = new T();
            var e = this.storage.GetEventsForAggregate(Id);
            obj.LoadsFromHistory(e);
            return obj;
        }

        public void Save(EventSourcedEntity aggregate, int expectedVersion)
        {
            this.storage.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedVersion);
        }
    }
}