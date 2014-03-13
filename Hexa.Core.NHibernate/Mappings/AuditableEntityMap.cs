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
            this.Map(x => x.CreatedAt)
                .Not.Nullable();

            this.Map(x => x.UpdatedAt)
                .Not.Nullable();

            Type keyType = typeof(TUserKey);
            if (keyType.Equals(typeof(string)))
            {
                this.Map(x => x.CreatedBy);
                this.Map(x => x.UpdatedBy);
            }
            else if (keyType.Equals(typeof(Guid)))
            {
                this.Map(x => x.CreatedBy)
                    .CustomType<StringToGuid>();
                this.Map(x => x.UpdatedBy)
                    .CustomType<StringToGuid>();
            }
            else if (keyType.Equals(typeof(int)))
            {
                this.Map(x => x.CreatedBy)
                    .CustomType<StringToInt>();
                this.Map(x => x.UpdatedBy)
                    .CustomType<StringToInt>();
            }
        }
    }
}