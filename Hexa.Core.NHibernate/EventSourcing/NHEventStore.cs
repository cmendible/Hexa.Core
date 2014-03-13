// -----------------------------------------------------------------------
// <copyright file="NHEventStore.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using NHibernate;

    public class NHibernateEventStore : BaseEventStore
    {
        private readonly IStatelessSession _session;

        public NHibernateEventStore(
            IEventPublisher publisher,
            IStatelessSession session)
            : base(publisher)
        {
            _session = session;
        }

        protected override IEnumerable<EventDescriptor> LoadEventDescriptorsForAggregate(Guid aggregateId)
        {
            var query = _session.GetNamedQuery("LoadEventDescriptors")
                        .SetGuid("aggregateId", aggregateId);
            return Transact(() => query.List<EventDescriptor>());
        }

        protected override void PersistEventDescriptors(
            IEnumerable<EventDescriptor> newEventDescriptors,
            Guid aggregateId, int expectedVersion)
        {
            // Don't bother to check expectedVersion. Since we can't delete
            // events, we won't skip a version. If we do have a true concurrency
            // violation, we'll get a PK violation exception.
            // SqlExceptionConverter will change it to a ConcurrencyViolation.
            Transact(() =>
            {
                foreach (var ed in newEventDescriptors)
                    _session.Insert(ed);
            });
        }

        protected virtual TResult Transact<TResult>(Func<TResult> func)
        {
            if (!_session.Transaction.IsActive)
            {
                // Wrap in transaction
                TResult result;
                using (var tx = _session.BeginTransaction())
                {
                    result = func.Invoke();
                    tx.Commit();
                }
                return result;
            }

            // Don't wrap;
            return func.Invoke();
        }

        protected virtual void Transact(Action action)
        {
            Transact<bool>(() =>
            {
                action.Invoke();
                return false;
            });
        }
    }
}