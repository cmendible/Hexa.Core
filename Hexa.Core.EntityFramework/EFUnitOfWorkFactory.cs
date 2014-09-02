//----------------------------------------------------------------------------------------------
// <copyright file="EFUnitOfWorkFactory.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Data.Entity;
    using Data;

    public class EFUnitOfWorkFactory : BaseUnitOfWorkFactory, IDatabaseManager
    {
        private string connectionString;
        private Type contextType;

        public EFUnitOfWorkFactory(string connectionString, Type contextType)
        {
            this.connectionString = connectionString;
            this.contextType = contextType;
        }

        public DbContext CurrentDbContext
        {
            get
            {
                IEFUnitOfWork unitOfWork = this.Current as IEFUnitOfWork;
                Guard.IsNotNull(unitOfWork, "No UnitOfWork in scope!!!");

                return unitOfWork.DbContext;
            }
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

        protected override INestableUnitOfWork InternalCreate(IUnitOfWork previousUnitOfWork)
        {
            DbContext context = this.CreateContext();
            return new EFUnitOfWork(context, previousUnitOfWork, this);
        }

        private DbContext CreateContext()
        {
            return Activator.CreateInstance(contextType, new object[] { this.connectionString }) as DbContext;
        }
    }
}