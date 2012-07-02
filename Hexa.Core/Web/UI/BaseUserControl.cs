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
using System.Globalization;

namespace Hexa.Core.Web.UI
{
/// <summary>
///
/// </summary>
    public class BaseUserControl : System.Web.UI.UserControl
    {
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
    }
}
