//----------------------------------------------------------------------------------------------
// <copyright file="AuditableEntityMap.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;

    public class AuditableEntityMap<TEntity> : AuditableEntityMap<TEntity, string>
        where TEntity : AuditableEntity<TEntity>
    {
    }

    public class AuditableEntityMap<TEntity, TUserKey> : EntityMap<TEntity, Guid>
        where TEntity : AuditableEntity<TEntity>
    {
        public AuditableEntityMap()
        {
            Map(x => x.CreatedAt)
            .Not.Nullable();

            Map(x => x.UpdatedAt)
            .Not.Nullable();

            Type keyType = typeof(TUserKey);
            if (keyType.Equals(typeof(string)))
            {
                Map(x => x.CreatedBy);
                Map(x => x.UpdatedBy);
            }
            else if (keyType.Equals(typeof(Guid)))
            {
                Map(x => x.CreatedBy)
                .CustomType<StringToGuid>();
                Map(x => x.UpdatedBy)
                .CustomType<StringToGuid>();
            }
            else if (keyType.Equals(typeof(int)))
            {
                Map(x => x.CreatedBy)
                .CustomType<StringToInt>();
                Map(x => x.UpdatedBy)
                .CustomType<StringToInt>();
            }
        }
    }
}