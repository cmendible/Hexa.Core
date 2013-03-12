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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading;
    using System.Web.UI;

    using Controls;

    using GNU.Gettext;

    using Validation;

    /// <summary>
    /// Base Page
    /// </summary>
    public class BasePage : Page
    {
        #region Properties

        protected Control FirstControl2SetFocus
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Displays the exception.
        /// </summary>
        /// <param name="Exception">The exception.</param>
        public void DisplayException(Exception exception)
        {
            if ((exception) is ThreadAbortException)
            {
                return;
            }

            var sb = new StringBuilder();
            sb.Append(t(exception.Message));

            if (Debugger.IsAttached)
            {
                sb.Append("\r\n").Append("\r\n").Append("StackTrace: ");
                sb.Append("\r\n").Append(exception.StackTrace);
            }

            if (exception is ValidationException)
            {
                AddInvalidValidator(((ValidationException)exception).ValidationErrors.Select(e => e.Message));
            }
            else
                AddInvalidValidator(new[] {exception.Message});
        }

        /// <summary>
        /// Displays the exception.
        /// </summary>
        /// <param name="error">The expection message.</param>
        public void DisplayException(string error)
        {
            AddInvalidValidator(new[] {error});
        }

        protected virtual void AddInvalidValidator(IEnumerable<string> errors)
        {
            foreach (string error in errors)
            {
                Validators.Add(new InvalidValidator(error));
            }
        }

        /// <summary>
        /// Helper method to get the value from the ViewState
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design",
                         "CA1004:GenericMethodsShouldProvideTypeParameter")]
        protected T GetProperty<T>(string propertyName)
        {
            if (ViewState[propertyName] == null)
            {
                return default(T);
            }

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

        /// <summary>
        /// Translates the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly",
                         MessageId = "t"),
        SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t")]
        protected string t(string key)
        {
            Assembly assembly = null;

            if (!GetType().FullName.StartsWith("ASP", StringComparison.OrdinalIgnoreCase))
            {
                assembly = Assembly.GetCallingAssembly();
            }
            else
            {
                assembly = GetType().BaseType.Assembly;
            }

            return GettextHelper.t(key, assembly);
        }

        [SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t"),
        SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly",
                         MessageId = "t")]
        protected string t(string key, params object[] args)
        {
            return string.Format(CultureInfo.InvariantCulture, t(key), args);
        }

        #endregion Methods
    }
}