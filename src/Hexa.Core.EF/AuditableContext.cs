//----------------------------------------------------------------------------------------------
// <copyright file="AuditableContext.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Hexa.Core.Validation;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using System.Reflection;
    using System.Security.Claims;

    public class AuditableContext : DbContext
    {
        Assembly[] assemblies;

        public AuditableContext(DbContextOptions options, Assembly[] assemblies)
            : base(options)
        {
            this.assemblies = assemblies;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddEntityConfigurationsFromAssembly(this.assemblies);
        }

        public override int SaveChanges()
        {
            this.AuditEntities();

            this.ValidateEntities();

            return base.SaveChanges();
        }

        protected void AuditEntities()
        {
            // Get the authenticated user name 
            string userName = string.Empty;

            var user = ClaimsPrincipal.Current;
            if (user != null)
            {
                var identity = user.Identity;
                if (identity != null)
                {
                    userName = identity.Name;
                }
            }

            // Get current date & time
            DateTime now = DateTime.Now;

            // For every changed entity marked as IAditable set the values for the audit properties
            foreach (EntityEntry<IAuditableEntity> entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                // If the entity was added.
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedBy").CurrentValue = userName;
                    entry.Property("CreatedAt").CurrentValue = now;
                }
                else if (entry.State == EntityState.Modified) // If the entity was updated
                {
                    entry.Property("UpdatedBy").CurrentValue = userName;
                    entry.Property("UpdatedAt").CurrentValue = now;
                }
            }
        }

        protected void ValidateEntities()
        {
            foreach (EntityEntry<IValidatable> entry in ChangeTracker.Entries<IValidatable>())
            {
                entry.Entity.AssertValidation();
            }
        }
    }
}