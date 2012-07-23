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

namespace Hexa.Core.Web.UI
{
    using System;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    public static class PageExtensions
    {
        #region Methods

        /// <summary>
        /// Starts a Download based on the specified byes and mime type.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="id">The id.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        public static void Download(this Page page, byte[] data, string id, string mimeType)
        {
            page.Response.Clear();
            page.Response.Buffer = true;

            page.Response.ContentType = mimeType;
            page.Response.AppendHeader("content-disposition", "attachment; filename=" + id);

            page.Response.BinaryWrite(data);
            page.Response.End();
        }

        /// <summary>
        /// Gets the post back control.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public static Control GetPostBackControl(this Page page)
        {
            Control control = null;
            string ctrlname = page.Request.Params["__EVENTTARGET"];
            if (!string.IsNullOrEmpty(ctrlname))
            {
                control = page.FindControl(ctrlname);
            }
            else
            {
                // if __EVENTTARGET is null, control is a button type and need to
                // iterate over the form collection to find it
                string ctrlStr = string.Empty;
                Control c = null;

                foreach (string ctl in page.Request.Form)
                {
                    // handle ImageButton controls
                    if (ctl.EndsWith(".x", StringComparison.Ordinal) |
                        ctl.EndsWith(".y", StringComparison.Ordinal))
                    {
                        ctrlStr = ctl.Substring(0, ctl.Length - 2);
                        c = page.FindControl(ctrlStr);
                    }
                    else
                    {
                        c = page.FindControl(ctl);
                    }
                    if (c is Button | c is ImageButton)
                    {
                        control = c;
                        break;
                    }
                }
            }

            return control;
        }

        /// <summary>
        /// Gets the post back control ID.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns></returns>
        public static string GetPostBackControlId(this Page page)
        {
            Control control = GetPostBackControl(page);
            if (control != null)
            {
                return control.ID;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Determines whether [the specified page] [is in async post back].
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="scriptManagerId">The script manager id.</param>
        /// <returns>
        /// 	<c>true</c> if [the specified page] [is in async post back]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInAsyncPostBack(this Page page, string scriptManagerId)
        {
            ScriptManager scriptManager;

            if (page.Master != null)
            {
                scriptManager = page.Master.FindControl(scriptManagerId) as ScriptManager;
            }
            else
            {
                scriptManager = page.FindControl(scriptManagerId) as ScriptManager;
            }

            if (scriptManager != null && scriptManager.IsInAsyncPostBack)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether [the specified page] [is in async post back].
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>
        /// 	<c>true</c> if [the specified page] [is in async post back]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInAsyncPostBack(this Page page)
        {
            return IsInAsyncPostBack(page, "ScriptManager");
        }

        #endregion Methods
    }
}