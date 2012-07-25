#region Header

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

#endregion Header

namespace Hexa.Core.Web.UI.Controls
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public class ExtendedRegularExpressionValidator : RegularExpressionValidator
    {
        #region Properties

        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings"),
        UrlProperty]
        public virtual string ImageUrl
        {
            get
            {
                string imageUrl = string.Empty;
                object o = ViewState["ImageUrl"];
                if (o != null)
                {
                    imageUrl = ViewState["ImageUrl"].ToString();
                }

                return imageUrl;
            }
            set
            {
                ViewState["ImageUrl"] = value;
            }
        }

        #endregion Properties

        #region Methods

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!string.IsNullOrEmpty(ImageUrl))
            {
                var img = new Image();
                img.ID = "i" + ID;
                img.ToolTip = ErrorMessage;
                img.ImageUrl = ImageUrl;
                Controls.Add(img);
            }
        }

        #endregion Methods
    }
}