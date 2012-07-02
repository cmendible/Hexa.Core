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

using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GNU.Gettext
{
/// <summary>
/// Class used to call Gettext.
/// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Gettext")]
    public sealed class GettextHelper
    {
        private GettextHelper()
        {
        }

        /// <summary>
        /// Gettext resource manager.
        /// </summary>
        private static Dictionary<string, GettextResourceManager> _ResourceManager = new Dictionary<string, GettextResourceManager>();

        /// <summary>
        /// Translates a string using Gettext resource manager.
        /// </summary>
        /// <param name="key">The string.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "t")]
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "t"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "t")]
        public static string t(string key, Assembly callingAssembly)
        {
            // Get Assembly Name
            string assemblyName = callingAssembly.GetName().Name;

            if (!_ResourceManager.Keys.Contains(assemblyName))
                _ResourceManager.Add(assemblyName, new GettextResourceManager(assemblyName, callingAssembly));

            return _ResourceManager[assemblyName].GetString(key);
        }

        public static void ForceInvariantCulture()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
        }
    }

}
