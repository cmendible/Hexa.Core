//----------------------------------------------------------------------------------------------
// <copyright file="NHibernateUnitOfWork.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Runtime.Remoting.Messaging;
    using System.ServiceModel;
    using System.Transactions;
    using System.Web;
    using NHibernate;
    using NHibernate.Linq;

    public class NHibernateUnitOfWork : INHibernateUnitOfWork
    {
        ISessionFactory sessionFactory;

        public NHibernateUnitOfWork(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public ISession Session
        {
            get;
            private set;
        }

        /// <summary>
        /// Adds the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Add<TEntity>(TEntity entity)
        where TEntity : class
        {
            Guard.IsNotNull(entity, "entity");
            this.Session.Save(entity);
        }

        /// <summary>
        /// Attaches the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Attach<TEntity>(TEntity entity)
        where TEntity : class
        {
            Guard.IsNotNull(entity, "entity");
            this.Session.Lock(entity, LockMode.None);
        }

        /// <summary>
        /// Commit all changes made in  a container.
        /// </summary>
        public void Commit()
        {
            try
            {
                this.Session.Transaction.Commit();
            }
            catch (StaleObjectStateException ex)
            {
                throw new ConcurrencyException("Object was edited or deleted by another transaction", ex);
            }
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Delete<TEntity>(TEntity entity)
        where TEntity : class
        {
            this.Session.Lock(entity, LockMode.None);
            this.Session.Delete(entity);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Modifies the specified entity.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        public void Modify<TEntity>(TEntity entity)
        where TEntity : class
        {
            Guard.IsNotNull(entity, "entity");
            if (!this.Session.Contains(entity))
            {
                this.Session.Update(entity);
            }
        }

        /// <summary>
        /// Returns an IQueryable<TEntity>
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public System.Linq.IQueryable<TEntity> Query<TEntity>()
        where TEntity : class
        {
            return this.Session.Query<TEntity>();
        }

        /// <summary>
        /// Rollback changes not stored in databse at
        /// this moment. See references of UnitOfWork pattern
        /// </summary>
        public void RollbackChanges()
        {
            this.Session.Transaction.Rollback();
        }

        public void Start()
        {
            this.Session = this.sessionFactory.OpenSession();
            this.Session.BeginTransaction();
        }

        public void Start(System.Data.IsolationLevel isolationLevel)
        {
            this.Session = this.sessionFactory.OpenSession();
            this.Session.BeginTransaction(isolationLevel);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.Session != null && this.Session.IsOpen)
                {
                    if (this.Session.Transaction != null)
                    {
                        if (this.Session.Transaction.IsActive)
                        {
                            if (Transaction.Current == null)
                            {
                                this.Session.Transaction.Rollback();
                            }
                        }

                        this.Session.Transaction.Dispose();
                    }

                    this.Session.Dispose();
                    this.Session = null;
                }
            }
        }
    }
}