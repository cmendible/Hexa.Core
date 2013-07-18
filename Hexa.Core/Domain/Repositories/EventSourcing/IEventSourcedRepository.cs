namespace Hexa.Core.Domain
{
    using System;

    public interface IEventSourcedRepository<T> where T : EventSourcedEntity, new()
    {
        void Save(EventSourcedEntity aggregate, int expectedVersion);
        T GetById(Guid id);
    }

}
