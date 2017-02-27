//----------------------------------------------------------------------------------------------
// <copyright file="GridData.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Web.Mvc.Helpers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Object to hold data used to fill the jqGrid
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public class GridData<TModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridData&lt;TModel&gt;"/> class.
        /// </summary>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="page">The page.</param>
        /// <param name="records">The records.</param>
        /// <param name="rows">The rows.</param>
        public GridData(int pageSize, int page, int records, List<TModel> rows)
        {
            this.total = (int)Math.Ceiling((decimal)records / (decimal)pageSize);
            this.page = page;
            this.records = records;
            this.rows = rows;
        }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int total
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the page.
        /// </summary>
        public int page
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the total number of records.
        /// </summary>
        public int records
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the data for the rows.
        /// </summary>
        public List<TModel> rows
        {
            get;
            private set;
        }
    }
}