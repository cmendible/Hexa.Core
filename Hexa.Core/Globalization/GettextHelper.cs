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

namespace GNU.Gettext
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// Class used to call Gettext.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly",
                     MessageId = "Gettext")]
    public sealed class GettextHelper
    {
        #region Fields

        /// <summary>
        /// Gettext resource manager.
        /// </summary>
        private static readonly Dictionary<string, GettextResourceManager> _ResourceManager = 
            new Dictionary<string, GettextResourceManager>();

        #endregion Fields

        #region Constructors

        private GettextHelper()
        {
        }

        #endregion Constructors

        #region Methods

        public static void ForceInvariantCulture()
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Translates a string using Gettext resource manager.
        /// </summary>
        /// <param name="key">The string.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t"),
        SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly",
                         MessageId = "t")]
        public static string t(string key)
        {
            return t(key, Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Translates a string using Gettext resource manager.
        /// </summary>
        /// <param name="key">The string</param>
        /// <param name="callingAssembly">The calling assembly.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design",
                         "CA1062:Validate arguments of public methods", MessageId = "1"),
        SuppressMessage("Microsoft.Naming",
                         "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t"),
        SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly",
                         MessageId = "t")]
        public static string t(string key, Assembly callingAssembly)
        {
            // Get Assembly Name
            string assemblyName = callingAssembly.GetName().Name;

            if (!_ResourceManager.Keys.Contains(assemblyName))
            {
                _ResourceManager.Add(assemblyName, new GettextResourceManager(assemblyName, callingAssembly));
            }

            return _ResourceManager[assemblyName].GetString(key);
        }

        #endregion Methods
    }
}