using System.Collections.Generic;

using Hexa.Core.Validation;

namespace Hexa.Core.Domain
{
    public abstract class ValidatableObject : IValidatable
    {

        #region IValidatable Implementation

        private IValidator _validator = null;

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <value>The validator.</value>
        /// <remarks>Object should _explicitly_ implement IValidatable or this call will fail.</remarks>
        private IValidator Validator
        {
            get
            {
                if (_validator == null)
                    _validator = ServiceLocator.GetInstance<IValidator>();

                return _validator;
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
            return Validator.IsValid(this);
        }

        /// <summary>
        /// Validates this instance.
        /// If instance is not valid, a collection of errors will be returned.
        /// </summary>
        /// <returns>A list containing error details, or null</returns>
        public virtual IEnumerable<IValidationError> Validate()
        {
            return Validator.Validate(this);
        }

        /// <summary>
        /// Validates this instance.
        /// If instance is not valid, method must throw a ValidationException.
        /// </summary>
        public virtual void AssertValidation()
        {
            if (!Validator.IsValid(this))
                throw new ValidationException(this.GetType(), Validator.Validate(this));
        }

        #endregion
    }
}
