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

namespace Hexa.Core.Web.UI.Controls.Validation
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Returns the IValidationInfos corresponding to the DataAnnotations in type Tentity
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class DataAnnotationValidationInfoProvider<TEntity> : IValidationInfoProvider
    {
        #region Methods

        /// <summary>
        /// Gets the validation info.
        /// </summary>
        /// <returns></returns>
        public IList<IValidationInfo> GetValidationInfo()
        {
            return GetValidationInfoList<TEntity>();
        }

        /// <summary>
        /// Gets the validation info.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public IList<IValidationInfo> GetValidationInfo(string propertyName)
        {
            return this.GetValidationInfo().Where(i => i.PropertyInfo.Name == propertyName).ToList();
        }

        /// <summary>
        /// Reads the data annotations of type TEntity and return a list of corresponding IValidationInfos
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        public static IList<IValidationInfo> GetValidationInfoList<TEntity>()
        {
            return (from prop in TypeDescriptor.GetProperties(typeof(TEntity)).Cast<PropertyDescriptor>()
                    from attribute in prop.Attributes.OfType<ValidationAttribute>()
                    select ConvertDataAnnotation<TEntity>(attribute, prop)).ToList();
        }

        /// <summary>
        /// Converts the data annotation.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="att">The att.</param>
        /// <param name="prop">The prop.</param>
        /// <returns></returns>
        private static IValidationInfo ConvertDataAnnotation<TEntity>(ValidationAttribute att, PropertyDescriptor prop)
        {
            if (att.GetType() == typeof(RequiredAttribute))
            {
                var reqAtt = att as RequiredAttribute;
                return new RequiredValidationInfo<TEntity>(prop.Name, reqAtt.ErrorMessage);
            }
            else if (att.GetType() == typeof(RegularExpressionAttribute))
            {
                var regAtt = att as RegularExpressionAttribute;
                return new RegexValidationInfo<TEntity>(prop.Name, regAtt.ErrorMessage, regAtt.Pattern);
            }
            if (att.GetType() == typeof(RangeAttribute))
            {
                var rangeAtt = att as RangeAttribute;
                return new RangeValidationInfo<TEntity>(prop.Name, rangeAtt.ErrorMessage, rangeAtt.Minimum,
                                                        rangeAtt.Maximum);
            }
            if (att.GetType() == typeof(StringLengthAttribute))
            {
                var lengthAtt = att as StringLengthAttribute;
                return new RegexValidationInfo<TEntity>(prop.Name, lengthAtt.ErrorMessage,
                                                        string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}",
                                                                "^[\\s\\S]{0,",
                                                                lengthAtt.MaximumLength.ToString(
                                                                        CultureInfo.InvariantCulture), "}$"));
            }

            return null;
        }

        #endregion Methods
    }
}