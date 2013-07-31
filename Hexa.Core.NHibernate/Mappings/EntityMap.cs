//----------------------------------------------------------------------------------------------
// <copyright file="EntityMap.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    using NHibernate.Dialect;

    public class EntityMap<TEntity, TKey> : BaseClassMap<TEntity>
        where TEntity : BaseEntity<TEntity, TKey>
        where TKey : struct, IEquatable<TKey>
    {
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
            {
                Version(x => x.Version)
                .Column("Timestamp")
                .CustomType<TicksAsString>();
            }
            else
            {
                Version(x => x.Version)
                .Column("`Timestamp`")
                .CustomType<TicksAsString>();
            }
        }
    }
}