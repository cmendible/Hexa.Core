using System;

namespace Hexa.Core.Validation
{
    /// <summary>
    /// Contains the details of a validation error
    /// </summary>
    public interface IValidationError
    {
        /// <summary>
        /// This is the class type that the validation result is applicable to.  For instance,
        /// if the validation result concerns a duplicate record found for an employee, then 
        /// this property would hold the typeof(Employee).  It should be expected that this 
        /// property will never be null.
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// If the validation result is applicable to a specific property, then this 
        /// <see cref="PropertyName" /> would be set to a property name.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Holds the message describing the validation result for the ClassContext and/or PropertyContext
        /// </summary>
        string Message { get; }
    }

}
