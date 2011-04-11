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

using System.Collections.Generic;

namespace Hexa.Core.Validation
{
	/// <summary>
	/// Validation Provider Interface.
	/// </summary>
	public interface IValidationInfoProvider
	{
		/// <summary>
		/// Gets the validation info.
		/// </summary>
		/// <returns></returns>
		IList<IValidationInfo> GetValidationInfo();

		/// <summary>
		/// Gets the validation info.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns></returns>
		IList<IValidationInfo> GetValidationInfo(string propertyName);
	}

	/// <summary>
	/// Validation Provider Interface for TEntity.
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
    public interface IValidationInfoProvider<TEntity> : IValidationInfoProvider
	{
	}
}
