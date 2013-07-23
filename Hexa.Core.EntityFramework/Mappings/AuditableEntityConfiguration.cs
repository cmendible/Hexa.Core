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

namespace Hexa.Core.Domain
{
    using System;
    using System.Data.Entity.ModelConfiguration;

    public class AuditableEntityConfiguration<TEntity> : AuditableEntityConfiguration<TEntity, string>
        where TEntity : AuditableEntity<TEntity>
    {
    }

    public class AuditableEntityConfiguration<TEntity, TUserKey> : EntityConfiguration<TEntity, Guid>
        where TEntity : AuditableEntity<TEntity>
    {
        public AuditableEntityConfiguration()
        {
            this.Property(x => x.CreatedAt)
            .IsRequired();

            this.Property(x => x.UpdatedAt)
            .IsRequired();

            Type keyType = typeof(TUserKey);
            if (keyType.Equals(typeof(string)))
            {
                this.Property(x => x.CreatedBy);
                this.Property(x => x.UpdatedBy);
            }
            else if (keyType.Equals(typeof(Guid)))
            {
                this.Property(x => x.CreatedBy)
                .HasColumnType("UniqueIdentifier");
                this.Property(x => x.UpdatedBy)
                .HasColumnType("UniqueIdentifier");
            }
            else if (keyType.Equals(typeof(int)))
            {
                this.Property(x => x.CreatedBy)
                .HasColumnType("int");
                this.Property(x => x.UpdatedBy)
                .HasColumnType("int");
            }
        }
    }
}