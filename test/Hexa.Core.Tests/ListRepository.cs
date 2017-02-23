namespace Hexa.Core.Domain.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Hexa.Core.Domain;
    using Hexa.Core.Domain.Tests;

    /// <summary>
    /// 
    /// </summary>
    internal class ListRepository : BaseRepository<Entity, int>
    {
        List<Entity> list = new List<Entity>();

        public ListRepository()
        {
            this.list.Add(new Entity() { Id = 1, SampleProperty = "test" });
        }

        protected override void InternalAdd(Entity entity)
        {
            this.list.Add(entity);
        }

        protected override void InternalAttach(Entity entity)
        {
            if (!this.list.Contains(entity))
            {
                this.list.Add(entity);
            }
        }

        protected override void InternalModify(Entity entity)
        {
            var idx = this.list.FindIndex(l => l.Id == entity.Id);
            if (idx > 0)
            {
                this.list[idx] = entity;
            }
        }

        protected override void InternalRemove(Entity entity)
        {
            this.list.Remove(entity);
        }

        protected override IQueryable<Entity> Query()
        {
            return this.list.AsQueryable();
        }

        protected override Entity Load(int id)
        {
            return this.list.Where(l => l.Id == id).First();
        }
    }
}