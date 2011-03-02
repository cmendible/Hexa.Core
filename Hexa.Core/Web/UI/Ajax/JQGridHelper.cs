using System;
using System.Collections;
using System.Collections.Generic;

namespace Hexa.Core.Web.UI.Ajax
{

    /// <summary>
    /// JQGrid Helper
    /// </summary>
    public static class JQGridHelper
    {
        /// <summary>
        /// Renders JQGrid script.
        /// </summary>
        /// <param name="gridName">Name of the grid.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="pager">The pager.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
        public static string JQGrid(string gridName, string caption, string dataType, string pager, string cols, string width, string height)
        {
            return JQGrid(gridName, caption, dataType, pager, cols, width, height, false, string.Empty, string.Empty);
        }

		/// <summary>
		/// Renders JQGrid script.
		/// </summary>
		/// <param name="gridName">Name of the grid.</param>
		/// <param name="caption">The caption.</param>
		/// <param name="dataType">Type of the data.</param>
		/// <param name="pager">The pager.</param>
		/// <param name="width">The width.</param>
		/// <param name="height">The height.</param>
		/// <returns></returns>
		public static string JQGrid(string gridName, string caption, string dataType, string pager, string cols,
			string width, string height, string onSelect)
		{
			return JQGrid(gridName, caption, dataType, pager, cols, width, height, false, onSelect, string.Empty);
		}

        /// <summary>
        /// Renders JQGrid script.
        /// </summary>
        /// <param name="gridName">Name of the grid.</param>
        /// <param name="caption">The caption.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="pager">The pager.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="multiselect">if set to <c>true</c> [multiselect].</param>
        /// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "rowList"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "multiselect")]
		public static string JQGrid(string gridName, string caption, string dataType, string pager,
			string cols, string width, string height, bool multiselect, string onSelect, string gridComplete)
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

            string script = string.Format("$(\"{0}\").jqGrid(", gridName) + "\r\n";
            script += "{" + "\r\n";
            script += string.Format("datatype: {0},", dataType) + "\r\n";
            script += string.Format("colModel: {0},", cols) + "\r\n";
            script += string.Format("pager: \"{0}\", ", pager) + "\r\n";
			//script += string.Format("loadtext: '{0}',", loadtext) + "\r\n";
			//script += string.Format("recordtext: \"{0}\",", recordtext) + "\r\n";
			//script += string.Format("emptyrecords: '{0}',", emptyrecords) + "\r\n";
			//script += string.Format("pgtext : '{0}',", pgtext) + "\r\n";
            script += string.Format("rowNum: \"{0}\",", rowNum.ToString()) + "\r\n";
			//script += "rowList: [10,20,30]," + "\r\n";
            script += "viewrecords: true," + "\r\n";
            script += string.Format("multiselect: {0}, ", multiselect.ToString().ToLower()) + "\r\n";
            script += string.Format("sortname: \"{0}\", ", sortname) + "\r\n";
            script += string.Format("sortorder: \"{0}\", ", sortorder) + "\r\n";

			if (!string.IsNullOrEmpty(onSelect))
				script += string.Format("onSelectRow: {0}, ", onSelect) + "\r\n";

			if (!string.IsNullOrEmpty(gridComplete))
				script += string.Format("gridComplete: {0}, ", gridComplete) + "\r\n";

            if (!string.IsNullOrEmpty(width))
                script += string.Format("width: \"{0}\",", width) + "\r\n";
            else
                script += "autowidth: true," + "\r\n";

			script += "hidegrid: false," + "\r\n"; // Cant hide grids.

            script += string.Format("height: \"{0}\",", height) + "\r\n";
            script += string.Format("caption: \"{0}\"", caption) + "\r\n";
            script += "})" + "\r\n";
			script += string.Format(".navGrid(\"{0}\",", pager);
			script += "{edit:false, add:false, search:true, del:false}, {}, {}, {}, {}, {});";

            #endregion

            return script;
        }
    }

    public class JQGridData
    {
        List<JQGridItem> _rows;
        int _totalRecords;
        int _pageIndex;
        int _pageSize;

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
		public JQGridData(IDictionary<string, List<string>> rows, int totalRecords, int pageIndex, int pageSize)
        {
            _rows = new List<JQGridItem>();
            _totalRecords = totalRecords;
            _pageIndex = pageIndex;
            _pageSize = pageSize;

            foreach (var row in rows)
                _rows.Add(new JQGridItem(row.Key, row.Value));
        }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "total")]
		public int total { get { return (int)Math.Ceiling((decimal)_totalRecords / (decimal)_pageSize); } }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "page")]
		public int page { get { return _pageIndex; } }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "records")]
		public int records { get { return _totalRecords; } }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "rows")]
		public IEnumerable rows { get { return _rows; } }
    }

    public class JQGridItem
    {
        #region Properties

        /// <summary>
        /// RowId de la fila.
        /// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "id")]
		public string id { get; protected set; }
        /// <summary>
        /// Fila del JQGrid.
        /// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "cell"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public IList<string> cell { get; protected set; }

        #endregion

        #region Active Attributes

        /// <summary>
        /// Contructor.
        /// </summary>
        public JQGridItem(string id, IList<string> values)
        {
            this.id = id;
			cell = values;
        }

        #endregion
    }

    public static class JQGridSearchHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
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
}
