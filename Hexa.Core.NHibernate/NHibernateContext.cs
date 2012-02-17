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
using System.Data;

using NHibernate;

namespace Hexa.Core.Domain
{
    public class NHibernateContext : IQueryableUnitOfWork
    {
        ISession _session;

        public NHibernateContext(ISession session)
        {
            _session = session;
            _session.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        #region IUnitOfWork Members

        public void Commit()
        {
            try
            {
                _session.Transaction.Commit();
            }
            catch (StaleObjectStateException ex)
            {
                throw new ConcurrencyException("Object was edited or deleted by another transaction", ex);
            }
        }

        public void CommitAndRefreshChanges()
        {
            //try
            //{
            //    _session.Transaction.Commit();
            //}
            //catch (OptimisticConcurrencyException ex)
            //{

            //    //if client wins refresh data ( queries database and adapt original values
            //    //and re-save changes in client
            //    base.Refresh(RefreshMode.ClientWins, ex.StateEntries.Select(se => se.Entity));
            //    base.SaveChanges();
            //}
        }

        public void RollbackChanges()
        {
            _session.Transaction.Rollback();
        }

        #endregion

        #region IQueryableContext Members

        public IEntitySet<TEntity> CreateSet<TEntity>() where TEntity : class
        {
            return new NHibernateObjectSet<TEntity>(_session);
        }

        #endregion

        #region IContext Members

        public void SetChanges<TEntity>(TEntity item)
            where TEntity : class
        {
            _session.Update(item);
        }

        #endregion

        #region IDisposable Members

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnitOfWorkContext.RemoveCurrent();
                if (_session != null && _session.IsOpen)
                {
                    if (_session.Transaction != null)
                    {
                        if (_session.Transaction.IsActive)
                        {
                            if (System.Transactions.Transaction.Current == null)
                                _session.Transaction.Rollback();
                        }

                        _session.Transaction.Dispose();
                    }

                    _session.Dispose();

                    _session = null;
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
