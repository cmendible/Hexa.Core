//----------------------------------------------------------------------------------------------
// <copyright file="EntityFrameworkUnitOfWork.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Data.Entity;

    public class EntityFrameworkUnitOfWork : IEntityFrameworkUnitOfWork
    {
        private DbContext dbContext;
        private bool disposed;

        public EntityFrameworkUnitOfWork(DbContext context)
        {
            this.dbContext = context;
        }

        public DbContext DbContext
        {
            get
            {
                return this.dbContext;
            }
        }

        public void Add<TEntity>(TEntity entity)
        where TEntity : class
        {
            this.dbContext.Set<TEntity>().Add(entity);
        }

        public void Attach<TEntity>(TEntity entity)
        where TEntity : class
        {
            this.dbContext.Set<TEntity>().Attach(entity);
        }

        public void Commit()
        {
            this.dbContext.SaveChanges();
        }

        public void Delete<TEntity>(TEntity entity)
        where TEntity : class
        {
            this.dbContext.Set<TEntity>().Remove(entity);
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

        public void Modify<TEntity>(TEntity entity)
        where TEntity : class
        {
            var entry = this.dbContext.Entry(entity);

            entry.State = System.Data.EntityState.Modified;
        }

        public System.Linq.IQueryable<TEntity> Query<TEntity>()
        where TEntity : class
        {
            return this.dbContext.Set<TEntity>();
        }

        public void Start()
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