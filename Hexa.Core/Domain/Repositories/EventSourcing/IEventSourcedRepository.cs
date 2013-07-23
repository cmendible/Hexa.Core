namespace Hexa.Core.Domain
{
    using System;

    public interface IEventSourcedRepository<T>
        where T : EventSourcedEntity, new()
    {
        T GetById(Guid id);

        void Save(EventSourcedEntity aggregate, int expectedVersion);
    }
}