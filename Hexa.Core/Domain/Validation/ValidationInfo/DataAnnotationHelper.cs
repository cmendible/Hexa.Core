#region License

//===================================================================================
//Copyright 2010 HexaSystems Corporation
//===================================================================================
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//===================================================================================
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//===================================================================================

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using GNU.Gettext;

namespace Hexa.Core.Validation
{
	/// <summary>
	/// Static class capable of readinng de DataAnnotations of a type and return a list of corresponding IValidationInfos.
	/// </summary>
	internal static class DataAnnotationHelper
	{
		/// <summary>
		/// Reads the data annotations of type TEntity and return a list of corresponding IValidationInfos
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <returns></returns>
		public static IList<IValidationInfo> GetValidationInfoList<TEntity>()
		{
            return (from prop in TypeDescriptor.GetProperties(typeof(TEntity)).Cast<PropertyDescriptor>()
                    from attribute in prop.Attributes.OfType<ValidationAttribute>()
                    select ConvertDataAnnotation<TEntity>(attribute as ValidationAttribute, prop)).ToList();
		}

		/// <summary>
		/// Reads the data annotations list.
		/// </summary>
		/// <typeparam name="TEntity">The type of the entity.</typeparam>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public static IList<KeyValuePair<string, ValidationAttribute>> GetDataAnnotationsList<TEntity>()
		{

            return (from prop in TypeDescriptor.GetProperties(typeof(TEntity)).Cast<PropertyDescriptor>()
                    from attribute in prop.Attributes.OfType<ValidationAttribute>()
                    select new KeyValuePair<string, ValidationAttribute>(prop.Name, attribute)).ToList();
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
				RequiredAttribute reqAtt = att as RequiredAttribute;
                return new RequiredValidationInfo<TEntity>(prop.Name, reqAtt.ErrorMessage);
			}
			else if (att.GetType() == typeof(RegularExpressionAttribute))
			{
				RegularExpressionAttribute regAtt = att as RegularExpressionAttribute;
                return new RegexValidationInfo<TEntity>(prop.Name, regAtt.ErrorMessage, regAtt.Pattern);
			}
			if (att.GetType() == typeof(RangeAttribute))
			{
				RangeAttribute rangeAtt = att as RangeAttribute;
                return new RangeValidationInfo<TEntity>(prop.Name, rangeAtt.ErrorMessage, rangeAtt.Minimum, rangeAtt.Maximum);
			}
			if (att.GetType() == typeof(StringLengthAttribute))
			{
				StringLengthAttribute lengthAtt = att as StringLengthAttribute;
                return new RegexValidationInfo<TEntity>(prop.Name, lengthAtt.ErrorMessage, 
					string.Format(CultureInfo.InvariantCulture,"{0}{1}{2}", "^[\\s\\S]{0,", 
					lengthAtt.MaximumLength.ToString(CultureInfo.InvariantCulture), "}$"));
			}

			return null;
		}

        public static string ParseDisplayName(Type entityType, string propertyName)
        {
            var displayName = propertyName;

            var displayAttribute = TypeDescriptor.GetProperties(entityType)
                 .Cast<PropertyDescriptor>()
                 .Where(p => p.Name == propertyName)
                 .SelectMany(p => p.Attributes.OfType<DisplayAttribute>()).FirstOrDefault();

            if (displayAttribute != null)
                displayName = displayAttribute.Name;

            return GettextHelper.t(displayName, entityType.Assembly);
        }
	}
}
