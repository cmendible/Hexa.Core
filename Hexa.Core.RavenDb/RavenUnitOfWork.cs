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

    using Raven.Client;

    [Export(typeof(IUnitOfWork))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class RavenUnitOfWork : IUnitOfWork
    {
        #region Fields

        private bool disposed;
        private IDocumentSession session;

        #endregion Fields

        #region Constructors

        public RavenUnitOfWork(IDocumentSession session)
        {
            this.session = session;
        }

        #endregion Constructors

        #region Methods

        public void Commit()
        {
            this.session.SaveChanges();
        }

        public IEntitySet<TEntity> CreateSet<TEntity>()
            where TEntity : class
        {
            return new RavenEntitySet<TEntity>(this.session);
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        public void RollbackChanges()
        {
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                UnitOfWorkScope.DisposeCurrent();
                if (this.session != null)
                {
                    this.session.Dispose();

                    this.session = null;
                }

                // Note disposing has been done.
                disposed = true;

            }
        }

        #endregion Methods
    }
}