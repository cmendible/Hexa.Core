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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

using Hexa.Core;
using Hexa.Core.Domain;
using HexaControls = Hexa.Core.Web.UI.Controls;
using Hexa.Core.Web.UI.Services;

namespace Hexa.Core.Web.UI
{
    public class EditionPageEventArgs<T> : EventArgs
    {
        public bool Cancel { get; set; }
        public string Message { get; set; }
        public T Instance { get; set; }

        public EditionPageEventArgs(T instance)
        {
            Instance = instance;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Validators")]
    public delegate void OnPreRenderValidatorsEventHandler(object sender, ref Type entityTypeToValidate);

    /// <summary>
    /// Base Edtion Screen
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseEditionPage<T> : BasePage, IPageWithOkCancelButtons where T : RootEntity<T>
    {

        #region  Private Fields

        private const string _PageMode = "BaseEditionScreen.PageMode";
        private const string _PreviousPageMode = "BaseEditionScreen.PreviousPageMode";

        private HexaControls.WebValidationHelper _ValidationHelper;

        #endregion

        #region Events

        public event EventHandler<EditionPageEventArgs<T>> PreSave;
        public event EventHandler<EditionPageEventArgs<T>> PostSave;
        public event EventHandler<EditionPageEventArgs<T>> PostDelete;

        public event EventHandler<EditionPageEventArgs<T>> AddDetail;
        public event EventHandler<EditionPageEventArgs<T>> DeleteDetail;

        public event EventHandler<EditionPageEventArgs<T>> PreCancel;
        public event EventHandler<EventArgs> ShowFormStarting;
        public event EventHandler<EventArgs> ShowFormCompleted;

        public event EventHandler<EditionPageEventArgs<T>> InvalidConstraintAction;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Validators")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event OnPreRenderValidatorsEventHandler OnPreRenderValidators;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the BLLID.
        /// </summary>
        /// <value>The BLLID.</value>
        public int ObjectId { get; set; }

        /// <summary>
        /// Gets or sets the page mode.
        /// </summary>
        /// <value>The page mode.</value>
        public PageMode PageMode
        {
            get
            {
                return (PageMode)ViewState[_PageMode];
            }
            set
            {
                ViewState[_PageMode] = value;
            }
        }

        /// <summary>
        /// Gets or sets the previous page mode.
        /// </summary>
        /// <value>The previous page mode.</value>
        protected PageMode PreviousPageMode
        {
            get
            {
                if (ViewState[_PreviousPageMode] != null)
                    return (PageMode)ViewState[_PreviousPageMode];
                else
                    return PageMode.ModeNew;
            }
            set
            {
                ViewState[_PreviousPageMode] = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is a detail page.
        /// </summary>
        /// <value><c>true</c> if this instance is detail; otherwise, <c>false</c>.</value>
        public virtual bool IsDetail { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the page is [not updateable].
        /// </summary>
        /// <value><c>true</c> if [not updateable]; otherwise, <c>false</c>.</value>
        public bool NotUpdateable { get; set; }

        /// <summary>
        /// Gets the type of the BLL.
        /// </summary>
        /// <value>The type of the BLL.</value>
        protected Type EntityType
        {
            get
            {
                return typeof(T);
            }
        }

        #endregion

        #region Service Dependencies

        public IEntitiesService EntitiesService { get; protected set; }

        public INavigationService NavigationService { get; protected set; }

        #endregion

        #region  Methods

        #region  ShowForm

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event to initialize the page.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.</param>
        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _ValidationHelper = new HexaControls.WebValidationHelper(this);
            _ValidationHelper.ValidatorDisplay = ValidatorDisplay.Static;

            Type entityType = typeof(T);

            if (OnPreRenderValidators != null)
                OnPreRenderValidators(this, ref entityType);

            _ValidationHelper.CreateValidators(true, entityType);

            EntitiesService = ServiceLocator.GetInstance<IEntitiesService>();
            NavigationService = ServiceLocator.GetInstance<INavigationService>();
        }

        /// <summary>
        /// Handles the Load event of the Page control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "pageTitle"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            T bll = default(T);

            try
            {
                PageMode = (PageMode)System.Enum.Parse(typeof(PageMode), Request.QueryString["Mode"].ToString(), true);
            }
            catch (Exception ex)
            {
                LastChanceBeforeModeException();
            }

            if (PageMode == PageMode.ModeEdit && Request.QueryString["Id"] == null)
                throw new InvalidOperationException("Entering ModeEdit requires parameter 'ID'");

            if (PageMode == PageMode.ModeView && Request.QueryString["Id"] == null)
                throw new InvalidOperationException("Entering ModeView requires parameter 'ID'");

            if (Request.QueryString["Id"] != null && Request.QueryString["Id"].Length > 0)
                ObjectId = System.Int32.Parse(Request.QueryString["Id"], CultureInfo.InvariantCulture);

            SetFocus2Control();

            string caption = PageModeText;
            string captionMode = string.Empty;

            if (!Page.IsPostBack)
            {
                if (PageModeText != null)
                {
                    switch (PageMode)
                    {
                        case PageMode.ModeDelete:
                            captionMode = t("Delete");
                            break;
                        case PageMode.ModeEdit:
                            if (ObjectId >= 0)
                                captionMode = t("Edit");
                            else
                            {
                                if (PreviousPageMode != PageMode.ModeCopy)
                                    captionMode = t("New");
                                else
                                    captionMode = t("Copy");
                            }
                            break;
                        case PageMode.ModeNew:
                            captionMode = t("New");
                            break;
                        case PageMode.ModeView:
                            captionMode = t("Viw");
                            break;
                        case PageMode.ModeCopy:
                            captionMode = t("Copy");
                            break;
                    }

                    //BaseEditionMaster master = (BaseEditionMaster)Master;

                    string pageTitle = string.Format(CultureInfo.InvariantCulture, "{0} - {1}", captionMode, caption);
                    //master.SetPageModeText(pageTitle);

                    PreviousPageMode = PageMode;
                }

                if (ShowFormStarting != null)
                    ShowFormStarting(this, new EventArgs());

                try
                {
                    ShowForm();
                    if (ShowFormCompleted != null)
                        ShowFormCompleted(this, new EventArgs());
                }
                catch (Exception ex)
                {
                    DisplayException(ex);
                }

            }

            bll = EntitiesService.GetObjectFromSession<T>();

            if (!IsDetail && (bll != null && !bll.Equals(default(T)) && !bll.IsTransient() && bll.IsValid())) //&& !bll.IsEditable
            {
                bool m_SetControls2ReadOnly = true;

                if (PageMode == PageMode.ModeDelete || PageMode == PageMode.ModeCopy)
                    m_SetControls2ReadOnly = false;

                EntityIsNotUpdateable(m_SetControls2ReadOnly);

                //if (!bll.IsEditable && PageMode != PageMode.ModeCopy)
                //    m_SetControls2ReadOnly = true;

                if (m_SetControls2ReadOnly)
                {
                    MakePageReadOnly();
                    PageMode = PageMode.ModeView;
                }
            }
        }

        /// <summary>
        /// Shows the form.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        protected virtual void ShowForm()
        {
            T bll = default(T);

            try
            {
                Guid uniqueId = Guid.Parse(Request.QueryString["ID"]);

                switch (PageMode)
                {
                    case PageMode.ModeNew: //ModeNew
                        if (IsDetail)
                            bll = EntitiesService.GetObjectFromSession<T>();
                        else
                            bll = CreateEntity();

                        EntitiesService.SetObjectInSession<T>(bll);
                        InitializeForm(bll);

                        break;
                    case PageMode.ModeEdit:
                    case PageMode.ModeView:
                    case PageMode.ModeCopy: //ModeEdit, ModeView, ModeCopy

                        bll = EntitiesService.GetObjectFromSession<T>();

                        if (!IsDetail && (bll == null || (!bll.UniqueId.Equals(uniqueId) && uniqueId != Guid.Empty)))
                        {
                            bll = FetchEntity(uniqueId);

                            if (PageMode == PageMode.ModeCopy) //Mode Copy Special Section
                                bll = (T)ObjectCloner.Clone<T>(bll);

                            EntitiesService.SetObjectInSession<T>(bll);
                        }

                        PopulateForm(bll);

                        if (PageMode == PageMode.ModeView)
                            MakeControlsReadOnly(); //Mode View Special Section

                        break;
                    case PageMode.ModeDelete:
                        bll = EntitiesService.GetObjectFromSession<T>();

                        if (!IsDetail && (bll == null || (!bll.UniqueId.Equals(uniqueId) && uniqueId != Guid.Empty)))
                        {
                            bll = FetchEntity(uniqueId);

                            EntitiesService.SetObjectInSession<T>(bll);
                        }

                        PopulateForm(bll);

                        MakeControlsReadOnly();

                        break;
                    default:
                        throw new InvalidOperationException("Invalid value for parameter 'mode'");
                }

            }
            catch (Exception ex)
            {
                MakeControlsReadOnly();
                PageMode = PageMode.ModeView;
                EntitiesService.SetObjectInSession<T>(bll);
                DisplayException(ex);
            }
        }

        /// <summary>
        /// Gets the principal.
        /// </summary>
        /// <returns></returns>
        public Security.CorePrincipal Principal
        {
            get { return System.Threading.Thread.CurrentPrincipal as Security.CorePrincipal; }
        }

        /// <summary>
        /// Runed when the BLL is not updateable.
        /// </summary>
        /// <param name="SetControls2ReadOnly">if set to <c>true</c> [set controls2 read only].</param>
        protected virtual void EntityIsNotUpdateable(bool setControlsToReadOnly)
        {

        }

        /// <summary>
        /// Makes the page read only.
        /// </summary>
        private void MakePageReadOnly()
        {
            MakeControlsReadOnly();
            //((BaseEditionMaster)Master).GetOkCancelButtons().SetButtons(PageMode.ModeView);
            NotUpdateable = true;
        }

        /// <summary>
        /// Sets the focus to A control.
        /// </summary>
        protected void SetFocus2Control()
        {
            if (FirstControl2SetFocus != null)
                SetFocus(FirstControl2SetFocus);
        }

        /// <summary>
        /// Makes the controls read only.
        /// </summary>
        protected void MakeControlsReadOnly()
        {
            Control form = Form;
            Control control = null;
            Control tmpcontrol = null;
            short index = 0;
            short i = 0;
            short nCount = System.Convert.ToInt16(form.Controls.Count - 1);

            try
            {
                for (index = 0; index <= nCount; index++)
                {
                    control = form.Controls[index];
                    if (control.HasControls())
                    {
                        ControlCollection collControl = control.Controls;
                        for (i = 0; i < collControl.Count; i++)
                        {
                            tmpcontrol = collControl[i];
                            MakeControlReadOnly(tmpcontrol);
                        }
                    }
                    else
                    {
                        DisableControl(control);
                    }
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }

        }

        /// <summary>
        /// Makes the control read only.
        /// </summary>
        /// <param name="control">The control.</param>
        protected void MakeControlReadOnly(Control control)
        {
            short index = 0;
            Control tmpcontrol = null;

            try
            {
                if (control.HasControls())
                {
                    ControlCollection collControl = control.Controls;
                    for (index = 0; index < collControl.Count; index++)
                    {

                        tmpcontrol = collControl[index];

                        if (tmpcontrol.HasControls())
                        {
                            MakeControlReadOnly(tmpcontrol);
                        }
                        else
                        {
                            DisableControl(tmpcontrol);
                        }
                    }
                }
                else
                {
                    DisableControl(control);
                }

            }
            catch (InvalidOperationException)
            {
                throw;
            }
        }

        /// <summary>
        /// Disables the control.
        /// </summary>
        /// <param name="control">The control.</param>
        private static void DisableControl(Control control)
        {
            if ((control) is Hexa.Core.Web.UI.Controls.ActiveButton || (control) is Hexa.Core.Web.UI.Controls.ActiveImageButton)
                return;

            if (typeof(TextBox).IsAssignableFrom(control.GetType()))
            {
                ((TextBox)control).ReadOnly = true;
            }
            else if (typeof(CheckBox).IsAssignableFrom(control.GetType()))
            {
                ((CheckBox)control).Enabled = false;
            }
            else if (typeof(CheckBoxList).IsAssignableFrom(control.GetType()))
            {
                ((CheckBoxList)control).Enabled = false;
            }
            else if (typeof(DropDownList).IsAssignableFrom(control.GetType()))
            {
                ((DropDownList)control).Enabled = false;
            }
            else if (typeof(RadioButton).IsAssignableFrom(control.GetType()))
            {
                ((RadioButton)control).Enabled = false;
            }
            else if (typeof(RadioButtonList).IsAssignableFrom(control.GetType()))
            {
                ((RadioButtonList)control).Enabled = false;
            }
            else if (typeof(Button).IsAssignableFrom(control.GetType()))
            {
                ((Button)control).Enabled = false;
            }
            else if (typeof(System.Web.UI.WebControls.Panel).IsAssignableFrom(control.GetType()))
            {
                ((System.Web.UI.WebControls.Panel)control).Enabled = false;
            }
            else if (typeof(ImageButton).IsAssignableFrom(control.GetType()))
            {
                ((ImageButton)control).Enabled = false;
            }
        }

        #endregion

        #region  Protected Overridable Sub with Nothing

        protected abstract string PageModeText { get; }

        protected abstract T CreateEntity();

        protected abstract T FetchEntity(Guid uniqueId);

        protected abstract T SaveEntity(T entity);

        protected abstract void DeleteEntity(T entity);

        /// <summary>
        /// Initializes the form.
        /// </summary>
        /// <param name="BLL">The BLL.</param>
        protected virtual void InitializeForm(T entity)
        {
        }

        /// <summary>
        /// Populates the form.
        /// </summary>
        /// <param name="BLL">The BLL.</param>
        protected virtual void PopulateForm(T entity)
        {
        }

        /// <summary>
        /// Populates the BLL.
        /// </summary>
        /// <param name="BLL">The BLL.</param>
        protected virtual void PopulateEntity(T entity)
        {
        }

        /// <summary>
        /// Fills the BLL.
        /// </summary>
        /// <param name="BLL">The BLL.</param>
        public void FillObject(object entity)
        {
            PopulateEntity((T)entity);
        }

        /// <summary>
        /// Lasts the chance before mode exception.
        /// </summary>
        protected virtual void LastChanceBeforeModeException()
        {
            throw new InvalidOperationException("LastChanceBeforeModeException has not been Overriden or Mode not set");
        }

        #endregion

        #region  Navigation (IBaseScreenWithOkCancelButtons)

        /// <summary>
        /// Back  pressed.
        /// </summary>
        public void BackPressed()
        {
            T bll = EntitiesService.GetObjectFromSession<T>();

            EditionPageEventArgs<T> args = new EditionPageEventArgs<T>(bll);

            if (PreCancel != null)
                PreCancel(this, args);

            if (!args.Cancel)
            {
                if (IsDetail)
                {
                    System.Collections.Stack Navigate2Url = new System.Collections.Stack();
                    Navigate2Url = (System.Collections.Stack)(Session["Core"]);
                    string url = (string)(Navigate2Url.Pop());

                    if (url.IndexOf("Mode=0", StringComparison.OrdinalIgnoreCase) != -1)
                        url = url.Replace("Mode=0", "Mode=1");

                    if (url.IndexOf("BLL", StringComparison.OrdinalIgnoreCase) == -1)
                        url = url + "&BLL=" + "BL";

                    Response.Redirect(url, true);
                }
                else
                {
                    EntitiesService.SetObjectInSession<T>(default(T));
                    NavigationService.Redirect2PreviousPage();
                }
            }
        }

        /// <summary>
        /// Delete pressed.
        /// </summary>
        /// <param name="BLL">The BLL.</param>
        public void DeletePressed(T entity)
        {
            EditionPageEventArgs<T> args = new EditionPageEventArgs<T>(entity);

            try
            {
                if (IsDetail)
                {
                    if (DeleteDetail != null)
                        DeleteDetail(this, args);

                    if (!string.IsNullOrEmpty(args.Message))
                        throw new EditionException(args.Message);
                    else
                    {
                        if (PostDelete != null)
                            PostDelete(this, args);

                        EntitiesService.SetObjectInSession<T>(entity);

                        NavigationService.Redirect2PreviousPage();
                    }
                }
                else
                {
                    DeleteEntity(entity);

                    if (PostDelete != null)
                        PostDelete(this, args);

                    EntitiesService.SetObjectInSession<T>(default(T));

                    if (PreCancel != null)
                        PreCancel(this, args);

                    if (!args.Cancel)
                        NavigationService.Redirect2PreviousPage();
                }
            }
            catch (CoreException ex)
            {
                DisplayException(ex);
            }
        }

        /// <summary>
        /// Saves pressed.
        /// </summary>
        public void SavePressed()
        {
            EditionPageEventArgs<T> args;
            T BLL = default(T);

            try
            {
                BLL = EntitiesService.GetObjectFromSession<T>();
                args = new EditionPageEventArgs<T>(BLL);
                if (PreSave != null)
                    PreSave(this, args);

                if (!string.IsNullOrEmpty(args.Message))
                {
                    throw new EditionException(args.Message);
                }

                PopulateEntity(BLL);

                if (!IsDetail)
                {
                    if (PageMode == PageMode.ModeDelete)
                    {
                        DeletePressed(BLL);
                        return;
                    }

                    BLL.AssertValidation();

                    BLL = SaveEntity(BLL);

                    args = new EditionPageEventArgs<T>(BLL);

                    if (PostSave != null)
                        PostSave(this, args);

                    EntitiesService.SetObjectInSession<T>(default(T));

                    if (ObjectId == -1)
                        NavigationService.Redirect2ModeNew();
                    else
                    {
                        if (PreCancel != null)
                            PreCancel(this, args);

                        if (!args.Cancel)
                            NavigationService.Redirect2PreviousPage();
                    }
                }
                else
                {
                    if (PageMode == PageMode.ModeDelete)
                    {
                        DeletePressed(BLL);
                        return;
                    }

                    if (PageMode == PageMode.ModeNew)
                    {
                        if (AddDetail != null)
                            AddDetail(this, args);

                        if (!string.IsNullOrEmpty(args.Message))
                        {
                            throw new EditionException(args.Message);
                        }
                        else
                        {
                            if (PostSave != null)
                                PostSave(this, args);

                            EntitiesService.SetObjectInSession<T>(BLL);

                            Response.Redirect(Request.RawUrl, true);
                        }
                    }
                    else
                    {
                        if (PostSave != null)
                            PostSave(this, args);

                        NavigationService.Redirect2PreviousPage();
                    }
                }
            }
            catch (CoreException ex)
            {
                // TODO: INVALID CONSTRAINT STUFF
                //if (ex.GetType() == typeof(Hexa.Core.DataLayer.ConstraintException))
                //{
                //    Hexa.Core.DataLayer.ConstraintException constraintException = ex as Hexa.Core.DataLayer.ConstraintException;

                //    args = new BaseEditionPageEventArgs<T>(BLL);
                //    args.Message = constraintException.Message;

                //    if (InvalidConstraintAction != null)
                //        InvalidConstraintAction(this, args);

                //    DisplayException(constraintException);
                //}
                //else
                DisplayException(ex);
            }
        }

        /// <summary>
        /// Shows the back button only.
        /// </summary>
        protected void ShowBackButtonOnly()
        {
            //((BaseEditionMaster)Master).GetOkCancelButtons().ShowBackButtonOnly();
        }

        /// <summary>
        /// Shows the page without save button.
        /// </summary>
        protected void ShowWithoutSaveButton()
        {
            //((BaseEditionMaster)Master).GetOkCancelButtons().ShowWithoutSaveButton();
        }

        #endregion

        public object ObjectInSession
        {
            get { return EntitiesService.GetObjectFromSession<T>(); }
            set { EntitiesService.SetObjectInSession<T>((T)value); }
        }

        public void SaveUrl()
        {
            NavigationService.SaveUrl();
        }

        #endregion

    }

}

