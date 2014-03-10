//----------------------------------------------------------------------------------------------
// <copyright file="RavenRepository.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System.Globalization;
    using System.Linq;
    using Hexa.Core.Logging;
    using Raven.Client;

    public class RavenRepository<TEntity> : BaseRepository<TEntity>
        where TEntity : class
    {
        private readonly ILogger logger;
        private IDocumentSession session;

        public RavenRepository(IDocumentSession session)
            : base()
        {
            this.logger = LoggerManager.GetLogger(GetType());
            this.logger.Debug(string.Format(CultureInfo.InvariantCulture, "Created repository for type: {0}", typeof(TEntity).Name));
            this.session = session;
        }

        protected override void InternalAdd(TEntity entity)
        {
            this.session.Store(entity);
        }

        protected override void InternalAttach(TEntity entity)
        {
        }

        protected override void InternalModify(TEntity entity)
        {
        }

        protected override void InternalRemove(TEntity entity)
        {
            this.session.Delete(entity);
        }

        protected override IQueryable<TEntity> Query()
        {
            return this.session.Query<TEntity>().Customize(x => x.WaitForNonStaleResultsAsOfNow());
        }
    }
}