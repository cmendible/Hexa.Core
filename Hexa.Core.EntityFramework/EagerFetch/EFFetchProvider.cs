//----------------------------------------------------------------------------------------------
// <copyright file="EFFetchProvider.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using System.Linq.Expressions;

    public class EFFetchProvider : IFetchProvider
    {
        public IFetchRequest<TOriginating, TRelated> Fetch<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
        where TOriginating : class
        {
            var fetch = DbExtensions.Include(query, relatedObjectSelector);
            return new BaseFetchRequest<TOriginating, TRelated>(fetch);
        }
    }
}