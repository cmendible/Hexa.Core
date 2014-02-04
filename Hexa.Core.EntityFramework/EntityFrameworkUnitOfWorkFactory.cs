//----------------------------------------------------------------------------------------------
// <copyright file="EntityFrameworkUnitOfWorkFactory.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Data.Entity;
    using Data;

    public class EntityFrameworkUnitOfWorkFactory<TContext> : IDatabaseManager
        where TContext : AuditableContext
    {
        private string connectionString;

        public EntityFrameworkUnitOfWorkFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IUnitOfWork Create()
        {
            return new EntityFrameworkUnitOfWork(this.CreateContext());
        }

        public void CreateDatabase()
        {
            using (DbContext context = this.CreateContext())
            {
                context.Database.CreateIfNotExists();
            }
        }

        public bool DatabaseExists()
        {
            using (DbContext context = this.CreateContext())
            {
                return context.Database.Exists();
            }
        }

        public void DeleteDatabase()
        {
            using (DbContext context = this.CreateContext())
            {
                context.Database.Delete();
            }
        }

        public void ValidateDatabaseSchema()
        {
            using (DbContext context = this.CreateContext())
            {
                context.Database.CompatibleWithModel(true);
            }
        }

        private TContext CreateContext()
        {
            return Activator.CreateInstance(typeof(TContext), new object[] { this.connectionString }) as TContext;
        }
    }
}