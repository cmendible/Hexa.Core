namespace Hexa.Core.Validation
{
    using System;

    /// <summary>
    /// Details of a validation error
    /// </summary>
    public class ValidationError
    {
        #region Constructors

        /// <summary>
        /// Default Constructor.
        /// Creates a new instance of the <see cref="ValidationError"/> data structure.
        /// </summary>
        /// <param name="message">string. The validation error message.</param>
        /// <param name="property">string. The property that was validated.</param>
        public ValidationError(string message, string property)
        {
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(message),
                                                 "Please provide a valid non null string as the validation error message");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty(property),
                                                 "Please provide a valid non null string as the validation property name");

            this.EntityType = typeof(void); // Avoid make this.EntityType == null as to not breaking existing code.
            this.Message = message;
            this.PropertyName = property;
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ValidationError"/> data structure.
        /// </summary>
        /// <param name="entityType">Type of the entity.</param>
        /// <param name="message">string. The validation error message.</param>
        /// <param name="property">string. The property that was validated.</param>
        public ValidationError(Type entityType, string message, string property)
        {
            Guard.Against<ArgumentNullException>(entityType == null,
                                                 "Please provide a valid non null Type as the validated Entity");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty("message"),
                                                 "Please provide a valid non null string as the validation error message");
            Guard.Against<ArgumentNullException>(string.IsNullOrEmpty("property"),
                                                 "Please provide a valid non null string as the validation property name");
            this.EntityType = entityType;
            this.Message = message;
            this.PropertyName = property;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> struct.
        /// </summary>
        /// <param name="ex">The exception which caused the validation error.</param>
        public ValidationError(string property, Exception ex)
            : this(ex.Message, property)
        {
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public Type EntityType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <value>The name of the property.</value>
        public string PropertyName
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ValidationError left, ValidationError right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(ValidationError left, ValidationError right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Overridden. Compares if an object is equal to the <see cref="ValidationError"/> instance.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(ValidationError))
            {
                return false;
            }
            return Equals((ValidationError)obj);
        }

        /// <summary>
        /// Overriden. Compares if a <see cref="ValidationError"/> instance is equal to this
        /// <see cref="ValidationError"/> instance.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals(ValidationError obj)
        {
            return Equals(obj.EntityType, this.EntityType)
                   && Equals(obj.PropertyName, this.PropertyName)
                   && Equals(obj.Message, this.Message);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.Message.GetHashCode() * 397) ^ this.PropertyName.GetHashCode();
            }
        }

        /// <summary>
        /// Overriden. Gets a string that represents the validation error.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("({0}::{1}) - {2}", this.EntityType, this.PropertyName, this.Message);
        }

        #endregion Methods
    }
}