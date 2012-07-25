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
    using NHibernate.Dialect;

    public class EntityMap<TEntity, TKey> : BaseClassMap<TEntity>
        where TEntity : BaseEntity<TEntity, TKey>
        where TKey : IEquatable<TKey>
    {
        #region Constructors

        public EntityMap()
        {
            if (typeof(TKey).Equals(typeof(int)))
            {
                Id(x => x.UniqueId)
                .UnsavedValue(0)
                .GeneratedBy.Native();
            }

            if (typeof(TKey).Equals(typeof(Guid)))
            {
                Id(x => x.UniqueId)
                .GeneratedBy.GuidComb();
            }

            // Use versioned timestamp as optimistick lock mechanism.
            OptimisticLock.Version();

            // Create Insert statements dynamically.
            DynamicInsert();
            // Create Update statements dynamically.
            DynamicUpdate();

            // Setup timestamp..
            if (Dialect is SQLiteDialect)
                Version(x => x.Version)
                .Column("Timestamp")
                .CustomType<TicksAsString>();
            else
                Version(x => x.Version)
                .Column("`Timestamp`")
                .CustomType<TicksAsString>();
        }

        #endregion Constructors
    }
}