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

    using Data;

    using System.Data.Objects;
    using System.Data.Entity;

    [Export(typeof(IDatabaseManager))]
    public class EntityFrameworkOfWorkFactory<TContext> : IDatabaseManager where TContext : AuditableContext
    {
        #region Fields

        private string connectionString;

        #endregion Fields

        #region Constructors

        public EntityFrameworkOfWorkFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        #endregion Constructors

        #region Methods

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

        #endregion Methods
    }
}