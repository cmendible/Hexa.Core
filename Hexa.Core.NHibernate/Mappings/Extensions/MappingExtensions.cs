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
    using FluentNHibernate.Mapping;

    using NHibernate.Dialect;

    public static class MappingExtensions
    {
        #region Methods

        private static PropertyPart TimePartWithDatabaseDefault<TEntity>(PropertyPart datePart, Dialect dialect)
        {
            if (dialect is MsSql2005Dialect)
            {
                datePart.Default("GETUTCDATE()");
            }

            if (dialect is MsSql2008Dialect)
            {
                datePart.Default("GETUTCDATE()");
            }

            if (dialect is Oracle10gDialect)
            {
                datePart.Default("SYSTIMESTAMP AT TIME ZONE 'UTC'");
            }

            if (dialect is SQLiteDialect)
            {
                datePart.Default("(datetime('now'))");
            }

            if (dialect is FirebirdDialect)
            {
                datePart.Default("current_date");
            }

            if (dialect is PostgreSQLDialect)
            {
                datePart.Default("current_timestamp");
            }

            return datePart;
        }

        #endregion Methods
    }
}