#region License

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

#endregion

using System;
using NHibernate;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Web;

namespace Hexa.Core.Domain
{
    public class NHibernateUnitOfWork : IUnitOfWork
    {
        ITransactionWrapper _transactionWrapper;

        public NHibernateUnitOfWork(ISessionFactory sessionFactory)
        {
            if (RunningSession == null)
                RunningSession = sessionFactory.OpenSession();

            _transactionWrapper = _BeginTransaction(RunningSession);
        }

        internal NHibernateUnitOfWork(ISession session)
        {
            RunningSession = session;
            _transactionWrapper = _BeginTransaction(RunningSession);
        }

        private static ITransactionWrapper _BeginTransaction(ISession session)
        {
            if (session.Transaction.IsActive)
                return new NestedTransactionWrapper(session.Transaction);

            return new TransactionWrapper(session.BeginTransaction());
        }

        #region IUnitOfWork Members

        public void Commit()
        {
            try
            {
                _transactionWrapper.Commit();
            }
            catch (StaleObjectStateException ex)
            {
                throw new ConcurrencyException("Object was edited or deleted by another transaction", ex);
            }
        }

        public void RollbackChanges()
        {
            _transactionWrapper.Rollback();
        }

        public IEntitySet<TEntity> CreateSet<TEntity>() where TEntity : class
        {
            return new NHibernateObjectSet<TEntity>(RunningSession);
        }

        #endregion

        #region IDisposable Members

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
                                if (System.Transactions.Transaction.Current == null)
                                    _transactionWrapper.Rollback();
                            }

                            RunningSession.Transaction.Dispose();
                        }

                        RunningSession.Dispose();
                        RunningSession = null;
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region Nested

        /// <summary>
        /// Custom extension for OperationContext scope
        /// </summary>
        class ContainerExtension : IExtension<OperationContext>
        {
            #region Members

            public object Value { get; set; }

            #endregion

            #region IExtension<OperationContext> Members

            public void Attach(OperationContext owner)
            {

            }

            public void Detach(OperationContext owner)
            {

            }

            #endregion
        }

        #endregion

        private static string _key = "Hexa.Core.Domain.RunningSession.Key";

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
                    return HttpContext.Current.Items[_key.ToString()] as ISession;
                }
                else
                {
                    //Not in WCF or ASP.NET Environment, UnitTesting, WinForms, WPF etc.
                    return CallContext.GetData(_key.ToString()) as ISession;
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
                    HttpContext.Current.Items[_key.ToString()] = value;
                }
                else
                {
                    //Not in WCF or ASP.NET Environment, UnitTesting, WinForms, WPF etc.
                    CallContext.SetData(_key.ToString(), value);
                }
            }
        }
    }
    
}
