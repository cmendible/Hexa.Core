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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using Hexa.Core.Web.UI.Controls;

namespace Hexa.Core.Web.UI
{

    /// <summary>
    /// Base Page
    /// </summary>
    public class BasePage : System.Web.UI.Page
    {
        protected Control FirstControl2SetFocus
        {
            get;
            set;
        }

        #region ViewState Helper

        /// <summary>
        /// Helper method to get the value from the ViewState
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        protected T GetProperty<T>(string propertyName)
        {
            if (ViewState[propertyName] == null)
                return default(T);

            return (T)ViewState[propertyName];
        }

        /// <summary>
        /// Helper method to set the value to the ViewState
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        protected void SetProperty<T>(string propertyName, T value)
        {
            ViewState[propertyName] = value;
        }

        #endregion

        #region  Methods

        #region  Globalization

        /// <summary>
        /// Translates the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "t"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t")]
        protected string t(string key)
        {
            System.Reflection.Assembly assembly = null;

            if (!this.GetType().FullName.StartsWith("ASP", StringComparison.OrdinalIgnoreCase))
                assembly = System.Reflection.Assembly.GetCallingAssembly();
            else
                assembly = this.GetType().BaseType.Assembly;

            return GNU.Gettext.GettextHelper.t(key, assembly);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "t")]
        protected String t(String key, params object[] args)
        {
            return String.Format(CultureInfo.InvariantCulture, t(key), args);
        }

        #endregion

        #region Exceptions

        /// <summary>
        /// Displays the exception.
        /// </summary>
        /// <param name="Exception">The exception.</param>
        public void DisplayException(Exception exception)
        {
            if ((exception) is System.Threading.ThreadAbortException)
                return;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(t(exception.Message));

            if (Debugger.IsAttached)
                {
                    sb.Append("\r\n").Append("\r\n").Append("StackTrace: ");
                    sb.Append("\r\n").Append(exception.StackTrace);
                }

            if (exception is Validation.ValidationException)
                AddInvalidValidator(((Validation.ValidationException)exception).ValidationErrors.Select(e => e.Message));
            else
                AddInvalidValidator(new string[] { exception.Message });
        }

        /// <summary>
        /// Displays the exception.
        /// </summary>
        /// <param name="error">The expection message.</param>
        public void DisplayException(string error)
        {
            AddInvalidValidator(new string[] { error });
        }

        protected virtual void AddInvalidValidator(IEnumerable<string> errors)
        {
            foreach (string error in errors)
                this.Validators.Add(new InvalidValidator(error));
        }

        #endregion

        #endregion

    }

}

