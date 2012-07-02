namespace Hexa.Core.Validation
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Contains the result of a <see cref="IValidator{TEntity}.Validate"/> method call.
    /// </summary>
    public sealed class ValidationResult : IEnumerable<ValidationError>
    {
        #region fields

        private readonly List<ValidationError> _errors = new List<ValidationError>();

        #endregion

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
            _errors.AddRange(errors);
        }

        #region properties

        /// <summary>
        /// Gets wheater the validation operation on an entity was valid or not.
        /// </summary>
        public bool IsValid
        {
            get { return _errors.Count == 0; }
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable{ValidationError}"/> that can be used to enumerate over
        /// the validation errors as a result of a <see cref="IValidatable.Validate"/> method
        /// call.
        /// </summary>
        public IEnumerable<ValidationError> Errors
        {
            get
            {
                foreach (ValidationError error in _errors)
                    yield return error;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Adds a validation error into the result.
        /// </summary>
        /// <param name="error"></param>
        public void AddError(ValidationError error)
        {
            _errors.Add(error);
        }

        /// <summary>
        /// Removes a validation error from the result.
        /// </summary>
        /// <param name="error"></param>
        public void RemoveError(ValidationError error)
        {
            if (_errors.Contains(error))
                _errors.Remove(error);
        }

        #endregion

        #region IEnumerable..

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Errors.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<ValidationError> IEnumerable<ValidationError>.GetEnumerator()
        {
            return Errors.GetEnumerator();
        }

        #endregion
    }
}