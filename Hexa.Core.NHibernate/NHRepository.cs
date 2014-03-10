//----------------------------------------------------------------------------------------------
// <copyright file="NHRepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System.Globalization;
    using System.Linq;
    using Logging;
    using NHibernate;
    using NHibernate.Linq;

    public class NHRepository<TEntity> : BaseRepository<TEntity>
        where TEntity : class
    {
        private readonly ILogger logger;
        private readonly ISession session;

        public NHRepository(ISession session) : base()
        {
            this.logger = LoggerManager.GetLogger(GetType());
            this.logger.Debug(string.Format(CultureInfo.InvariantCulture, "Created repository for type: {0}", typeof(TEntity).Name));
            this.session = session;
        }

        protected override void InternalAdd(TEntity entity)
        {
            this.session.Save(entity);
        }

        protected override void InternalAttach(TEntity entity)
        {
            this.session.Lock(entity, LockMode.None);
        }

        protected override void InternalModify(TEntity entity)
        {
            if (!this.session.Contains(entity))
            {
                this.session.Update(entity);
            }
        }

        protected override void InternalRemove(TEntity entity)
        {
            this.session.Lock(entity, LockMode.None);
            this.session.Delete(entity);
        }

        protected override IQueryable<TEntity> Query()
        {
            return this.session.Query<TEntity>();
        }
    }
}