
namespace Hexa.Core.Validation
{
	/// <summary>
	/// Interface implemented by different flavors of validators that provide validation
	/// logic on domain entities.
	/// </summary>
	public interface IValidator<TEntity>
	{
		/// <summary>
		/// Determines whether the specified instance is valid.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns>
		/// 	<c>true</c> if the specified instance is valid; otherwise, <c>false</c>.
		/// </returns>
		bool IsValid(TEntity instance);
		/// <summary>
		/// Validates this instance.
		/// If instance is not valid, method must throw a ValidationException.
		/// </summary>
		/// <param name="instance">The instance.</param>
		void AssertValidation(TEntity instance);
		/// <summary>
		/// Validates the specified instance.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <returns></returns>
		ValidationResult Validate(TEntity instance);
	}

	/// <summary>
	/// Interface implemented by different flavors of validators that provide validation
	/// logic on domain entities.
	/// </summary>
	public interface IValidator : IValidator<object>
	{
	}
}
