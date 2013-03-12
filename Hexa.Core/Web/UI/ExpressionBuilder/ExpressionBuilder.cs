#region Header

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

#endregion Header

namespace Hexa.Core.Web.UI
{
    using System.CodeDom;
    using System.Web.Compilation;
    using System.Web.UI;

    /// <summary>
    ///
    /// </summary>
    [ExpressionPrefix("Code")]
    public sealed class CodeExpressionBuilder : ExpressionBuilder
    {
        #region Methods

        /// <summary>
        /// When overridden in a derived class, returns code that is used during page execution to obtain the evaluated expression.
        /// Requieres de following configuration in web.config:
        /// <compilation debug="false">
        ///    <expressionBuilders>
        ///        <add expressionPrefix="Code" type="Hexa.Core.Web.UI.CodeExpressionBuilder, Hexa.Core"/>
        ///    </expressionBuilders>
        ///	</compilation>
        /// </summary>
        /// <param name="entry">The object that represents information about the property bound to by the expression.</param>
        /// <param name="parsedData">The object containing parsed data as returned by <see cref="M:System.Web.Compilation.ExpressionBuilder.ParseExpression(System.String,System.Type,System.Web.Compilation.ExpressionBuilderContext)"/>.</param>
        /// <param name="context">Contextual information for the evaluation of the expression.</param>
        /// <returns>
        /// A <see cref="T:System.CodeDom.CodeExpression"/> that is used for property assignment.
        /// </returns>
        public override CodeExpression GetCodeExpression(BoundPropertyEntry entry, object parsedData,
            ExpressionBuilderContext context)
        {
            return new CodeSnippetExpression(entry.Expression);
        }

        #endregion Methods
    }
}