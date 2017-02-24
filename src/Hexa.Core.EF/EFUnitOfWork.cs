//----------------------------------------------------------------------------------------------
// <copyright file="EFUnitOfWork.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using Microsoft.EntityFrameworkCore;

    public class EFUnitOfWork : IEFUnitOfWork
    {
        private DbContext dbContext;
        private bool disposed;

        public EFUnitOfWork(DbContext context, IUnitOfWork previous)
        {
            this.dbContext = context;
        }

        public DbContext DbContext { get { return this.dbContext; } }

        public void Commit()
        {
            this.dbContext.SaveChanges();
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
                if (this.dbContext != null)
                {
                    this.dbContext.Dispose();

                    this.dbContext = null;
                }

                // Note disposing has been done.
                this.disposed = true;
            }
        }

    }
}