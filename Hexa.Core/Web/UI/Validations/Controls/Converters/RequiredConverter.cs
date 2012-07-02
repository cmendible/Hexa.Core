

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

namespace Hexa.Core.Web.UI.Controls
{
    using System.Web.UI.WebControls;
    using Validation;

    /// <summary>
    /// Class used to convert from a Required attribute to a BaseValidator
    /// </summary>
    internal class RequiredConverter : BaseConverter
    {
        /// <summary>
        /// Converts a given attribute to a BaseValidator
        /// </summary>
        /// <param name="attribute">Attribute representing the validator</param>
        /// <param name="pi">Property that holds attribute</param>
        /// <returns>A BaseValidator</returns>
        public override BaseValidator Convert(IValidationInfo validationInfo)
        {
            return new ExtendedRequiredFieldValidator();
        }
    }
}