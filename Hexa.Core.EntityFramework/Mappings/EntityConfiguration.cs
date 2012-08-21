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

    using System.Data.Entity.ModelConfiguration;

    public class EntityConfiguration<TEntity, TKey> : EntityTypeConfiguration<TEntity>
        where TEntity : BaseEntity<TEntity, TKey>
        where TKey : IEquatable<TKey>
    {
        #region Constructors

        public EntityConfiguration()
        {
            this.HasKey(x => x.UniqueId);

            this.Property(x => x.Version)
                .HasColumnName("Timestamp")
                .IsConcurrencyToken();
                //.CustomType<TicksAsString>();

            this.ToTable(Inflector.Underscore(typeof(TEntity).Name).ToUpper(), string.Empty);
        }

        #endregion Constructors
    }
}