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

using System.Web;

namespace Hexa.Core.Web.UI.Services
{
    /// <summary>
    /// 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
    public sealed class NavigationService : INavigationService
    {
        /// <summary>
        /// Sets the URL in stack.
        /// </summary>
        public void SaveUrl()
        {
            SaveUrl(HttpContext.Current.Request.RawUrl);
        }

        /// <summary>
        /// Sets the URL in stack.
        /// </summary>
        /// <param name="url">The URL.</param>
        public void SaveUrl(string url)
        {
            System.Collections.Stack navigate2Url = null;

            // If Stack exist.
            if (HttpContext.Current.Session["Core"] != null)
                navigate2Url = (System.Collections.Stack)(HttpContext.Current.Session["Core"]);
            else
                navigate2Url = new System.Collections.Stack();

            // Do not allow dups in stack
            if (!navigate2Url.Contains(url))
            {
                navigate2Url.Push(url);
                HttpContext.Current.Session["Core"] = navigate2Url;
            }
        }

        /// <summary>
        /// Reads the URL from stack.
        /// </summary>
        /// <returns></returns>
        public string PeekUrl()
        {
            return (string)(((System.Collections.Stack)(HttpContext.Current.Session["Core"])).Peek());
        }

        /// <summary>
        /// Gets the URL from stack.
        /// </summary>
        /// <returns></returns>
        public string GetUrl()
        {
            return (string)(((System.Collections.Stack)(HttpContext.Current.Session["Core"])).Pop());
        }

        /// <summary>
        /// Go2s the URL from stack.
        /// </summary>
        public void Redirect2PreviousPage()
        {
            HttpContext.Current.Response.Redirect((string)(((System.Collections.Stack)(HttpContext.Current.Session["Core"])).Pop()));
        }

        /// <summary>
        /// Clears the stack.
        /// </summary>
        public void Clear()
        {
            HttpContext.Current.Session.Remove("Core");
        }

        /// <summary>
        /// Redirect2s the mode new.
        /// </summary>
        public void Redirect2ModeNew()
        {
            System.Text.StringBuilder Go2URL = new System.Text.StringBuilder();
            Go2URL.Append(HttpContext.Current.Request.RawUrl.Substring(0, HttpContext.Current.Request.RawUrl.IndexOf('?')));
            Go2URL.Append("?").Append("Mode").Append(string.Format("=0&ID={0}", System.Guid.Empty.ToString()));

            HttpContext.Current.Response.Redirect(Go2URL.ToString());
        }

    }
}
