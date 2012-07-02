#region License

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
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hexa.Core.Web.UI.Controls
{

    public class GridView : System.Web.UI.WebControls.GridView, IPostBackDataHandler, IPostBackEventHandler
    {

        #region  Private Variables

        private bool _useDefaultCssClass = true;
        private bool _designMode = (HttpContext.Current == null);

        private bool _isScrollable = true;
        private System.Web.UI.WebControls.Unit _maxHeight = System.Web.UI.WebControls.Unit.Pixel(140);

        #endregion

        #region  Properties

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToBoolean(System.Object)")]
        public bool SelectFirstRow
        {
            get
                {
                    bool tempSelectFirstRow = false;
                    if (this.ViewState["SelectFirstRow"] != null)
                        {
                            return System.Convert.ToBoolean(this.ViewState["SelectFirstRow"]);
                        }
                    else
                        {
                            tempSelectFirstRow = true;
                        }
                    return tempSelectFirstRow;
                }
            set
                {
                    this.ViewState["SelectFirstRow"] = value;
                }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToBoolean(System.Object)")]
        public bool AllowRowSelection
        {
            get
                {
                    bool tempAllowRowSelection = false;
                    if (this.ViewState["AllowRowSelection"] != null)
                        {
                            return System.Convert.ToBoolean(this.ViewState["AllowRowSelection"]);
                        }
                    else
                        {
                            tempAllowRowSelection = false;
                        }
                    return tempAllowRowSelection;
                }
            set
                {
                    this.ViewState["AllowRowSelection"] = value;
                }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToBoolean(System.Object)")]
        public bool AllowClickRow
        {
            get
                {
                    bool tempAllowClickRow = false;
                    if (this.ViewState["AllowClickRow"] != null)
                        {
                            return System.Convert.ToBoolean(this.ViewState["AllowClickRow"]);
                        }
                    else
                        {
                            tempAllowClickRow = false;
                        }
                    return tempAllowClickRow;
                }
            set
                {
                    this.ViewState["AllowClickRow"] = value;
                }
        }

        public bool UseDefaultCssClass
        {
            get
                {
                    return _useDefaultCssClass;
                }
            set
                {
                    _useDefaultCssClass = value;
                }
        }

        public System.Web.UI.WebControls.Unit MaxHeight
        {
            get
                {
                    return _maxHeight;
                }
            set
                {
                    _maxHeight = value;
                }
        }

        public bool IsScrollable
        {
            get
                {
                    return _isScrollable;
                }
            set
                {
                    _isScrollable = value;
                }
        }

        #endregion

        #region  Methods

        public int GetSelectedItemIndex()
        {
            if (this.Rows.Count > 0)
                {
                    return this.SelectedIndex;
                }
            else
                {
                    return -1;
                }
        }

        public string GetSelectedItemUniqueId()
        {
            if (this.SelectedIndex >= 0)
                {
                    return this.SelectedRow.Cells[0].Text;
                }
            else
                {
                    return string.Empty;
                }
        }

        protected override void OnRowCreated(GridViewRowEventArgs e)
        {
            if (AllowRowSelection && e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes.Add("onclick", String.Format("MSDatagridSelectRow('{0}', {1});",  this.ClientID, e.Row.RowIndex));
                }
        }

        protected override void OnPreRender(System.EventArgs e)
        {
            base.OnPreRender(e);

            Page.RegisterRequiresPostBack(this);

            if (AllowRowSelection)
                {

                    if (SelectFirstRow && this.SelectedIndex < 0)
                        {
                            this.SelectedIndex = 0;
                        }

                    string hidxName = this.ClientID + "_idx";

                    this.Attributes.Add("hidxName", hidxName);

                    Page.ClientScript.RegisterHiddenField(hidxName, this.SelectedIndex.ToString());

                    if (!(Page.ClientScript.IsClientScriptBlockRegistered("SelectMSGridRow")))
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            sb.Append("<script language=\"javascript\">").Append("\r\n");
                            sb.Append("function MSDatagridSelectRow(grdID, row)").Append("\r\n");
                            sb.Append("{var grd = document.getElementById(grdID);").Append("\r\n");
                            sb.Append("var hdn = document.getElementsByName(grd.getAttribute('hidxName'));").Append("\r\n");
                            sb.Append("var actualidx = parseInt(row) + 1;").Append("\r\n");
                            sb.Append("if (actualidx > 0) {").Append("\r\n");
                            sb.Append("grd.rows.item(actualidx).setAttribute('ocolor', row.className);").Append("\r\n");
                            sb.Append("grd.rows.item(parseInt(hdn.item(0).value) + 1).className = grd.rows.item(parseInt(hdn.item(0).value) + 1).getAttribute('ocolor');").Append("\r\n");
                            sb.Append("hdn.item(0).value = row;").Append("\r\n");
                            sb.Append("grd.rows.item(actualidx).className = '").Append(this.SelectedRowStyle.CssClass).Append("';").Append("\r\n");
                            sb.Append("}}</script>");
                            Page.ClientScript.RegisterClientScriptBlock(typeof(string), "SelectMSGridRow", sb.ToString());
                        }
                }

        }

        public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
        {
            if (AllowRowSelection)
                {
                    string hidxName = this.ClientID + "_idx";

                    if (postCollection[hidxName] != null)
                        {
                            this.SelectedIndex = System.Convert.ToInt32(postCollection[hidxName]);
                        }
                }
            return false;
        }

        public void RaisePostDataChangedEvent()
        {
            //NOT IMPLEMENTED
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            base.SelectedIndexChanged += new EventHandler(GridView_SelectedIndexChanged);
        }

        private void GridView_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.SelectedIndex = base.SelectedIndex;
        }

        #endregion

    }
}