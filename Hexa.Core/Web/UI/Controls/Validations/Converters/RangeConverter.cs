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

namespace Hexa.Core.Web.UI.Controls
{
    using System;
    using System.Web.UI.WebControls;

    using Validation;

    /// <summary>
    /// Class used to convert from a Range attribute to a BaseValidator
    /// </summary>
    internal class RangeConverter : BaseConverter
    {
        #region Methods

        /// <summary>
        /// Converts a given attribute to a BaseValidator
        /// </summary>
        /// <param name="attribute">Attribute representing the validator</param>
        /// <param name="pi">Property that holds attribute</param>
        /// <returns>A BaseValidator</returns>
        public override BaseValidator Convert(IValidationInfo validationInfo)
        {
            var rangeValidator = new ExtendedRangeValidator();

            var rangevalidationInfo = validationInfo as IRangeValidationInfo;

            rangeValidator.MinimumValue = rangevalidationInfo.Minimum.ToString();
            rangeValidator.MaximumValue = rangevalidationInfo.Maximum.ToString();

            switch (Type.GetTypeCode(rangevalidationInfo.Minimum.GetType()))
            {
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
                rangeValidator.Type = ValidationDataType.Integer;
                break;
            case TypeCode.Double:
                rangeValidator.Type = ValidationDataType.Double;
                break;
            case TypeCode.DateTime:
                rangeValidator.Type = ValidationDataType.Date;
                break;
            case TypeCode.String:
                rangeValidator.Type = ValidationDataType.String;
                break;
            }

            return rangeValidator;
        }

        #endregion Methods
    }
}