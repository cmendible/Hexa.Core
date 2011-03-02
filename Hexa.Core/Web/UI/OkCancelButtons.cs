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
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Hexa.Core.Web.UI
{

    public class OkCancelButtonsEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OkCancelButtons : Hexa.Core.Web.UI.BaseUserControl
    {

        #region Properties

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "btn")]
        protected Button btnSave;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "btn")]
        protected Hexa.Core.Web.UI.Controls.ActiveButton btnBack;

        public event EventHandler<OkCancelButtonsEventArgs> PreSave;
        public event EventHandler<OkCancelButtonsEventArgs> PreBack;
        public event EventHandler DeletePostClick;
        public event EventHandler PostLoad;

        private bool _AskOnBack;
        private bool _AskOnExit = true;
        private bool _SetSaveMessage = true;

        private IPageWithOkCancelButtons _Container;

        /// <summary>
        /// Gets or sets a value indicating whether [ask on exit].
        /// </summary>
        /// <value><c>true</c> if [ask on exit]; otherwise, <c>false</c>.</value>
        public bool AskOnExit
        {
            get
            {
                return _AskOnExit;
            }
            set
            {
                _AskOnExit = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [ask on back].
        /// </summary>
        /// <value><c>true</c> if [ask on back]; otherwise, <c>false</c>.</value>
        public bool AskOnBack
        {
            get
            {
                return _AskOnBack;
            }
            set
            {
                _AskOnBack = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [set save message].
        /// </summary>
        /// <value><c>true</c> if [set save message]; otherwise, <c>false</c>.</value>
        public bool SetSaveMessage
        {
            get
            {
                return _SetSaveMessage;
            }
            set
            {
                _SetSaveMessage = value;
            }
        }

        /// <summary>
        /// Gets the button save.
        /// </summary>
        /// <value>The button save.</value>
        public Button ButtonSave
        {
            get
            {
                return btnSave;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        override protected void OnInit(System.EventArgs e)
        {
            base.OnInit(e);

            btnSave.Text = t("Save");
            btnBack.Text = t("Exit");

            btnSave.Click += btnSave_Click;
            btnBack.Click += btnBack_Click;
        }

        /// <summary>
        /// Sets the index of the buttons tab.
        /// </summary>
        private void SetButtonsTabIndex()
        {
            Control form = Page.Form;

            short ControlCount = System.Convert.ToInt16(form.Controls.Count - 1);

            int TabIndex = 1;

            if (btnSave.Visible)
            {
                btnSave.TabIndex = System.Convert.ToInt16(ControlCount + TabIndex);
                TabIndex++;
            }

            if (btnBack.Visible)
            {
                btnBack.TabIndex = System.Convert.ToInt16(ControlCount + TabIndex);
                TabIndex++;
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        override protected void OnLoad(System.EventArgs e)
        {
            if (!(Page is IPageWithOkCancelButtons))
            {
                btnBack.Visible = false;
                btnSave.Visible = false;
            }
            else
            {

                base.OnLoad(e);

                _Container = (IPageWithOkCancelButtons)Page;

                SetButtons(_Container.PageMode);

                if (_Container.PageMode == PageMode.ModeDelete)
                {
                    btnSave.Attributes.Add("onClick", "return confirm('Do you want to delete the record?')");
                    btnSave.CausesValidation = false;
                }

                if (_SetSaveMessage)
                {
                    if ((PageMode)System.Enum.Parse(typeof(PageMode), Request.QueryString["Mode"], true) == PageMode.ModeEdit && _Container.ObjectId > 0)
                    {
                        btnSave.Attributes.Add("OnClick", "var dodel=true; if (typeof(Page_ClientValidate) == 'function') { dodel=Page_ClientValidate(); }; if (dodel) { return confirm('Are you sure you want to modify the record?')}");
                    }
                    else if ((PageMode)System.Enum.Parse(typeof(PageMode), Request.QueryString["Mode"], true) == PageMode.ModeEdit && _Container.ObjectId == 0)
                    {
                        btnSave.Attributes.Add("OnClick", "var dodel=true; if (typeof(Page_ClientValidate) == 'function') { dodel=Page_ClientValidate(); }; if (dodel) { return confirm('Are you sure you want to save the record?')}");
                    }
                    else if ((PageMode)System.Enum.Parse(typeof(PageMode), Request.QueryString["Mode"], true) == PageMode.ModeNew || ((PageMode)System.Enum.Parse(typeof(PageMode), Request.QueryString["Mode"], true) == PageMode.ModeCopy))
                    {
                        btnSave.Attributes.Add("OnClick", "var dodel=true; if (typeof(Page_ClientValidate) == 'function') { dodel=Page_ClientValidate(); }; if (dodel) { return confirm('Are you sure you want to save the record?')}");
                    }
                }

                if (AskOnExit)
                {
                    RegisterBackButtonScript();
                }

                if (PostLoad != null)
                    PostLoad(this, new EventArgs());

                SetButtonsTabIndex();

                if (!(Page.IsClientScriptBlockRegistered("AskForExit")))
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    sb.Append("<script language='javascript'>").Append("\r\n");
                    sb.Append("function AskForExit()").Append("\r\n");
                    sb.Append("{ if (confirm('Are you sure you want to exit?') ==true)").Append("\r\n");
                    sb.Append("return true;").Append("\r\n");
                    sb.Append("else").Append("\r\n");
                    sb.Append("return false;").Append("\r\n");
                    sb.Append("}").Append("\r\n");
                    sb.Append("</script>").Append("\r\n");
                    Page.RegisterClientScriptBlock("AskForExit", sb.ToString());
                }
            }

        }

        #region  Buttons Actions

        /// <summary>
        /// Handles the Click event of the btnSave control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "btn"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "btn")]
        public void btnSave_Click(object sender, System.EventArgs e)
        {
            OkCancelButtonsEventArgs args = new OkCancelButtonsEventArgs();

            if (PreSave != null)
                PreSave(this, args);

            if (args.Cancel)
                return;

            _Container.SavePressed();
            SetButtons(_Container.PageMode);
        }

        /// <summary>
        /// Handles the Click event of the btnBack control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void btnBack_Click(object sender, System.EventArgs e)
        {
            OkCancelButtonsEventArgs args = new OkCancelButtonsEventArgs();

            if (PreBack != null)
                PreBack(this, args);

            if (args.Cancel)
                return;

            _Container.BackPressed();
        }

        #endregion

        #region  JavaScripts For Buttons

        /// <summary>
        /// Registers the back button script.
        /// </summary>
        private void RegisterBackButtonScript()
        {

            btnBack.CausesValidation = false;

            if (Page is IPageWithOkCancelButtons)
            {
                if (!_Container.IsDetail)
                {
                    btnBack.Attributes.Add("OnClick", "{ return confirm('" + t("") + "')}");
                }
                else
                {
                    btnBack.Attributes.Remove("OnClick");
                    btnBack.Text = t("Back");
                    if (_AskOnBack)
                    {
                        btnBack.Attributes.Add("OnClick", "{ return confirm('" + t("Ask on back") + "')}");
                    }
                }
            }
        }

        #endregion

        #region  ShowButtons

        /// <summary>
        /// Sets the buttons.
        /// </summary>
        public void SetButtons()
        {
            PageSetButtons(_Container.PageMode);
        }

        /// <summary>
        /// Sets the buttons.
        /// </summary>
        /// <param name="PageMode">The page mode.</param>
        public void SetButtons(PageMode pageMode)
        {
            PageSetButtons(pageMode);
        }

        /// <summary>
        /// Sets the buttons for the Page.
        /// </summary>
        /// <param name="PageMode">The page mode.</param>
        private void PageSetButtons(PageMode pageMode)
        {
            btnSave.Text = t("Save");

            switch (pageMode)
            {
                case PageMode.ModeNew:
                    btnSave.Enabled = true;
                    break;
                case PageMode.ModeEdit:
                case PageMode.ModeCopy:
                    btnSave.Enabled = true;
                    break;
                case PageMode.ModeView:
                    btnSave.Visible = false;
                    btnBack.Enabled = true;
                    break;
                case PageMode.ModeDelete:
                    btnSave.Enabled = true;
                    btnSave.Text = t("Accept");
                    btnBack.Enabled = true;
                    break;
            }

        }

        /// <summary>
        /// Shows the back button only.
        /// </summary>
        public void ShowBackButtonOnly()
        {
            btnBack.Enabled = true;
            btnSave.Visible = false;
        }

        /// <summary>
        /// Shows the page without save button.
        /// </summary>
        public void ShowWithoutSaveButton()
        {
            btnSave.Visible = false;
        }

        /// <summary>
        /// Shows the page with save button.
        /// </summary>
        public void ShowWithSaveButton()
        {
            btnSave.Visible = true;
        }

        /// <summary>
        /// Shows the close button only.
        /// </summary>
        public void ShowCloseButtonOnly()
        {
            btnBack.Enabled = true;
            btnSave.Visible = false;
        }

        /// <summary>
        /// Shows the save button only.
        /// </summary>
        public void ShowSaveButtonOnly()
        {
            btnBack.Enabled = false;
            btnSave.Visible = true;
        }

        /// <summary>
        /// Makes the cancel button able to close window.
        /// </summary>
        public void MakeCancel2CloseWindow()
        {
            btnBack.CausesValidation = false;
            btnBack.Attributes.Add("onClick", "window.close(); return false;");
        }

        #endregion

        #endregion

    }

}

