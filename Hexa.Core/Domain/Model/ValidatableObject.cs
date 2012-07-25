namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;

    using Validation;

    [Serializable]
    public abstract class ValidatableObject<TEntity> : IValidatable<TEntity>
        where TEntity : ValidatableObject<TEntity>
    {
        #region Fields

        private DataAnnotationsValidator<TEntity> dataAnnotationsValidator;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <value>The validator.</value>
        /// <remarks>Object should _explicitly_ implement IValidatable or this call will fail.</remarks>
        private DataAnnotationsValidator<TEntity> Validator
        {
            get
            {
                if (this.dataAnnotationsValidator == null)
                {
                    this.dataAnnotationsValidator = new DataAnnotationsValidator<TEntity>();
                }

                return this.dataAnnotationsValidator;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Validates this instance.
        /// If instance is not valid, method must throw a ValidationException.
        /// </summary>
        public virtual void AssertValidation()
        {
            this.Validator.AssertValidation((TEntity)this);
        }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsValid()
        {
            return this.Validator.IsValid((TEntity)this);
        }

        /// <summary>
        /// Validates this instance.
        /// If instance is not valid, a collection of errors will be returned.
        /// </summary>
        /// <returns>A list containing error details, or null</returns>
        public virtual ValidationResult Validate()
        {
            return this.Validator.Validate((TEntity)this);
        }

        /// <summary>
        /// Validates the specified validator.
        /// </summary>
        /// <param name="validator">The validator.</param>
        /// <returns></returns>
        public virtual ValidationResult Validate(IValidator<TEntity> validator)
        {
            return validator.Validate((TEntity)this);
        }

        #endregion Methods
    }
}