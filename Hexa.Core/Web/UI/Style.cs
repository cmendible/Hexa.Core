// Credits should go to Egil Hansen.
// see: http://stackoverflow.com/questions/328763/how-to-take-control-of-style-sheets-in-asp-net-themes-with-the-styleplaceholder-a


using System.ComponentModel;
using System.Web.UI;
using System;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

//<%@ Register TagPrefix="cc2" Namespace="Hexa.Core.Web.UI.Controls" Assembly="Hexa.Core" %>
//<cc2:Styles runat="server">
//    <link rel="Stylesheet" type="text/css" href="%Theme/StyleSheet.css" media="all"/>
//    <link rel="Stylesheet" type="text/css" href="%Theme/Print.css" media="print"/>
//</cc2:Styles>
namespace Hexa.Core.Web.UI.Controls
{

    [DefaultProperty("ThemeVariableName")]
    [ToolboxData("<{0}:Styles runat=\"server\"></{0}:Styles>")]
    [Themeable(true)]
    public class Styles : PlaceHolder
    {

        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("%Theme")]
        [Localizable(false)]
        [Description("Name of the variable that should be replaced by the actual theme path")]
        public string ThemeVariableName
        {
            get
            {
                String s = (String)ViewState["ThemeVariableName"];
                return ((s == null) ? "%Theme" : s);
            }

            set
            {
                ViewState["ThemeVariableName"] = value;
            }
        }

        /// <summary>
        /// Fix controls before render
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (this.Visible)
            {
                // Hide any server side css 
                foreach (Control c in this.Page.Header.Controls)
                {
                    if (c is HtmlControl && ((HtmlControl)c).TagName.Equals("link",
                                StringComparison.OrdinalIgnoreCase))
                    {
                        c.Visible = false;
                    }
                }

                // Replace ThemeVariableName with actual theme path
                Regex reg = new Regex(ThemeVariableName,
                                      System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                foreach (Control c in this.Controls)
                {
                    if (c is LiteralControl)
                    {
                        LiteralControl l = (LiteralControl)c;
                        l.Text = reg.Replace(l.Text, this.ThemePath);
                    }
                }
            }
        }

        /// <summary>
        /// Get the theme path
        /// </summary>
        public string ThemePath
        {
            get
            {
                return String.Format("{0}/App_Themes/{1}",
                                     this.Page.Request.ApplicationPath,
                                     this.Page.Theme).Replace("//", "/");
            }
        }
    }
}
