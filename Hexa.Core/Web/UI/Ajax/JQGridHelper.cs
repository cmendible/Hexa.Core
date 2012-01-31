#region License

//===================================================================================
//Copyright 2010 HexaSystems Corporation
//===================================================================================
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//===================================================================================
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//===================================================================================

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using Hexa.Core.Domain.Specification;

namespace Hexa.Core.Web.UI.Ajax
{

    /// <summary>
    /// JQGrid Helper
    /// </summary>
    public class jqGridHelper
    {
        private string _gridName;
        private string _caption;
        private string _dataType;
        private string _pager;
        private string _cols;
        private string _width;
        private string _height;
        private string _onSelect = string.Empty;
        private string _onComplete = string.Empty;
        private string _multiSelect = "false";
        private string _multiSearch = "false";

        private jqGridHelper(string gridName)
        {
            _gridName = gridName;
        }

        public static jqGridHelper Create(string gridName)
        {
            return new jqGridHelper(gridName);
        }

        public jqGridHelper Caption(string caption)
        {
            _caption = caption;
            return this;
        }

        public jqGridHelper DataType(string dataType)
        {
            _dataType = dataType;
            return this;
        }

        public jqGridHelper Pager(string pager)
        {
            _pager = pager;
            return this;
        }

        public jqGridHelper Cols(string cols)
        {
            _cols = cols;
            return this;
        }

        public jqGridHelper Width(string width)
        {
            _width = width;
            return this;
        }

        public jqGridHelper Height(string height)
        {
            _height = height;
            return this;
        }

        public jqGridHelper OnSelect(string onSelect)
        {
            _onSelect = onSelect;
            return this;
        }

        public jqGridHelper OnComplete(string onComplete)
        {
            _onComplete = onComplete;
            return this;
        }

        public jqGridHelper MultiSelect(bool multiSelect)
        {
            _multiSelect = multiSelect.ToString().ToLower();
            return this;
        }

        public jqGridHelper MultiSearch(bool multiSearch)
        {
            _multiSearch = multiSearch.ToString().ToLower();
            return this;
        }

		public string ToString()
        {
            #region datagrid texts

			//string loadtext = "Cargando datos...";
			//string recordtext = "{0} - {1} de {2} elementos";
			//string emptyrecords = "No hay resultados";
			//string pgtext = "Pág: {0} de {1}"; //Paging input control text format.

            #endregion

            int rowNum = 10; // PageSize.
            int[] rowList = new int[] { 10, 20, 30 }; //Variable PageSize DropDownList. 

            string sortname = string.Empty; //Default SortColumn
            string sortorder = "asc"; //Default SortOrder.

            #region script

            string script = string.Format("$(\"{0}\").jqGrid(", _gridName) + "\r\n";
            script += "{" + "\r\n";
            script += string.Format("datatype: {0},", _dataType) + "\r\n";
            script += string.Format("colModel: {0},", _cols) + "\r\n";
            script += string.Format("pager: \"{0}\", ", _pager) + "\r\n";
			//script += string.Format("loadtext: '{0}',", loadtext) + "\r\n";
			//script += string.Format("recordtext: \"{0}\",", recordtext) + "\r\n";
			//script += string.Format("emptyrecords: '{0}',", emptyrecords) + "\r\n";
			//script += string.Format("pgtext : '{0}',", pgtext) + "\r\n";
            script += string.Format("rowNum: \"{0}\",", rowNum.ToString()) + "\r\n";
			//script += "rowList: [10,20,30]," + "\r\n";
            script += "viewrecords: true," + "\r\n";
            script += string.Format("multiselect: {0}, ", _multiSelect) + "\r\n";
            script += string.Format("sortname: \"{0}\", ", sortname) + "\r\n";
            script += string.Format("sortorder: \"{0}\", ", sortorder) + "\r\n";

			if (!string.IsNullOrEmpty(_onSelect))
				script += string.Format("onSelectRow: {0}, ", _onSelect) + "\r\n";

			if (!string.IsNullOrEmpty(_onComplete))
				script += string.Format("gridComplete: {0}, ", _onComplete) + "\r\n";

            if (!string.IsNullOrEmpty(_width))
                script += string.Format("width: \"{0}\",", _width) + "\r\n";
            else
                script += "autowidth: true," + "\r\n";

			script += "hidegrid: false," + "\r\n"; // Cant hide grids.

            var closeOnEscape = true;
            var closeAfterSearch = true;

            var searhBoxOptions = string.Format("closeOnEscape: {0}, multipleSearch: {1}, closeAfterSearch: {2} ",
                closeOnEscape.ToString().ToLower(),
                _multiSearch,
                closeAfterSearch.ToString().ToLower());

            script += string.Format("height: \"{0}\",", _height) + "\r\n";
            script += string.Format("caption: \"{0}\"", _caption) + "\r\n";
            script += "})" + "\r\n";
			script += string.Format(".navGrid(\"{0}\",", _pager);
            script += "{edit:false, add:false, search:true, del:false}, {}, {}, {}, {" + searhBoxOptions + "}, {});";

            #endregion

            return script;
        }
    }

    public class jqGridData
    {
        List<jqGridItem> _rows;
        int _totalRecords;
        int _pageIndex;
        int _pageSize;

