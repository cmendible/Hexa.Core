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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hexa.Core.Web.UI.Controls
{

	public class DataGrid : System.Web.UI.WebControls.DataGrid, IPostBackDataHandler, IPostBackEventHandler
	{

		#region  Private Variables 

		private bool m_UseDefaultCssClass = true;
		private bool m_DesignMode = (HttpContext.Current == null);

		private bool m_IsScrollable = true;
		private System.Web.UI.WebControls.Unit m_MaxHeight = System.Web.UI.WebControls.Unit.Pixel(140);

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
				return m_UseDefaultCssClass;
			}
			set
			{
				m_UseDefaultCssClass = value;
			}
		}

		public System.Web.UI.WebControls.Unit MaxHeight
		{
			get
			{
				return m_MaxHeight;
			}
			set
			{
				m_MaxHeight = value;
			}
		}

		public bool IsScrollable
		{
			get
			{
				return m_IsScrollable;
			}
			set
			{
				m_IsScrollable = value;
			}
		}

		#endregion

		#region  Events 

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public delegate void CustomInitializeLayoutEventHandler();
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
		public event CustomInitializeLayoutEventHandler CustomInitializeLayout;

		#endregion

		#region  Methods 

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
		public int GetSelectedItemIndex()
		{
			if (this.Items.Count > 0)
			{
				return this.SelectedIndex;
			}
			else
			{
				return -1;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToInt32(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ID")]
		public int GetSelectedItemID()
			{
				if (this.SelectedIndex >= 0)
				{
					return System.Convert.ToInt32(this.SelectedItem.Cells[0].Text);
				}
				else
				{
					return -1;
				}
			}

		public DataGridColumn GetColumnByName(string colName)
		{
			int col = 0;
				int colcounter = 0;
			colcounter = this.Columns.Count - 1;
			for (col = 0; col <= colcounter; col++)
			{
				if (((BoundColumn)(this.Columns[col])).DataField == colName)
				{
					return this.Columns[col];
				}
			}
			return null;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
		protected override DataGridItem CreateItem(int itemIndex, int dataSourceIndex, ListItemType itemType)
		{

			DataGridItem item = new DataGridItem(itemIndex, dataSourceIndex, itemType);

			if (AllowRowSelection && (itemType != ListItemType.Header & itemType != ListItemType.Footer & itemType != ListItemType.Pager))
			{
				item.Attributes["onclick"] = "MSDatagridSelectRow('" + this.ClientID + "', this);";
			}

			return item;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
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

				if (! (Page.ClientScript.IsClientScriptBlockRegistered("SelectMSGridRow")))
				{
					System.Text.StringBuilder sb = new System.Text.StringBuilder();
					sb.Append("<script language=\"javascript\">").Append("\r\n");
					sb.Append("function MSDatagridSelectRow(grdID, row)").Append("\r\n");
					sb.Append("{var grd = document.getElementById(grdID);").Append("\r\n");
					sb.Append("var hdn = document.getElementsByName(grd.getAttribute('hidxName'));").Append("\r\n");
					sb.Append("var actualidx = parseInt(hdn.item(0).value) + 1;").Append("\r\n");
					sb.Append("if (actualidx > 0) {").Append("\r\n");
					sb.Append("grd.rows.item(actualidx).className = grd.rows.item(actualidx).getAttribute('ocolor');").Append("\r\n");
					sb.Append("hdn.item(0).value = row.rowIndex - 1 ;").Append("\r\n");
					sb.Append("row.setAttribute('ocolor', row.className);").Append("\r\n");
					sb.Append("row.className = '").Append(this.SelectedItemStyle.CssClass).Append("';").Append("\r\n");
					sb.Append("}}</script>");
					Page.ClientScript.RegisterClientScriptBlock(typeof(string), "SelectMSGridRow", sb.ToString());
				}
				if (! (Page.ClientScript.IsClientScriptBlockRegistered("DeselectAll")))
				{
					System.Text.StringBuilder sb = new System.Text.StringBuilder();
					sb.Append("<script language=\"javascript\">").Append("\r\n");
					sb.Append("function MSDatagridDeselectAll(grdID)").Append("\r\n");
					sb.Append("{var grd = document.getElementById(grdID);").Append("\r\n");
					sb.Append("var hdn = document.getElementsByName(grd.getAttribute('hidxName'));").Append("\r\n");
					sb.Append("var actualidx = parseInt(hdn.item(0).value) + 1;").Append("\r\n");
					sb.Append("if (actualidx > 0) {").Append("\r\n");
					sb.Append("grd.rows.item(actualidx).className = grd.rows.item(actualidx).getAttribute('ocolor');}").Append("\r\n");
					sb.Append("hdn.item(0).value = - 1 ;").Append("\r\n");
					sb.Append("}</script>");
					Page.ClientScript.RegisterClientScriptBlock(typeof(string), "DeselectAll", sb.ToString());
				}

			}

		}

		private void ApplyDefaultStyle()
		{

			if (m_UseDefaultCssClass)
			{

				if (! m_DesignMode)
				{
					this.AutoGenerateColumns = false;
				}

				this.UseAccessibleHeader = true;

				if (this.AllowPaging | this.AllowCustomPaging)
				{
					this.PagerStyle.Mode = PagerMode.NextPrev;
					this.PagerStyle.NextPageText = "next";
					this.PagerStyle.PrevPageText = "prev";
				}
			}

			if (CustomInitializeLayout != null)
				CustomInitializeLayout();

		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Convert.ToInt32(System.String)")]
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

		public override void DataBind()
		{
			ApplyDefaultStyle();
			base.DataBind();
		}

		private void MSDataGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.CurrentPageIndex = e.NewPageIndex;
		}

		protected override void Render(System.Web.UI.HtmlTextWriter writer)
		{
			LiteralControl l1 = null;
			LiteralControl l2 = null;

			if (m_IsScrollable)
			{
				l1 = new LiteralControl("<div id='scroller' style='OVERFLOW: auto; WIDTH: 100%; HEIGHT: " 
					+ System.Web.UI.WebControls.Unit.Pixel(Convert.ToInt32(m_MaxHeight.Value)).ToString() + "'>");

				l1.RenderControl(writer);
			}

			base.Render(writer);

			if (m_IsScrollable)
			{
				l2 = new LiteralControl("</div>");
				l2.RenderControl(writer);
			}
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			base.PageIndexChanged += MSDataGrid_PageIndexChanged;
			base.SelectedIndexChanged += MSDataGrid_SelectedIndexChanged;
		}

		#region row-clickable

		//Event which is raised when a row is clicked
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
		public delegate void ItemClickedEventHandler(object sender, EventArgs e);
		public event ItemClickedEventHandler ItemClicked;

		//Method to raise the event
		protected virtual void OnItemClicked(object sender, EventArgs e)
		{
			if (ItemClicked != null)
				ItemClicked(sender, e);
		}

		//When items are created, add the script to cause a postback
		//with item index as argument
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
		protected override void CreateControlHierarchy(bool useDataSource)
		{
			base.CreateControlHierarchy(useDataSource);

			if (AllowClickRow)
			{
				int i = 0;
				for (i = 0; i < Items.Count; i++)
				{
					this.Items[i].Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(this, i.ToString()));
				}
			}
		}

		//When a row is clicked, this is called and event will be raised
		//with the specific item as sender of the event
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.Parse(System.String)")]
		public void RaisePostBackEvent(string eventArgument)
		{
			if (eventArgument != null)
			{
				int i = Int32.Parse(eventArgument);
				OnItemClicked(this.Items[i], null);
			}
		}

		#endregion

		#endregion

		private void MSDataGrid_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.SelectedIndex = base.SelectedIndex;
		}

	}
}