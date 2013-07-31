//----------------------------------------------------------------------------------------------
// <copyright file="MappingExtensions.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using FluentNHibernate.Mapping;

    using NHibernate.Dialect;

    public static class MappingExtensions
    {
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
    }
}