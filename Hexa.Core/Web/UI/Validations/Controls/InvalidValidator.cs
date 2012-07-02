#region License

// ===================================================================================
// Copyright 2010 HexaSystems Corporation
// ===================================================================================
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// ===================================================================================
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// See the License for the specific language governing permissions and
// ===================================================================================

#endregion

using System.Web.UI;

namespace Hexa.Core.Web.UI.Controls
{
	/// <summary>
	/// Validator used to show a custom message in a ValidationSummary Control.
	/// </summary>
	internal class InvalidValidator : IValidator
	{
		/// <summary>
		/// Initializes a new instance of the ShowErrorValidator class.
		/// </summary>
		/// <param name="message">The message.</param>
		public InvalidValidator(string message)
		{
			ErrorMessage = message;
		}

		/// <summary>
		/// When implemented by a class, gets or sets the error message text generated when the condition being validated fails.
		/// </summary>
		/// <value></value>
		/// <returns>
		/// The error message to generate.
		/// </returns>
		public string ErrorMessage
		{
			get;
			set;
		}

		/// <summary>
		/// When implemented by a class, gets or sets a value indicating whether the user-entered content in the specified control passes validation.
		/// </summary>
		/// <value></value>
		/// <returns>true if the content is valid; otherwise, false.
		/// </returns>
		public bool IsValid
		{
			get { return false; }
			set { return; }
		}

		/// <summary>
		/// When implemented by a class, evaluates the condition it checks and updates the <see cref="P:System.Web.UI.IValidator.IsValid"/> property.
		/// </summary>
		public void Validate()
		{
			return;
		}
	}
}
