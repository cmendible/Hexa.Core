namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IEventStore
    {
        List<Event> GetEventsForAggregate(Guid aggregateId);

        void SaveEvents(Guid aggregateId, IEnumerable<Event> events, int expectedVersion);
    }
}