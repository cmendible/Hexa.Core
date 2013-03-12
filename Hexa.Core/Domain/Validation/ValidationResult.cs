namespace Hexa.Core.Validation
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Contains the result of a <see cref="IValidator{TEntity}.Validate"/> method call.
    /// </summary>
    public sealed class ValidationResult : IEnumerable<ValidationError>
    {
        #region Fields

        private readonly List<ValidationError> _errors = new List<ValidationError>();

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        public ValidationResult()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult"/> class.
        /// </summary>
        /// <param name="errors">The errors.</param>
        public ValidationResult(IEnumerable<ValidationError> errors)
            : this()
        {
            this._errors.AddRange(errors);
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the errors.
        /// </summary>
        public IEnumerable<ValidationError> Errors
        {
            get
            {
                foreach (ValidationError error in this._errors)
                {
                    yield return error;
                }
            }
        }

        /// <summary>
        /// Gets wheater the validation operation on an entity was valid or not.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return this._errors.Count == 0;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds a validation error into the result.
        /// </summary>
        /// <param name="error"></param>
        public void AddError(ValidationError error)
        {
            this._errors.Add(error);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Errors.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<ValidationError> IEnumerable<ValidationError>.GetEnumerator()
        {
            return this.Errors.GetEnumerator();
        }

        /// <summary>
        /// Removes a validation error from the result.
        /// </summary>
        /// <param name="error"></param>
        public void RemoveError(ValidationError error)
        {
            if (this._errors.Contains(error))
            {
                this._errors.Remove(error);
            }
        }

        #endregion Methods
    }
}