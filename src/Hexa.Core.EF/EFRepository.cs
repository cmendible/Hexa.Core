//----------------------------------------------------------------------------------------------
// <copyright file="EFRepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using Microsoft.Extensions.Logging;

    public class EFRepository<TEntity, TKey> : BaseRepository<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        private DbContext dbContext;

        public EFRepository(DbContext dbContext, ILogger logger)
            : base(logger)
        {
            this.dbContext = dbContext;
        }

        protected override void InternalAdd(TEntity entity)
        {
            this.dbContext.Set<TEntity>().Add(entity);
        }

        protected override void InternalAttach(TEntity entity)
        {
            this.dbContext.Set<TEntity>().Attach(entity);
        }

        protected override void InternalModify(TEntity entity)
        {
            var entry = this.dbContext.Entry(entity);
            entry.State = EntityState.Modified;
        }

        protected override void InternalRemove(TEntity entity)
        {
            this.dbContext.Set<TEntity>().Remove(entity);
        }

        protected override IQueryable<TEntity> Query()
        {
            return this.dbContext.Set<TEntity>();
        }

        protected override TEntity Load(TKey id)
        {
            return this.dbContext.Set<TEntity>().Find(id);
        }
    }
}