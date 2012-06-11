#region License

//===================================================================================
//Copyright 2010 HexaSystems Corporation
//===================================================================================
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//===================================================================================
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//===================================================================================

#endregion

using System;
using NHibernate;

namespace Hexa.Core.Domain
{
    public class NHibernateUnitOfWork : IUnitOfWork
    {
        static ISession _session;
        ITransactionWrapper _transactionWrapper;

        public NHibernateUnitOfWork(ISessionFactory sessionFactory)
        {
            if (_session == null)
                _session = sessionFactory.OpenSession();

            _transactionWrapper = _BeginTransaction(_session);
        }

        internal NHibernateUnitOfWork(ISession session)
        {
            _session = session;
            _transactionWrapper = _BeginTransaction(_session);
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
            return new NHibernateObjectSet<TEntity>(_session);
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
                    if (_session != null && _session.IsOpen)
                    {
                        if (_session.Transaction != null)
                        {
                            if (_session.Transaction.IsActive)
                            {
                                if (System.Transactions.Transaction.Current == null)
                                    _transactionWrapper.Rollback();
                            }

                            _session.Transaction.Dispose();
                        }

                        _session.Dispose();

                        _session = null;
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
    }
    
}
