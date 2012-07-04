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
    using System.Collections.Generic;
    using System.Web.UI;

    public static class ControlExtensions
    {
        #region Methods

        /// <summary>
        /// Finds the control.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="controlID">The control ID.</param>
        /// <returns></returns>
        public static Control FindControlRecursive(this Control parent, string controlId)
        {
            Control current = parent;
            var controlList = new LinkedList<Control>();

            while (current != null)
            {
                if (current.ID == controlId)
                {
                    return current;
                }

                foreach (Control child in current.Controls)
                {
                    if (child.ID == controlId)
                    {
                        return child;
                    }
                    if (child.HasControls())
                    {
                        controlList.AddLast(child);
                    }
                }

                if (controlList.Count == 0)
                {
                    return null;
                }

                current = controlList.First.Value;
                controlList.Remove(current);
            }

            return null;
        }

        /// <summary>
        /// Determines whether the control is inside an <see cref="INamingContainer"/>.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <returns>
        /// 	<c>true</c> if inside an <see cref="INamingContainer"/>; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInNamingContainer(this Control control)
        {
            // If control is Page or null return false.
            if (control == null || control is Page)
            {
                return false;
            }

            // Return true if inside an INamingContainer
            if (control is INamingContainer)
            {
                return true;
            }

            return IsInNamingContainer(control.NamingContainer);
        }

        #endregion Methods
    }
}