		public jqGridData(IDictionary<string, List<string>> rows, int totalRecords, int pageIndex, int pageSize)
        {
            _rows = new List<jqGridItem>();
            _totalRecords = totalRecords;
            _pageIndex = pageIndex;
            _pageSize = pageSize;

            foreach (var row in rows)
                _rows.Add(new jqGridItem(row.Key, row.Value));
        }

		public int total { get { return (int)Math.Ceiling((decimal)_totalRecords / (decimal)_pageSize); } }

		public int page { get { return _pageIndex; } }

		public int records { get { return _totalRecords; } }

		public IEnumerable rows { get { return _rows; } }
    }

    public class jqGridItem
    {
        #region Properties

        /// <summary>
        /// RowId de la fila.
        /// </summary>
		public string id { get; protected set; }
        /// <summary>
        /// Fila del JQGrid.
        /// </summary>
		public IList<string> cell { get; protected set; }

        #endregion

        /// <summary>
        /// Contructor.
        /// </summary>
        public jqGridItem(string id, IList<string> values)
        {
            this.id = id;
			cell = values;
        }

    }

    [DataContract]
    public class jqFilter
    {
        [DataMember]
        public string groupOp { get; set; }
        [DataMember]
        public jqRule[] rules { get; set; }

        internal static jqFilter Create(string jsonData)
        {
            try
            {
                var serializer =
                  new DataContractJsonSerializer(typeof(jqFilter));
                System.IO.StringReader reader =
                  new System.IO.StringReader(jsonData);
                System.IO.MemoryStream ms =
                  new System.IO.MemoryStream(
                  Encoding.Default.GetBytes(jsonData));
                return serializer.ReadObject(ms) as jqFilter;
            }
            catch
            {
                return null;
            }
        }
    }

    [DataContract]
    public class jqRule
    {
        [DataMember]
        public string field { get; set; }
        [DataMember]
        public string op { get; set; }
        [DataMember]
        public string data { get; set; }
    }

    public static class jqGridSearchHelper
    {
        static Dictionary<string, string> _linqOperations = new Dictionary<string, string> {
            {"eq","=="}, //equal
            {"ne","!="},//not equal
            {"lt","<"}, //less than
            {"le","<="},//less than or equal
            {"gt",">"}, //greater than
            {"ge",">="},//greater than or equal
            {"bw","{0}.StartsWith({1})"}, //begins with
            {"bn","!{0}.StartsWith({1})"}, //doesn"t begin with
            {"in","{0}.Contains({1})"}, //is in
            {"ni","!{0}.Contains({1})"}, //is not in
            {"ew","{0}.EndsWith({1})"}, //ends with
            {"en","!{0}.EndsWith({1})"}, //doesn"t end with
            {"cn","{0}.Contains({1})"}, // contains
            {"nc","!{0}.Contains({1})"}  //doesn"t contain
        };

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

        public static jqFilter DeserializeFilters(string filters)
        {
            return jqFilter.Create(filters);
        }

    }

    public static class LinqExtensions
    {
        public static ISpecification<T> AndAlso<T>(this ISpecification<T> query, string column, object value, string operation) where T : class
        {
            return query.AndAlso(CreateSpecification<T>(column, value, operation));
        }

        public static ISpecification<T> OrElse<T>(this ISpecification<T> query, string column, object value, string operation) where T : class
        {
            return query.OrElse(CreateSpecification<T>(column, value, operation));
        }

        public static ISpecification<T> CreateSpecification<T>(string column, object value, string operation) where T : class
        {
            ParameterExpression parameter = Expression.Parameter(typeof(T), "p");

            MemberExpression memberAccess = _GetMemberAccess(column, parameter);

            if (memberAccess.Type == typeof(DateTime))
            {
                column += ".Date";
                memberAccess = _GetMemberAccess(column, parameter);
            }

            //change param value type
            //necessary to getting bool from string
            ConstantExpression filter = Expression.Constant
                (
                    Convert.ChangeType(value, memberAccess.Type)
                );

            Expression condition = null;
            LambdaExpression lambda = null;
            switch (operation)
            {
                //equal ==
                case "eq":
                    condition = Expression.Equal(memberAccess, filter);

                    lambda = Expression.Lambda(condition, parameter);
                    break;
                //not equal !=
                case "new":
                    condition = Expression.NotEqual(memberAccess, filter);
                    lambda = Expression.Lambda(condition, parameter);
                    break;
                //string.Contains()
                case "cn":
                case "bw":
                    condition = Expression.Call(memberAccess,
                        typeof(string).GetMethod("Contains"),
                        Expression.Constant(value));
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
                default:
                    throw new ArgumentOutOfRangeException("operation");
            }

            var hLambda = Expression<Func<T, bool>>.Lambda<Func<T, bool>>(condition, parameter);

            return new DirectSpecification<T>(hLambda);
        }

        private static MemberExpression _GetMemberAccess(string column, ParameterExpression parameter)
        {
            MemberExpression memberAccess = null;
            foreach (var property in column.Split('.'))
            {
                memberAccess = MemberExpression.Property
                   (memberAccess ?? (parameter as Expression), property);
            }
            return memberAccess;
        }
    }
}
