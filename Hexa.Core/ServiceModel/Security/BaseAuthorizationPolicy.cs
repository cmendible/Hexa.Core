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
namespace Hexa.Core.ServiceModel.Security
{
    using System;
    using System.IdentityModel.Claims;
    using System.IdentityModel.Policy;
    using System.Reflection;
    using System.Security.Principal;

    using Hexa.Core.Logging;

    /// <summary>
    ///
    /// </summary>
    public abstract class BaseAuthorizationPolicy : IAuthorizationPolicy
    {
        private static readonly ILogger _Log = LoggerManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseAuthorizationPolicy"/> class.
        /// </summary>
        protected BaseAuthorizationPolicy()
        {
            this.Id = Guid.NewGuid().ToString();
            _Log.DebugFormat("New instance {0} created.", this.Id);
        }

        public string Id
        {
            get;
            protected set;
        }

        public ClaimSet Issuer
        {
            get
            {
                return ClaimSet.System;
            }
        }

        /// <summary>
        /// Evaluates whether a user meets the requirements for this authorization policy.
        /// </summary>
        /// <param name="evaluationContext">An <see cref="T:System.IdentityModel.Policy.EvaluationContext"/> that contains the claim set that the authorization policy evaluates.</param>
        /// <param name="state">A <see cref="T:System.Object"/>, passed by reference that represents the custom state for this authorization policy.</param>
        /// <returns>
        /// false if the <see cref="M:System.IdentityModel.Policy.IAuthorizationPolicy.Evaluate(System.IdentityModel.Policy.EvaluationContext,System.Object@)"/> method for this authorization policy must be called if additional claims are added by other authorization policies to <paramref name="evaluationContext"/>; otherwise, true to state no additional evaluation is required by this authorization policy.
        /// </returns>
        public abstract bool Evaluate(EvaluationContext evaluationContext, ref object state);

        /// <summary>
        /// Setup ups the evaluation context.
        /// AuthorizationPolicies should call this method when authorizing requests
        /// to create an apropiate Principal/Identity objects.
        /// </summary>
        /// <param name="ctx">The EvaluationContext.</param>
        /// <param name="claims">The List of Claims to add at generated ClaimSet</param>
        protected static void SetupEvaluationContext(EvaluationContext context, IPrincipal principal)
        {
            _Log.DebugFormat("User: {0} was authorized", principal.Identity.Name);
            context.Properties["Principal"] = principal;
        }
    }
}