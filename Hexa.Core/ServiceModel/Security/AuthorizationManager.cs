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
using System.Collections.Generic;
using System.Security.Principal;
using System.ServiceModel;
using log4net;

namespace Hexa.Core.ServiceModel.Security
{
    internal class ServiceAuthorizationManager : System.ServiceModel.ServiceAuthorizationManager
    {
        protected static readonly ILog _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected static List<string> _AnonymousActions = new List<string>
        {
            "http://schemas.xmlsoap.org/ws/2004/09/transfer/Get", // WS-Transfer WSDL request.
        };

        public ServiceAuthorizationManager()
        {
            _Log.Debug("New instance constructed.");
        }

        protected override bool CheckAccessCore(OperationContext operationContext)
        {
            string action = operationContext.RequestContext.RequestMessage.Headers.Action;

            _Log.DebugFormat("Authentication in progress. Action: {0}", action);

            // Check globally anonymous actions..
            if (_AnonymousActions.Contains(action))
                {
                    _Log.Debug("Request authorized as an Anonymous Action");
                    return true;
                }

            if (_Log.IsDebugEnabled)
                {
                    int count = 0;
                    foreach (IIdentity idt in operationContext.ServiceSecurityContext.GetIdentities())
                        _Log.DebugFormat("Identity{1}-{0}: {2}", idt.AuthenticationType, count++, idt.Name);
                }

            if (operationContext.ServiceSecurityContext.AuthorizationContext.Properties.ContainsKey("Principal"))
                {
                    System.Threading.Thread.CurrentPrincipal =
                        (IPrincipal)operationContext.ServiceSecurityContext.AuthorizationContext.Properties["Principal"];

                    return base.CheckAccessCore(operationContext);
                }
            else
                return false;
        }
    }
}
