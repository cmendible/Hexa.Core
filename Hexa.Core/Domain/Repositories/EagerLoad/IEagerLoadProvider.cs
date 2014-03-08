//----------------------------------------------------------------------------------------------
// <copyright file="IEagerLoadProvider.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public interface IEagerLoadProvider
    {
        IEagerLoadRequest<TOriginating, TRelated> With<TOriginating, TRelated>(IQueryable<TOriginating> query, Expression<Func<TOriginating, TRelated>> relatedObjectSelector)
        where TOriginating : class;
    }
}