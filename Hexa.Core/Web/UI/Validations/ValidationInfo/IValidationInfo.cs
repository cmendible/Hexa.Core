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
using System.Reflection;
using System.Linq.Expressions;

namespace Hexa.Core.Validations
{
	/// <summary>
	/// Validation Info interface.
	/// </summary>
	public interface IValidationInfo
	{
		string ErrorMessage { get; }
		PropertyInfo PropertyInfo { get; }
	}

	/// <summary>
	/// Abstarct class for Validation infos.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public abstract class BaseValidationInfo<TEntity> : IValidationInfo
	{
		private string _ErrorMessage;
		private PropertyInfo _PropertyInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="BaseValidationInfo&lt;TEntity&gt;"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="error">The error.</param>
		protected BaseValidationInfo(string propertyName, string error)
		{
			_ErrorMessage = error;
			_PropertyInfo = typeof(TEntity).GetProperty(propertyName);
		}

		/// <summary>
		/// Gets the error.
		/// </summary>
		/// <value>The error.</value>
		public virtual string ErrorMessage
		{
			get { return _ErrorMessage; }
		}

		/// <summary>
		/// Gets the property info.
		/// </summary>
		/// <value>The property info.</value>
		public virtual PropertyInfo PropertyInfo
		{
			get { return _PropertyInfo; }
		}

	}
}
