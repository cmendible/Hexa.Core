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

namespace Hexa.Core.Validation
{
    using System.Diagnostics.CodeAnalysis;
    using Resources;

    /// <summary>
    ///
    /// </summary>
    public interface IRegexValidationInfo : IValidationInfo
    {
        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <value>The expression.</value>
        string Expression { get; }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class RegexValidationInfo<TEntity> : BaseValidationInfo<TEntity>, IRegexValidationInfo
    {
        private readonly string expression;

        /// <summary>
        /// Initializes a new instance of the RequiredValidationInfo class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="expression">The expression.</param>
        public RegexValidationInfo(string propertyName, string expression)
            : this(propertyName, null, expression)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RequiredValidationInfo class.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="error">The error.</param>
        /// <param name="expression">The expression.</param>
        public RegexValidationInfo(string propertyName, string error, string expression)
            : base(propertyName, DefaultMessage<TEntity>(propertyName, error))
        {
            this.expression = expression;
        }

        #region IRegexValidationInfo Members

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <value>The expression.</value>
        public string Expression
        {
            get { return expression; }
        }

        #endregion

        [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider",
            MessageId = "System.String.Format(System.String,System.Object)")]
        private static string DefaultMessage<TEntity>(string propertyName, string error)
        {
            if (string.IsNullOrEmpty(error))
                return string.Format(Resource.ValueIsNotCorrectlyFormatted,
                                     DataAnnotationHelper.ParseDisplayName(typeof(TEntity), propertyName));
            else
                return error;
        }
    }
}