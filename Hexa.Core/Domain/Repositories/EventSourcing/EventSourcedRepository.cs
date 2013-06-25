namespace Hexa.Core.Domain
{
    using System;

    public class EventSourcedRepository<T> : IEventSourcedRepository<T> where T : EventSourcedEntity, new() //shortcut you can do as you see fit with new()
    {
        private readonly IEventStore storage;

        public EventSourcedRepository(IEventStore storage)
        {
            this.storage = storage;
        }

        public void Save(EventSourcedEntity aggregate, int expectedVersion)
        {
            this.storage.SaveEvents(aggregate.Id, aggregate.GetUncommittedChanges(), expectedVersion);
        }

        public T GetById(Guid Id)
        {
            var obj = new T();//lots of ways to do this
            var e = this.storage.GetEventsForAggregate(Id);
            obj.LoadsFromHistory(e);
            return obj;
        }
    }

}
