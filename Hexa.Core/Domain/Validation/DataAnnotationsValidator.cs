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
    using System.Linq;

    [Serializable]
    public class DataAnnotationsValidator : IValidator
    {
        #region Methods

        public void AssertValidation(object instance)
        {
            ValidationResult result = Validate(instance);
            if (!result.IsValid)
            {
                throw new ValidationException(instance.GetType(), result.Errors);
            }
        }

        public bool IsValid(object instance)
        {
            return Validate(instance).IsValid;
        }

        public ValidationResult Validate(object instance)
        {
            Type entityType = instance.GetType();

            IEnumerable<ValidationError> errors =
                from prop in TypeDescriptor.GetProperties(instance).Cast<PropertyDescriptor>()
                from attribute in prop.Attributes.OfType<ValidationAttribute>()
                where !attribute.IsValid(prop.GetValue(instance))
                select
                new ValidationError(entityType, attribute.FormatErrorMessage(string.Empty),
                                    DataAnnotationHelper.ParseDisplayName(entityType, prop.Name));

            if (errors.Any())
            {
                return new ValidationResult(errors.Cast<ValidationError>());
            }
            else
            {
                return new ValidationResult();
            }
        }

        #endregion Methods
    }
}