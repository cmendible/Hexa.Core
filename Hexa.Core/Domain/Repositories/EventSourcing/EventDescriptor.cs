namespace Hexa.Core.Domain
{
    using System;

    public class EventDescriptor
    {
        private const int HASH_MULTIPLIER = 31;
        private int? cachedHashcode;

        protected EventDescriptor()
        {
        }

        public EventDescriptor(Guid id, Event eventData, int version)
        {
            this.EventData = eventData;
            this.Version = version;
            this.Id = id;
        }

        public virtual Event EventData
        {
            get;
            protected set;
        }

        public virtual Guid Id
        {
            get;
            protected set;
        }

        public virtual int Version
        {
            get;
            protected set;
        }

        public override int GetHashCode()
        {
            // Once we have a hash code we'll never change it
            if (this.cachedHashcode.HasValue)
            {
                return this.cachedHashcode.Value;
            }

            unchecked
            {
                // It's possible for two objects to return the same hash code based on
                // identically valued properties, even if they're of two different types,
                // so we include the object's type in the hash calculation
                int hashCode = this.GetType().GetHashCode();
                this.cachedHashcode = (this.Version.GetHashCode() * hashCode * HASH_MULTIPLIER) ^ this.Id.GetHashCode();
            }

            return cachedHashcode.Value;
        }

        public override bool Equals(object obj)
        {
            var compareTo = obj as EventDescriptor;

            if (object.ReferenceEquals(this, compareTo))
            {
                return true;
            }

            if (compareTo == null || compareTo is EventDescriptor == false)
            {
                return false;
            }

            return this.Id.Equals(compareTo.Id) && this.Version.Equals(compareTo.Version);
        }
    }
}