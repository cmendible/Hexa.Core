using System;
using System.Globalization;
using System.Collections.Generic;

namespace Hexa.Core.Validation
{
	/// <summary>
	/// Validation Exception
	/// </summary>
	[Serializable]
	public class ValidationException: CoreException
	{
		/// <summary>
		/// Gets or sets the validation errors.
		/// </summary>
		/// <value>The validation errors.</value>
		public IEnumerable<IValidationError> ValidationErrors { get; private set; }

		/// <summary>
		/// Gets or sets the type of the entity.
		/// </summary>
		/// <value>The type of the entity.</value>
		public Type EntityType { get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="errors">The errors.</param>
		/// <param name="entityType">Type of the entity.</param>
		public ValidationException(Type entityType, string message)
			: base(message)
		{
			ValidationErrors = new IValidationError[] {};
			EntityType = entityType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ValidationException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="errors">The errors.</param>
		/// <param name="entityType">Type of the entity.</param>
		public ValidationException(Type entityType, string message, IEnumerable<IValidationError> errors) 
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
		public ValidationException(Type entityType, IEnumerable<IValidationError> errors)
			: base(string.Format(CultureInfo.InvariantCulture, "Entity {0} is not valid.", entityType.Name))
		{
			ValidationErrors = errors;
			EntityType = entityType;
		}
	}
}
