// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// ===================================================================================

namespace Hexa.Core.Web.UI.Ajax
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;

    using Domain.Specification;

    [DataContract]
    public class jqFilter
    {
        [DataMember]
        public string groupOp
        {
            get;
            set;
        }

        [DataMember]
        public jqRule[] rules
        {
            get;
            set;
        }

        internal static jqFilter Create(string jsonData)
        {
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(jqFilter));
                using (StringReader reader = new StringReader(jsonData))
                using (MemoryStream ms = new MemoryStream(Encoding.Default.GetBytes(jsonData)))
                {
                    return serializer.ReadObject(ms) as jqFilter;
                }
            }
            catch
            {
                return null;
            }
        }
    }

    public class jqGridData
    {
        private readonly int _pageIndex;
        private readonly int _pageSize;
        private readonly List<jqGridItem> _rows;
        private readonly int _totalRecords;

        public jqGridData(IDictionary<string, List<string>> rows, int totalRecords, int pageIndex, int pageSize)
        {
            this._rows = new List<jqGridItem>();
            this._totalRecords = totalRecords;
            this._pageIndex = pageIndex;
            this._pageSize = pageSize;

            foreach (var row in rows)
            {
                this._rows.Add(new jqGridItem(row.Key, row.Value));
            }
        }

        public int page
        {
            get
            {
                return this._pageIndex;
            }
        }

        public int records
        {
            get
            {
                return this._totalRecords;
            }
        }

        public IEnumerable rows
        {
            get
            {
                return this._rows;
            }
        }

        public int total
        {
            get
            {
                return (int)Math.Ceiling(this._totalRecords / (decimal)this._pageSize);
            }
        }
    }

    /// <summary>
    /// JQGrid Helper
    /// </summary>
    public class jqGridHelper
    {
        private readonly string _gridName;

        private string _caption;
        private string _cols;
        private string _dataType;
        private string _firstSortOrder = "asc";
        private string _height;
        private string _multiSearch = "false";
        private string _multiSelect = "false";
        private string _onComplete = string.Empty;
        private string _onSelect = string.Empty;
        private string _onSelectAll = string.Empty;
        private string _pager;
        private string _sortColumn = string.Empty;
        private string _sortOrder = string.Empty;
        private string _width;

        private jqGridHelper(string gridName)
        {
            this._gridName = gridName;
        }

        public static jqGridHelper Create(string gridName)
        {
            return new jqGridHelper(gridName);
        }

        public jqGridHelper Caption(string caption)
        {
            this._caption = caption;
            return this;
        }

        public jqGridHelper Cols(string cols)
        {
            this._cols = cols;
            return this;
        }

        public jqGridHelper DataType(string dataType)
        {
            this._dataType = dataType;
            return this;
        }

        public jqGridHelper FirstSortOrder(string firstSortOrder)
        {
            this._firstSortOrder = firstSortOrder;
            return this;
        }

        public jqGridHelper Height(string height)
        {
            this._height = height;
            return this;
        }

        public jqGridHelper MultiSearch(bool multiSearch)
        {
            this._multiSearch = multiSearch.ToString().ToLower();
            return this;
        }

        public jqGridHelper MultiSelect(bool multiSelect)
        {
            this._multiSelect = multiSelect.ToString().ToLower();
            return this;
        }

        public jqGridHelper OnComplete(string onComplete)
        {
            this._onComplete = onComplete;
            return this;
        }

        public jqGridHelper OnSelect(string onSelect)
        {
            this._onSelect = onSelect;
            return this;
        }

        public jqGridHelper OnSelectAll(string onSelectAll)
        {
            this._onSelectAll = onSelectAll;
            return this;
        }

        public jqGridHelper Pager(string pager)
        {
            this._pager = pager;
            return this;
        }

        public jqGridHelper SortColumn(string sortColumn)
        {
            this._sortColumn = sortColumn;
            return this;
        }

        public jqGridHelper SortOrder(string sortOrder)
        {
            this._sortOrder = sortOrder;
            return this;
        }

        public string ToString()
        {
            #region datagrid texts

            // string loadtext = "Cargando datos...";
            // string recordtext = "{0} - {1} de {2} elementos";
            // string emptyrecords = "No hay resultados";
            // string pgtext = "Pág: {0} de {1}"; // Paging input control text format.

            #endregion

            // PageSize.
            int rowNum = 10;

            // Variable PageSize DropDownList.
            var rowList = new[] { 10, 20, 30 };

            #region script

            string script = string.Format("$(\"{0}\").jqGrid(", this._gridName) + "\r\n";
            script += "{" + "\r\n";
            script += string.Format("datatype: {0},", this._dataType) + "\r\n";
            script += string.Format("colModel: {0},", this._cols) + "\r\n";
            script += string.Format("pager: \"{0}\", ", this._pager) + "\r\n";
            script += string.Format("rowNum: \"{0}\",", rowNum) + "\r\n";
            script += "viewrecords: true," + "\r\n";
            script += string.Format("multiselect: {0}, ", this._multiSelect) + "\r\n";

            if (!string.IsNullOrEmpty(this._onSelect))
            {
                script += string.Format("onSelectRow: {0}, ", this._onSelect) + "\r\n";
            }

            if (!string.IsNullOrEmpty(this._onSelectAll))
            {
                script += string.Format("onSelectAll: {0}, ", this._onSelectAll) + "\r\n";
            }

            if (!string.IsNullOrEmpty(this._onComplete))
            {
                script += string.Format("gridComplete: {0}, ", this._onComplete) + "\r\n";
            }

            if (!string.IsNullOrEmpty(this._width))
            {
                script += string.Format("width: \"{0}\",", this._width) + "\r\n";
            }
            else
            {
                script += "autowidth: true," + "\r\n";
            }

            // Cant hide grids.
            script += "hidegrid: false," + "\r\n";

            bool closeOnEscape = true;
            bool closeAfterSearch = true;

            string searhBoxOptions = string.Format(
                                         "closeOnEscape: {0}, multipleSearch: {1}, closeAfterSearch: {2} ",
                                         closeOnEscape.ToString().ToLower(),
                                         this._multiSearch,
                                         closeAfterSearch.ToString().ToLower());

            if (!string.IsNullOrEmpty(this._sortColumn))
            {
                script += string.Format("sortname: \"{0}\",", this._sortColumn) + "\r\n";

                if (!string.IsNullOrEmpty(this._sortOrder))
                {
                    script += string.Format("sortorder: \"{0}\",", this._sortOrder) + "\r\n";
                }
            }

            if (!string.IsNullOrEmpty(this._firstSortOrder))
            {
                script += string.Format("firstsortorder: \"{0}\",", this._firstSortOrder) + "\r\n";
            }

            script += string.Format("height: \"{0}\",", this._height) + "\r\n";
            script += string.Format("caption: \"{0}\"", this._caption) + "\r\n";

            script += "})" + "\r\n";
            script += string.Format(".navGrid(\"{0}\",", this._pager);
            script += "{edit:false, add:false, search:true, del:false}, {}, {}, {}, {" + searhBoxOptions + "}, {});";

            #endregion

            return script;
        }

        public jqGridHelper Width(string width)
        {
            this._width = width;
            return this;
        }
    }

    public class jqGridItem
    {
        /// <summary>
        /// Contructor.
        /// </summary>
        public jqGridItem(string id, IList<string> values)
        {
            this.id = id;
            this.cell = values;
        }

        /// <summary>
        /// Fila del JQGrid.
        /// </summary>
        public IList<string> cell
        {
            get;
            protected set;
        }

        /// <summary>
        /// RowId de la fila.
        /// </summary>
        public string id
        {
            get;
            protected set;
        }
    }

    public static class jqGridSearchHelper
    {
        private static readonly Dictionary<string, string> _linqOperations = new Dictionary<string, string>
        {
            { "eq", "==" },

            // equal
            { "ne", "!=" },

            // not equal
            { "lt", "<" },

            // less than
            { "le", "<=" },

            // less than or equal
            { "gt", ">" },

            // greater than
            { "ge", ">=" },

            // greater than or equal
            { "bw", " { 0 } .StartsWith( { 1 })" },

            // begins with
            { "bn", "! { 0 } .StartsWith( { 1 })" },

            // doesn"t begin with
            { "in", " { 0 } .Contains( { 1 })" },

            // is in
            { "ni", "! { 0 } .Contains( { 1 })" },

            // is not in
            { "ew", " { 0 } .EndsWith( { 1 })" },

            // ends with
            { "en", "! { 0 } .EndsWith( { 1 })" },

            // doesn"t end with
            { "cn", " { 0 } .Contains( { 1 })" },

            // contains
            { "nc", "! { 0 } .Contains( { 1 })" }

            // doesn"t contain
        };

        public static jqFilter DeserializeFilters(string filters)
        {
            return jqFilter.Create(filters);
        }

        public static string ToLinq(string operation, string column, string value)
        {
            column += ".ToString()";
            value = string.Format("\"{0}\"", value);

            if (operation == "bw" || operation == "bn" || operation == "ew" || operation == "en" ||
                operation == "cn" || operation == "nc" || operation == "in" || operation == "ni")
            {
                return string.Format(_linqOperations[operation], column, value);
            }
            else
            {
                return string.Format("{0} {1} {2}", column, _linqOperations[operation], value);
            }
        }
    }

    [DataContract]
    public class jqRule
    {
        [DataMember]
        public string data
        {
            get;
            set;
        }

        [DataMember]
        public string field
        {
            get;
            set;
        }

        [DataMember]
        public string op
        {
            get;
            set;
        }
    }

    [Obsolete]
    public static class LinqExtensions
    {
        [Obsolete]
        public static ISpecification<T> AndAlso<T>(
            this ISpecification<T> query,
            string column,
            object value,
            string operation)
        where T : class
        {
            return query.AndAlso(CreateSpecification<T>(column, value, operation));
        }

        [Obsolete]
        public static ISpecification<T> CreateSpecification<T>(string column, object value, string operation)
        where T : class
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");

            MemberExpression memberAccess = _GetMemberAccess(column, parameter);

            if (memberAccess.Type == typeof(DateTime))
            {
                column += ".Date";
                memberAccess = _GetMemberAccess(column, parameter);
            }

            // change param value type
            // necessary to getting bool from string
            ConstantExpression filter = Expression.Constant(Convert.ChangeType(value, memberAccess.Type));

            Expression condition = null;
            LambdaExpression lambda = null;
            switch (operation)
            {
                // equal ==
            case "eq":
                condition = Expression.Equal(memberAccess, filter);

                lambda = Expression.Lambda(condition, parameter);
                break;

                // not equal !=
            case "ne":
                condition = Expression.NotEqual(memberAccess, filter);
                lambda = Expression.Lambda(condition, parameter);
                break;

                // string.Contains()
            case "cn":
                condition = Expression.Call(
                                memberAccess,
                                typeof(string).GetMethod("Contains"),
                                Expression.Constant(value));

                lambda = Expression.Lambda(condition, parameter);
                break;
            case "bw":
                condition = Expression.Call(
                                memberAccess,
                                typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
                                Expression.Constant(value));

                lambda = Expression.Lambda(condition, parameter);
                break;
            case "bn":
                condition = Expression.Call(
                                memberAccess,
                                typeof(string).GetMethod("StartsWith", new[] { typeof(string) }),
                                Expression.Constant(value));

                condition = Expression.Not(condition);

                lambda = Expression.Lambda(condition, parameter);
                break;
            case "ew":
                condition = Expression.Call(
                                memberAccess,
                                typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
                                Expression.Constant(value));

                lambda = Expression.Lambda(condition, parameter);
                break;
            case "en":
                condition = Expression.Call(
                                memberAccess,
                                typeof(string).GetMethod("EndsWith", new[] { typeof(string) }),
                                Expression.Constant(value));

                condition = Expression.Not(condition);

                lambda = Expression.Lambda(condition, parameter);
                break;
            case "gt":
                condition = Expression.GreaterThan(memberAccess, filter);

                lambda = Expression.Lambda(condition, parameter);
                break;
            case "ge":
                condition = Expression.GreaterThanOrEqual(memberAccess, filter);

                lambda = Expression.Lambda(condition, parameter);
                break;
            case "lt":
                condition = Expression.LessThan(memberAccess, filter);

                lambda = Expression.Lambda(condition, parameter);
                break;
            case "le":
                condition = Expression.LessThanOrEqual(memberAccess, filter);

                lambda = Expression.Lambda(condition, parameter);
                break;
            case "nc":
                condition = Expression.Call(
                                memberAccess,
                                typeof(string).GetMethod("Contains"),
                                Expression.Constant(value));

                condition = Expression.Not(condition);

                lambda = Expression.Lambda(condition, parameter);

                break;
            default:
                throw new ArgumentOutOfRangeException("operation");
            }

            Expression<Func<T, bool>> hLambda = Expression.Lambda<Func<T, bool>>(condition, parameter);

            return new DirectSpecification<T>(hLambda);
        }

        [Obsolete]
        public static ISpecification<T> OrElse<T>(
            this ISpecification<T> query,
            string column,
            object value,
            string operation)
        where T : class
        {
            return query.OrElse(CreateSpecification<T>(column, value, operation));
        }

        [Obsolete]
        private static MemberExpression _GetMemberAccess(string column, ParameterExpression parameter)
        {
            MemberExpression memberAccess = null;
            foreach (string property in column.Split('.'))
            {
                memberAccess = Expression.Property
                               (memberAccess ?? (parameter as Expression), property);
            }

            return memberAccess;
        }
    }
}