namespace Hexa.Core.Validation
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    /// <summary>
    /// Validation Exception
    /// </summary>
    [Serializable]
    public class ValidationException : CoreException
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="entityType">Type of the entity.</param>
        public ValidationException(Type entityType, string message)
            : base(message)
        {
            ValidationErrors = new ValidationError[] {};
            EntityType = entityType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errors">The errors.</param>
        /// <param name="entityType">Type of the entity.</param>
        public ValidationException(Type entityType, string message, IEnumerable<ValidationError> errors)
            : base(message)
        {
            ValidationErrors = errors;
            EntityType = entityType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationException"/> class.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <param name="entityType">Type of the entity.</param>
        public ValidationException(Type entityType, IEnumerable<ValidationError> errors)
            : base(string.Format(CultureInfo.InvariantCulture, "Entity {0} is not valid.", entityType.Name))
        {
            ValidationErrors = errors;
            EntityType = entityType;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the type of the entity.
        /// </summary>
        /// <value>The type of the entity.</value>
        public Type EntityType
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets or sets the validation errors.
        /// </summary>
        /// <value>The validation errors.</value>
        public IEnumerable<ValidationError> ValidationErrors
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The <paramref name="info"/> parameter is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <PermissionSet>
        /// 	<IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        /// 	<IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion Methods
    }
}