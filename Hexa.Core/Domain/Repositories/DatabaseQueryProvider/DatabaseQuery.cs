//----------------------------------------------------------------------------------------------
// <copyright file="DatabaseQuery.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;

    public static class DatabaseQuery
    {
        public static Func<IDatabaseQueryProvider> DatabaseQueryProvider;

        public static IList<TEntity> ExecuteQuery<TEntity>(string queryName, IDictionary<string, object> parameters)
        {
            return DatabaseQueryProvider().ExecuteQuery<TEntity>(queryName, parameters);
        }
    }
}