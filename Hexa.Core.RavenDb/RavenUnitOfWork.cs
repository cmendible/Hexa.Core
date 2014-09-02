//----------------------------------------------------------------------------------------------
// <copyright file="RavenUnitOfWork.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using Raven.Client;
    using Raven.Client.Document;
    using Raven.Client.Embedded;

    public class RavenUnitOfWork : IRavenUnitOfWork
    {
        private bool disposed;
        private IDocumentSession session;
        private RavenUnitOfWorkFactory factory;

        public RavenUnitOfWork(IDocumentSession session, IUnitOfWork previous, RavenUnitOfWorkFactory factory)
        {
            this.session = session;
            this.Previous = previous;
            this.factory = factory;
        }

        public IUnitOfWork Previous { get; private set; }

        public IDocumentSession DocumentSession { get { return this.session; } }

        /// <summary>
        /// Commit all changes made in  a container.
        /// </summary>
        public void Commit()
        {
            this.session.SaveChanges();
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            this.Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
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
                if (this.session != null)
                {
                    this.session.Dispose();

                    this.session = null;
                }

                this.factory.UpdateCurrent(this);

                // Note disposing has been done.
                this.disposed = true;
            }
        }

        /// <summary>
        /// Rollbacks the changes.
        /// </summary>
        public void RollbackChanges()
        {
        }
    }
}