//----------------------------------------------------------------------------------------------
// <copyright file="BaseEagerLoadRequest.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class BaseEagerLoadRequest<TQueried, TFetch> : IEagerLoadRequest<TQueried, TFetch>
        where TQueried : class
    {
        private IQueryable<TQueried> query;

        public BaseEagerLoadRequest(IQueryable<TQueried> query)
        {
            this.query = query;
        }

        public Type ElementType
        {
            get
            {
                return this.query.ElementType;
            }
        }

        public Expression Expression
        {
            get
            {
                return this.query.Expression;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return this.query.Provider;
            }
        }

        public IEnumerator<TQueried> GetEnumerator()
        {
            return this.query.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.query.GetEnumerator();
        }
    }
}