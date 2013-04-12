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

namespace Hexa.Core
{
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Security.Principal;
    using System.Threading;
    using System.Web;

    using log4net;

    /// <summary>
    /// Core Context singleton class. Contains a reference to a root CoreContainer object.
    /// </summary>
    public static class ApplicationContext
    {
        #region Properties

        public static IPrincipal User
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    return HttpContext.Current.User;
                }
                else
                {
                    return Thread.CurrentPrincipal;
                }
            }
            set
            {
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.User = value;
                }

                Thread.CurrentPrincipal = value;
            }
        }

        #endregion Properties
    }
}