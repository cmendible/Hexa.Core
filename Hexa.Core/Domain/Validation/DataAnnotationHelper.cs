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

namespace Hexa.Core.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;

    using GNU.Gettext;

    /// <summary>
    /// Static class capable of readinng de DataAnnotations of a type and return a list of corresponding IValidationInfos.
    /// </summary>
    internal static class DataAnnotationHelper
    {
        #region Methods

        public static string ParseDisplayName(Type entityType, string propertyName)
        {
            string displayName = propertyName;

            DisplayAttribute displayAttribute = TypeDescriptor.GetProperties(entityType)
                                                .Cast<PropertyDescriptor>()
                                                .Where(p => p.Name == propertyName)
                                                .SelectMany(p => p.Attributes.OfType<DisplayAttribute>()).FirstOrDefault();

            if (displayAttribute != null)
            {
                displayName = displayAttribute.Name;
            }

            return GettextHelper.t(displayName, entityType.Assembly);
        }

        /// <summary>
        /// Reads the data annotations list.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <returns></returns>
        //public static IDictionary<string, ValidationAttribute> GetDataAnnotations<TEntity>()
        //{
        //    return (from prop in TypeDescriptor.GetProperties(typeof(TEntity)).Cast<PropertyDescriptor>()
        //            from attribute in prop.Attributes.OfType<ValidationAttribute>()
        //            select new KeyValuePair<string, ValidationAttribute>(prop.Name, attribute))
        //            .ToDictionary(k => k.Key, s => s.Value);
        //}

        #endregion Methods
    }
}