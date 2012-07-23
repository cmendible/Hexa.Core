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
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Reflection;
    using System.Web.UI;

    using GNU.Gettext;

    /// <summary>
    ///
    /// </summary>
    public class BaseUserControl : UserControl
    {
        #region Methods

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