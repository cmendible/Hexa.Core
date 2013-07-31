//----------------------------------------------------------------------------------------------
// <copyright file="EagerFetch.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public static class EagerFetch
    {
        public static Func<IFetchProvider> FetchingProvider;

        public static IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(
            this IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
            where TOriginating : class
        {
            return FetchingProvider().Fetch(query, relatedObjectSelector);
        }
    }
}