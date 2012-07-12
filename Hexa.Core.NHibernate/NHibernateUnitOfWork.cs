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

    [Export(typeof(IUnitOfWork))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NHibernateUnitOfWork : IUnitOfWork
    {
        #region Fields

        private readonly ITransactionWrapper _transactionWrapper;

        private static string _key = "Hexa.Core.Domain.RunningSession.Key";

        #endregion Fields

        #region Constructors

        public NHibernateUnitOfWork(ISessionFactory sessionFactory)
        {
            if (RunningSession == null)
            {
                RunningSession = sessionFactory.OpenSession();
            }

            this._transactionWrapper = _BeginTransaction(RunningSession);
        }

        internal NHibernateUnitOfWork(ISession session)
        {
            RunningSession = session;
            this._transactionWrapper = _BeginTransaction(RunningSession);
        }

        #endregion Constructors

        #region Properties

        public static ISession RunningSession
        {
            get
            {
                //Get object depending on  execution environment ( WCF without HttpContext,HttpContext or CallContext)
                if (OperationContext.Current != null)
                {
                    //WCF without HttpContext environment
                    var containerExtension = OperationContext.Current.Extensions.Find<ContainerExtension>();

                    if (containerExtension == null)
                    {
                        containerExtension = new ContainerExtension();

                        OperationContext.Current.Extensions.Add(containerExtension);
                    }

                    return containerExtension.Value as ISession;
                }
                else if (HttpContext.Current != null)
                {
                    return HttpContext.Current.Items[_key] as ISession;
                }
                else
                {
                    //Not in WCF or ASP.NET Environment, UnitTesting, WinForms, WPF etc.
                    return CallContext.GetData(_key) as ISession;
                }
            }
            set
            {
                //Get object depending on  execution environment ( WCF without HttpContext,HttpContext or CallContext)
                if (OperationContext.Current != null)
                {
                    //WCF without HttpContext environment
                    var containerExtension = OperationContext.Current.Extensions.Find<ContainerExtension>();

                    if (containerExtension == null)
                    {
                        containerExtension = new ContainerExtension();
                        OperationContext.Current.Extensions.Add(containerExtension);
                    }
                    containerExtension.Value = value;
                }
                else if (HttpContext.Current != null)
                {
                    HttpContext.Current.Items[_key] = value;
                }
                else
                {
                    //Not in WCF or ASP.NET Environment, UnitTesting, WinForms, WPF etc.
                    CallContext.SetData(_key, value);
                }
            }
        }

        #endregion Properties

        #region Methods

        public void Commit()
        {
            try
            {
                this._transactionWrapper.Commit();
            }
            catch (StaleObjectStateException ex)
            {
                throw new ConcurrencyException("Object was edited or deleted by another transaction", ex);
            }
        }

        public IEntitySet<TEntity> CreateSet<TEntity>()
            where TEntity : class
        {
            return new NHibernateObjectSet<TEntity>(RunningSession);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RollbackChanges()
        {
            this._transactionWrapper.Rollback();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnitOfWorkScope.DisposeCurrent();
                if (UnitOfWorkScope.RunningScopes.Count == 0)
                {
                    if (RunningSession != null && RunningSession.IsOpen)
                    {
                        if (RunningSession.Transaction != null)
                        {
                            if (RunningSession.Transaction.IsActive)
                            {
                                if (Transaction.Current == null)
                                {
                                    this._transactionWrapper.Rollback();
                                }
                            }

                            RunningSession.Transaction.Dispose();
                        }

                        RunningSession.Dispose();
                        RunningSession = null;
                    }
                }
            }
        }

        private static ITransactionWrapper _BeginTransaction(ISession session)
        {
            if (session.Transaction.IsActive)
            {
                return new NestedTransactionWrapper(session.Transaction);
            }

            return new TransactionWrapper(session.BeginTransaction());
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Custom extension for OperationContext scope
        /// </summary>
        private class ContainerExtension : IExtension<OperationContext>
        {
            #region Properties

            public object Value
            {
                get;
                set;
            }

            #endregion Properties

            #region Methods

            public void Attach(OperationContext owner)
            {
            }

            public void Detach(OperationContext owner)
            {
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}
