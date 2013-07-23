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
    using System.Collections.Generic;
    using System.Reflection;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Threading;

    using Hexa.Core.Logging;

    // <configuration>
    // <system.serviceModel>
    //  <behaviors>
    //    <serviceAuthorization serviceAuthorizationManagerType=
    //              "Samples.MyServiceAuthorizationManager" >
    //      <authorizationPolicies>
    //        <add policyType="Samples.MyAuthorizationPolicy"
    //      </authorizationPolicies>
    //    </serviceAuthorization>
    //  </behaviors>
    // </system.serviceModel>
    // </configuration>
    public class ServiceAuthorizationManager : System.ServiceModel.ServiceAuthorizationManager
    {
        protected static readonly ILogger Log =
            LoggerManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected static List<string> AnonymousActions = new List<string> { "http://schemas.xmlsoap.org/ws/2004/09/transfer/Get" };

        public ServiceAuthorizationManager()
        {
            Log.Debug("New instance constructed.");
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            string action = operationContext.RequestContext.RequestMessage.Headers.Action;

            Log.DebugFormat("Authentication in progress. Action: {0}", action);

            // Check globally anonymous actions..
            if (AnonymousActions.Contains(action))
            {
                Log.Debug("Request authorized as an Anonymous Action");
                return true;
            }

            int count = 0;
            foreach (IIdentity idt in operationContext.ServiceSecurityContext.GetIdentities())
            {
                Log.DebugFormat("Identity{1}-{0}: {2}", idt.AuthenticationType, count++, idt.Name);
            }

            if (operationContext.ServiceSecurityContext.AuthorizationContext.Properties.ContainsKey("Principal"))
            {
                Thread.CurrentPrincipal =
                    (IPrincipal)operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"];

                return base.CheckAccessCore(operationContext);
            }
            else
            {
                return false;
            }
        }
    }
}