// -----------------------------------------------------------------------
// <copyright file="EventSourcedEntityA.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Hexa.Core.Tests.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Hexa.Core.Tests.Domain.Events;

    using Core.Domain;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class EntityC : EventSourcedEntity
    {
        #region Fields

        private IList<Guid> entitiesOfB;

        #endregion

        public EntityC()
        {
        }

        public string Name
        {
            get;
            protected set;
        }

        public IEnumerable<Guid> EntitiesOfB
        {
            get
            {
                return this.entitiesOfB;
            }
            protected set 
            {
                this.entitiesOfB = value.ToList();
            }
        }

        public EntityC(Guid id, string name)
        {
            ApplyChange(new EntityCCreated(id, name));
        }

        public void AddB(Guid bId)
        {
            ApplyChange(new AddedBToEntityC(this.Id, bId));
        }

        public void Update(string name)
        {
            ApplyChange(new EntityCUpdated(this.Id, name));
        }

        private void Apply(EntityCCreated e)
        {
            this.Id = e.Id;
            this.Name = e.Name;
        }

        private void Apply(EntityCUpdated e)
        {
            this.Name = e.Name;
        }

        private void Apply(AddedBToEntityC e)
        {
            if (this.entitiesOfB == null)
            {
                this.entitiesOfB = new List<Guid>();
            }
            this.entitiesOfB.Add(e.bId);
        }
    }
}

namespace Hexa.Core.Tests.Domain.Events
{
    using Hexa.Core.Domain;
    using System;
    
    public class EntityCCreated : Event
    {
        public EntityCCreated()
        { 
        
        }

        public Guid Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public EntityCCreated(Guid id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public int Version
        {
            get;
            set;
        }
    }

    public class EntityCUpdated : EntityCCreated
    {
        public EntityCUpdated()
        { }

        public EntityCUpdated(Guid id, string name) : base(id, name)
        {
        }
    }

    public class AddedBToEntityC: Event
    {
        public AddedBToEntityC()
        { }

        public readonly Guid Id;
        public readonly Guid bId;
        public AddedBToEntityC(Guid id, Guid bId)
        {
            this.Id = id;
            this.bId = bId;
        }
    }
}
