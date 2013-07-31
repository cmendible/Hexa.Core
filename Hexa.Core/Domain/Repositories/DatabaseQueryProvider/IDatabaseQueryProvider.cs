//----------------------------------------------------------------------------------------------
// <copyright file="IDatabaseQueryProvider.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System.Collections.Generic;

    public interface IDatabaseQueryProvider
    {
        IList<TEntity> ExecuteQuery<TEntity>(string queryName, IDictionary<string, object> parameters);
    }
}