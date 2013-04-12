#region Header

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion Header

namespace Hexa.Core.Domain
{
    using System;
    using System.ComponentModel.Composition;
    using System.Runtime.Remoting.Messaging;
    using System.ServiceModel;
    using System.Transactions;
    using System.Web;

    using NHibernate;
    using NHibernate.Linq;

    [Export(typeof(IUnitOfWork))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NHibernateUnitOfWork : INHibernateUnitOfWork
    {
        ISessionFactory sessionFactory;

        #region Constructors

        public NHibernateUnitOfWork(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        #endregion Constructors

        #region Properties

        public ISession Session
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public void Start()
        {
            Session = sessionFactory.OpenSession();
            Session.BeginTransaction();
        }

        public void Start(System.Data.IsolationLevel isolationLevel)
        {
            Session = sessionFactory.OpenSession();
            Session.BeginTransaction(isolationLevel);
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
        public void Delete<TEntity>(TEntity entity) where TEntity : class
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

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Session != null && Session.IsOpen)
                {
                    if (Session.Transaction != null)
                    {
                        if (Session.Transaction.IsActive)
                        {
                            if (Transaction.Current == null)
                            {
                                this.Session.Transaction.Rollback();
                            }
                        }

                        Session.Transaction.Dispose();
                    }

                    Session.Dispose();
                    Session = null;
                }
            }
        }

        #endregion Methods
    }
}