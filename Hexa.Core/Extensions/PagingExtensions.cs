//----------------------------------------------------------------------------------------------
// <copyright file="PagingExtensions.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Hexa.Core;

    public static class PagingExtensions
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            Guard.Against<ArgumentException>(pageNumber <= 0, "pageNumber");

            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }

        public static IEnumerable<T> Page<T>(this IEnumerable<T> query, int pageNumber, int pageSize)
        {
            Guard.Against<ArgumentException>(pageNumber <= 0, "pageNumber");

            return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
        }
    }
}