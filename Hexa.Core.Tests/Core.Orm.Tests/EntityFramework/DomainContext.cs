//----------------------------------------------------------------------------------------------
// <copyright file="DomainContext.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Orm.Tests.EF
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;

    using Hexa.Core.Domain;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DomainContext : AuditableContext
    {
        public DomainContext(string nameOrConnectionString)
        : base(nameOrConnectionString)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.AddFromAssembly(typeof(Hexa.Core.Tests.Data.EntityAConfiguration).Assembly);
        }
    }
}