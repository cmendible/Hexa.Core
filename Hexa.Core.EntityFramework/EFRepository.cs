//----------------------------------------------------------------------------------------------
// <copyright file="EFRepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using Logging;

    public class EFRepository<TEntity> : BaseRepository<TEntity>
        where TEntity : class
    {
        private readonly ILogger logger;
        private DbContext dbContext;

        public EFRepository(DbContext dbContext) : base()
        {
            this.logger = LoggerManager.GetLogger(GetType());
            this.logger.Debug(string.Format(CultureInfo.InvariantCulture, "Created repository for type: {0}", typeof(TEntity).Name));
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
            entry.State = System.Data.Entity.EntityState.Modified;
        }

        protected override void InternalRemove(TEntity entity)
        {
            this.dbContext.Set<TEntity>().Remove(entity);
        }

        protected override IQueryable<TEntity> Query()
        {
            return this.dbContext.Set<TEntity>();
        }
    }
}