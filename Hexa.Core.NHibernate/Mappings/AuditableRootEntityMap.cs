#region License

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

#endregion

namespace Hexa.Core.Domain
{
    using System;

    public class AuditableRootEntityMap<TEntity> : AuditableRootEntityMap<TEntity, string>
        where TEntity : AuditableRootEntity<TEntity>
    {
    }

    public class AuditableRootEntityMap<TEntity, TKey> : RootEntityMap<TEntity>
        where TEntity : AuditableRootEntity<TEntity>
    {
        public AuditableRootEntityMap()
        {
            Map(x => x.CreatedAt)
                .Not.Nullable();

            Map(x => x.UpdatedAt)
                .Not.Nullable();

            Type keyType = typeof(TKey);
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