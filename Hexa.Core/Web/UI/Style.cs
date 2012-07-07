// Credits should go to Egil Hansen.
// see: http://stackoverflow.com/questions/328763/how-to-take-control-of-style-sheets-in-asp-net-themes-with-the-styleplaceholder-a
//<%@ Register TagPrefix="cc2" Namespace="Hexa.Core.Web.UI.Controls" Assembly="Hexa.Core" %>
//<cc2:Styles runat="server">
//    <link rel="Stylesheet" type="text/css" href="%Theme/StyleSheet.css" media="all"/>
//    <link rel="Stylesheet" type="text/css" href="%Theme/Print.css" media="print"/>
//</cc2:Styles>
namespace Hexa.Core.Web.UI.Controls
{
    using System;
    using System.ComponentModel;
    using System.Text.RegularExpressions;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    [DefaultProperty("ThemeVariableName")]
    [ToolboxData("<{0}:Styles runat=\"server\"></{0}:Styles>")]
    [Themeable(true)]
    public class Styles : PlaceHolder
    {
        #region Properties

        /// <summary>
        /// Get the theme path
        /// </summary>
        public string ThemePath
        {
            get
            {
                return string.Format("{0}/App_Themes/{1}",
                                     Page.Request.ApplicationPath,
                                     Page.Theme).Replace("//", "/");
            }
        }

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("%Theme")]
        [Localizable(false)]
        [Description("Name of the variable that should be replaced by the actual theme path")]
        public string ThemeVariableName
        {
            get
            {
                var s = (string)ViewState["ThemeVariableName"];
                return ((s == null) ? "%Theme" : s);
            }

            set
            {
                ViewState["ThemeVariableName"] = value;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Fix controls before render
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Visible)
            {
                // Hide any server side css
                foreach (Control c in Page.Header.Controls)
                {
                    if (c is HtmlControl && ((HtmlControl)c).TagName.Equals("link",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        c.Visible = false;
                    }
                }

                // Replace ThemeVariableName with actual theme path
                var reg = new Regex(ThemeVariableName,
                                    RegexOptions.IgnoreCase);

                foreach (Control c in Controls)
                {
                    if (c is LiteralControl)
                    {
                        var l = (LiteralControl)c;
                        l.Text = reg.Replace(l.Text, ThemePath);
                    }
                }
            }
        }

        #endregion Methods
    }
}
