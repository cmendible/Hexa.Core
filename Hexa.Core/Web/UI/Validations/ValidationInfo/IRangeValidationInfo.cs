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

namespace Hexa.Core.Validations
{
	/// <summary>
	/// 
	/// </summary>
	public interface IRangeValidationInfo : IValidationInfo
	{
		/// <summary>
		/// Gets the maximum.
		/// </summary>
		/// <value>The maximum.</value>
		IComparable Maximum { get; }
		/// <summary>
		/// Gets the minimum.
		/// </summary>
		/// <value>The minimum.</value>
		IComparable Minimum { get; }
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TEntity">The type of the entity.</typeparam>
	public class RangeValidationInfo<TEntity> : BaseValidationInfo<TEntity>, IRangeValidationInfo
	{
		private IComparable minimum;
		private IComparable maximum;

		/// <summary>
		/// Initializes a new instance of the <see cref="RangeValidationInfo&lt;TEntity&gt;"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="error">The error.</param>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		public RangeValidationInfo(string propertyName, string error, object minimum, object maximum)
			: base(propertyName, DefaultMessage(propertyName, error, minimum.ToString(), maximum.ToString()))
		{
			this.minimum = minimum as IComparable;
			this.maximum = maximum as IComparable;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RangeValidationInfo&lt;TEntity&gt;"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="error">The error.</param>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Int32.ToString")]
		public RangeValidationInfo(string propertyName, string error, int minimum, int maximum)
			: base(propertyName, DefaultMessage(propertyName, error, minimum.ToString(), maximum.ToString()))
		{
			this.minimum = minimum;
			this.maximum = maximum;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RangeValidationInfo&lt;TEntity&gt;"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="error">The error.</param>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Double.ToString")]
		public RangeValidationInfo(string propertyName, string error, double minimum, double maximum)
			: base(propertyName, DefaultMessage(propertyName, error, minimum.ToString(), maximum.ToString()))
		{
			this.minimum = minimum;
			this.maximum = maximum;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RangeValidationInfo&lt;TEntity&gt;"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="error">The error.</param>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		public RangeValidationInfo(string propertyName, string error, DateTime minimum, DateTime maximum)
			: base(propertyName, DefaultMessage(propertyName, error, minimum.ToShortDateString(), maximum.ToShortDateString()))
		{
			this.minimum = minimum;
			this.maximum = maximum;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RangeValidationInfo&lt;TEntity&gt;"/> class.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="error">The error.</param>
		/// <param name="minimum">The minimum.</param>
		/// <param name="maximum">The maximum.</param>
		public RangeValidationInfo(string propertyName, string error, string minimum, string maximum)
			: base(propertyName, DefaultMessage(propertyName, error, minimum.ToString(), maximum.ToString()))
		{
			this.minimum = minimum;
			this.maximum = maximum;
		}

		/// <summary>
		/// Gets the minimum.
		/// </summary>
		/// <value>The minimum.</value>
		public IComparable Minimum
		{
			get { return this.minimum; }
		}

		/// <summary>
		/// Gets the maximum.
		/// </summary>
		/// <value>The maximum.</value>
		public IComparable Maximum
		{
			get { return this.maximum; }
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object,System.Object,System.Object)")]
		private static string DefaultMessage(string propertyName, string error, string minimum, string maximum)
		{
			if (string.IsNullOrEmpty(error))
				return string.Format(Hexa.Core.Resources.Resource.ValueMustBeBetween1And2, propertyName, minimum, maximum);
			else
				return error;
		}
	}
}
