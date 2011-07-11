using System.Collections.Generic;
using System.Security.Principal;
using System.ServiceModel;
using Hexa.Core.ServiceModel;
using log4net;

namespace Hexa.Core.WebServices.Security
{
	internal class ServiceAuthorizationManager : System.ServiceModel.ServiceAuthorizationManager
	{
		protected static readonly ILog _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		protected static List<string> _AnonymousActions = new List<string> {
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
			if (_AnonymousActions.Contains(action)) {
				_Log.Debug("Request authorized as an Anonymous Action");
				return true;
			}

			if (_Log.IsDebugEnabled) {
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
