namespace Hexa.Core.Domain
{
    using System;
    using System.Collections.Generic;

    using Validation;

    [Serializable]
    public abstract class ValidatableObject : IValidatable
    {
        #region Fields

        private IValidator _validator;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <value>The validator.</value>
        /// <remarks>Object should _explicitly_ implement IValidatable or this call will fail.</remarks>
        private IValidator Validator
        {
            get
            {
                if (this._validator == null)
                {
                    this._validator = ServiceLocator.GetInstance<IValidator>();
                }

                return this._validator;
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
            if (!Validator.IsValid(this))
            {
                throw new ValidationException(GetType(), this.Validator.Validate(this));
            }
        }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool IsValid()
        {
            return this.Validator.IsValid(this);
        }

        /// <summary>
        /// Validates this instance.
        /// If instance is not valid, a collection of errors will be returned.
        /// </summary>
        /// <returns>A list containing error details, or null</returns>
        public virtual IEnumerable<ValidationError> Validate()
        {
            return this.Validator.Validate(this);
        }

        #endregion Methods
    }
}