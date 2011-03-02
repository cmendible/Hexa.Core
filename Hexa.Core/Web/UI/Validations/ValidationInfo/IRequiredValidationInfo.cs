#region License

//===================================================================================
//Copyright 2010 HexaSystems Corporation
//===================================================================================
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//===================================================================================
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//===================================================================================

#endregion

using System;
using System.Linq.Expressions;

namespace Hexa.Core.Validations
{
	/// <summary>
	/// 
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
	public interface IRequiredValidationInfo : IValidationInfo
	{
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public class RequiredValidationInfo<TEntity> : BaseValidationInfo<TEntity>, IRequiredValidationInfo
	{

		/// <summary>
		/// Initializes a new instance of the RequiredValidationInfo class.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		public RequiredValidationInfo(string propertyName)
			: this(propertyName, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the RequiredValidationInfo class.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="error">The error.</param>
		public RequiredValidationInfo(string propertyName, string error)
			: base(propertyName, DefaultMessage(propertyName, error))
		{
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)")]
		private static string DefaultMessage(string propertyName, string error)
		{
			if (string.IsNullOrEmpty(error))
				return string.Format(Hexa.Core.Resources.Resource.IsRequired, propertyName);
			else
				return error;
		}
	}

}
