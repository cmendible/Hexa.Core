//----------------------------------------------------------------------------------------------
// <copyright file="PagedElements.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;

    public class PagedElements<TEntity>
        where TEntity : class
    {
        public PagedElements(IEnumerable<TEntity> elements, int totalElements)
        {
            this.Elements = elements;
            this.TotalElements = totalElements;
        }

        public IEnumerable<TEntity> Elements
        {
            get;
            private set;
        }

        public int TotalElements
        {
            get;
            private set;
        }

        public int TotalPages(int pageSize)
        {
            return (int)Math.Ceiling(Convert.ToDouble(this.TotalElements) / pageSize);
        }
    }
}