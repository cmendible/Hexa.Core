//----------------------------------------------------------------------------------------------
// <copyright file="EFEagerLoadProvider.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    public class EFEagerLoadProvider : IEagerLoadProvider
    {
        public IEagerLoadRequest<TOriginating, TRelated> With<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
        where TOriginating : class
        {
            var fetch = QueryableExtensions.Include(query, relatedObjectSelector);
            return new BaseEagerLoadRequest<TOriginating, TRelated>(fetch);
        }
    }
}