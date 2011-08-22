#region License

//===================================================================================
//Copyright 2010 HexaSystems Corporation
//===================================================================================
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//===================================================================================
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//===================================================================================

#endregion

using System;
using System.Linq.Expressions;
using FluentNHibernate.Mapping;
using NHibernate.Dialect;

namespace Hexa.Core.Domain
{
    public class AuditableRootEntityMap<TEntity> : RootEntityMap<TEntity>
		where TEntity : AuditableRootEntity<TEntity>
	{
        public AuditableRootEntityMap()
			: base()
		{
            MapTimePart(x => x.CreatedAt)
				.Generated.Insert();

            Map(x => x.UpdatedAt);

            Map(x => x.CreatedBy);

            Map(x => x.UpdatedBy);
		}

        private PropertyPart MapTimePart(Expression<Func<TEntity, object>> expression)
        {
            var tmp = Map(expression);

            if (Dialect is MsSql2005Dialect)
                tmp.Default("GETUTCDATE()");

            if (Dialect is MsSql2008Dialect)
                tmp.Default("GETUTCDATE()");

            if (Dialect is Oracle10gDialect)
                tmp.Default("SYSTIMESTAMP AT TIME ZONE 'UTC'");

            if (Dialect is SQLiteDialect)
                tmp.Default("(datetime('now'))");
				
			if (Dialect is FirebirdDialect)
                tmp.Default("current_date");

            return tmp;
        }
        
	}
}
