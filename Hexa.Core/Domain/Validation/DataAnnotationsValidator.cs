//----------------------------------------------------------------------------------------------
// <copyright file="DataAnnotationsValidator.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Validation
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    [Serializable]
    public class DataAnnotationsValidator<TEntity> : IValidator<TEntity>
    {
        public void AssertValidation(TEntity instance)
        {
            ValidationResult result = this.Validate(instance);
            if (!result.IsValid)
            {
                throw new ValidationException(instance.GetType(), result.Errors);
            }
        }

        public bool IsValid(TEntity instance)
        {
            return this.Validate(instance).IsValid;
        }

        public virtual ValidationResult Validate(TEntity instance)
        {
            Type entityType = instance.GetType();

            IEnumerable<ValidationError> errors =
                from prop in TypeDescriptor.GetProperties(instance).Cast<PropertyDescriptor>()
                from attribute in prop.Attributes.OfType<ValidationAttribute>()
                where !attribute.IsValid(prop.GetValue(instance))
                select
                new ValidationError(
                    entityType,
                    attribute.FormatErrorMessage(string.Empty),
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
    }
}