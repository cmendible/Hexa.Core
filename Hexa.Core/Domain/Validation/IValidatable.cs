namespace Hexa.Core.Validation
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for validatable objects.
    /// </summary>
    public interface IValidatable
    {
        #region Methods

        /// <summary>
        /// Validates this instance.
        /// If instance is not valid, method must throw a ValidationException.
        /// </summary>
        void AssertValidation();

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        bool IsValid();

        /// <summary>
        /// Validates this instance.
        /// If instance is not valid, a collection of errors will be returned.
        /// </summary>
        /// <returns>A list containing error details, or null</returns>
        ValidationResult Validate();

        #endregion Methods
    }

    /// <summary>
    /// Interface for validatable objects.
    /// </summary>
    public interface IValidatable<T> : IValidatable
    {
        #region Methods

        /// <summary>
        /// Validates an entitty using the specified validator.
        /// </summary>
        /// <param name="validator">The validator.</param>
        /// <returns></returns>
        ValidationResult Validate(IValidator<T> validator);

        #endregion Methods
    }
}