//----------------------------------------------------------------------------------------------
// <copyright file="AuditableContext.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    using Hexa.Core.Security;
    using Hexa.Core.Validation;

    public class AuditableContext : DbContext
    {
        public AuditableContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public override int SaveChanges()
        {
            this.AuditEntities();

            this.ValidateEntities();

            return base.SaveChanges();
        }

        protected void AuditEntities()
        {
            string userUniqueId = string.Empty;

            var user = ApplicationContext.User;
            if (user != null)
            {
                var identity = ApplicationContext.User.Identity as ICoreIdentity;
                if (identity != null)
                {
                    userUniqueId = identity.Id;
                }
            }

            DateTime now = DateTime.Now;

            foreach (DbEntityEntry<IAuditableEntity> entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                if (entry.State == System.Data.Entity.EntityState.Added || entry.State == System.Data.Entity.EntityState.Modified)
                {
                    entry.Entity.GetType().GetProperty("Version").SetValue(entry.Entity, DateTime.UtcNow.Ticks.ToString(), null);
                }

                if (entry.State == System.Data.Entity.EntityState.Added)
                {
                    entry.Entity.CreatedBy = userUniqueId;
                    entry.Entity.UpdatedBy = userUniqueId;
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                }
                else if (entry.State == System.Data.Entity.EntityState.Modified)
                {
                    entry.Entity.UpdatedBy = userUniqueId;
                    entry.Entity.UpdatedAt = now;
                }
            }
        }

        protected void ValidateEntities()
        {
            foreach (DbEntityEntry<IValidatable> entry in ChangeTracker.Entries<IValidatable>())
            {
                entry.Entity.AssertValidation();
            }
        }

        private string GetEntityUniqueId(object entity)
        {
            return string.Empty;
        }
    }
}