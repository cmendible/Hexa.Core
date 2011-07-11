using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Hexa.Core.Security;
using log4net;

namespace Hexa.Core.WebServices
{
	internal class ServiceFactory : ServiceHostFactory
	{
		private const int MaxTransferSize = 10485760;

		protected Settings _Configuration;
		protected IWebServicesManager _SvcManager;

        private static readonly ILog _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public ServiceFactory()
		{
            _Configuration = Settings.Get();
			_SvcManager = ServiceLocator.GetInstance<IWebServicesManager>();
		}

		protected static bool SchemeEnabled(Uri[] addrs, string scheme)
		{
			foreach (Uri addr in addrs)
			{
				if (addr.Scheme.Equals(scheme))
					return true;
			}
			return false;
		}

        protected static bool HttpsEnabled(Uri[] addrs)
		{
			return SchemeEnabled(addrs, "https");
		}

        protected static bool HttpEnabled(Uri[] addrs)
		{
			return SchemeEnabled(addrs, "http");
		}

		protected WSHttpBinding SetupWSHttpBinding(ServiceHost host)
		{
			// Configure bindings..
            WSHttpBinding binding = new WSHttpBinding();
			binding.Name = "default"; 
			binding.MaxReceivedMessageSize = MaxTransferSize;
			binding.CloseTimeout = new TimeSpan(0, 0, 30);
			binding.MessageEncoding = WSMessageEncoding.Mtom; 
			binding.TransactionFlow = true; 

			binding.Namespace = host.Description.Namespace;
            
            if (IsValid(host))
            {
                switch (_Configuration.SecurityMode)
                {
                    case Hexa.Core.WebServices.Security.SecurityMode.Transport:
                        binding.Security.Mode = System.ServiceModel.SecurityMode.Transport;
                        binding.Security.Transport.Realm = "Hexa Core WebServices";
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                        break;
                    case Hexa.Core.WebServices.Security.SecurityMode.Message:
                        binding.Security.Mode = System.ServiceModel.SecurityMode.Message;
                        binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
                        binding.Security.Message.NegotiateServiceCredential = false;
                        binding.Security.Message.EstablishSecurityContext = false;
                        break;
                    case Hexa.Core.WebServices.Security.SecurityMode.TransportWithMessage:
                        binding.Security.Mode = System.ServiceModel.SecurityMode.TransportWithMessageCredential;
                        binding.Security.Transport.Realm = "Hexa Core WebServices";
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                        binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
                        binding.Security.Message.NegotiateServiceCredential = false;
                        binding.Security.Message.EstablishSecurityContext = false;
                        break;
                    case Hexa.Core.WebServices.Security.SecurityMode.None:
                        binding.Security.Mode = System.ServiceModel.SecurityMode.None;
                        break;
                }
            }

			_Log.DebugFormat("wsHttpBinding binding is ready with Security Mode: {0}", _Configuration.SecurityMode);

			return binding;
		}

		protected BasicHttpBinding SetupBasicHttpBinding(ServiceHost host)
		{
			BasicHttpBinding binding = new BasicHttpBinding();
			binding.Name = "Basic"; 
			binding.MaxReceivedMessageSize = MaxTransferSize;
			binding.CloseTimeout = new TimeSpan(0, 0, 30);
			binding.MessageEncoding = WSMessageEncoding.Text; // TODO#28: Configurable..			
			binding.Namespace = host.Description.Namespace;
            
            if (IsValid(host))
            {
                binding.Security.Transport.Realm = "Hexa Core WebServices";
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;

                switch (_Configuration.SecurityMode)
                {
                    case Hexa.Core.WebServices.Security.SecurityMode.Transport:
                        binding.Security.Mode = BasicHttpSecurityMode.Transport;
                        binding.TransferMode = TransferMode.Streamed; // TODO#28: Configurable..
                        binding.Security.Transport.Realm = "Hexa Core WebServices";
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                        break;
                    case Hexa.Core.WebServices.Security.SecurityMode.Message:
                        binding.Security.Mode = BasicHttpSecurityMode.Message;
                        binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;
                        break;
                    case Hexa.Core.WebServices.Security.SecurityMode.TransportWithMessage:
                        binding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;
                        binding.Security.Transport.Realm = "Hexa Core WebServices";
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                        binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;
                        break;
                    case Hexa.Core.WebServices.Security.SecurityMode.None:
                        binding.Security.Mode = BasicHttpSecurityMode.None;
                        break;
                }
            }
			_Log.DebugFormat("BasicHttpBinding binding is ready with Security Mode: {0}", _Configuration.SecurityMode);

			return binding;
		}

        protected static void SetupClientCredentials(ServiceCredentials behavior)
		{
			((ServiceCredentials)behavior).ClientCertificate.Authentication.CertificateValidationMode =
				System.ServiceModel.Security.X509CertificateValidationMode.None;

			((ServiceCredentials)behavior).ClientCertificate.Authentication.RevocationMode =
				System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        protected void SetupServerCredentials(ServiceCredentials behavior)
		{
			ServiceCredentialsElement cfg = _Configuration.ServiceCredentials;

			if (_Configuration.SecurityMode != Hexa.Core.WebServices.Security.SecurityMode.Transport && cfg == null)
			{
				var ex = new ApplicationException("SecurityMode != Transport, but no ServiceCredentials configuration defined");
				_Log.Error("Configuration exception", ex);
				throw ex;
			}

			if (cfg == null)
				return;

			if (cfg.X509FindType == X509FindType.FindByFile)
			{
				X509Certificate2 serverCertificate = CertificateHelper.LoadFromFile(cfg.FindValue);
				behavior.ServiceCertificate.Certificate = serverCertificate;
			}
			else
			{
				var type = (System.Security.Cryptography.X509Certificates.X509FindType)Enum.Parse(
					typeof(System.Security.Cryptography.X509Certificates.X509FindType),
					cfg.X509FindType.ToString()
				);
				behavior.ServiceCertificate.SetCertificate(
					cfg.StoreLocation,
					cfg.StoreName,
					type,
					cfg.FindValue
				);
			}
		}

		protected void SetupServiceCredentials(ServiceHost host)
		{
			var behavior = new ServiceCredentials();
			SetupClientCredentials(behavior);
			SetupServerCredentials(behavior);
			host.Description.Behaviors.Add(behavior);
		}

		protected void SetupDebugBehavior(ServiceHost host)
		{
			IServiceBehavior behavior = null;

			// Add behaviors..
			if (!host.Description.Behaviors.Contains(typeof(ServiceDebugBehavior)))
			{
				behavior = new ServiceDebugBehavior();
				((ServiceDebugBehavior)behavior).IncludeExceptionDetailInFaults = _Configuration.Debug;
				host.Description.Behaviors.Add(behavior);
			}
			else
			{
				behavior = host.Description.Behaviors.Find<ServiceDebugBehavior>();
				((ServiceDebugBehavior)behavior).IncludeExceptionDetailInFaults = _Configuration.Debug;
			}
		}

        protected static void SetupServiceAuthorization(ServiceHost host)
		{
			IAuthorizationPolicy[] policies = ServiceLocator.GetAllInstances<IAuthorizationPolicy>().ToArray();

            if (policies.Length == 0)
                _Log.Warn("No authorization policies are in place.");
			
			ServiceAuthorizationBehavior authBehavior = host.Description.Behaviors.Find<ServiceAuthorizationBehavior>();

			if (authBehavior == null)
			{
				authBehavior = new ServiceAuthorizationBehavior();
				host.Description.Behaviors.Add(authBehavior);
			}

			authBehavior.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
			host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;
			authBehavior.ServiceAuthorizationManager = new Security.ServiceAuthorizationManager();
			authBehavior.ExternalAuthorizationPolicies = new ReadOnlyCollection<IAuthorizationPolicy>(policies);

			_Log.DebugFormat("{0} IAuthorizationPolicy found and configured", policies.Length);
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        public override System.ServiceModel.ServiceHostBase CreateServiceHost(string constructorString, System.Uri[] baseAddresses)
		{
			WebServiceDescriptor sdesc = null;
			ServiceHost host = null;
			WSHttpBinding binding1 = null;
			BasicHttpBinding binding2 = null;
			bool has_contract = false;

			// Get current Application virtual directory.
			string virtualDirectory = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;

            _Log.DebugFormat("ApplicationVirtualPath: {0}", virtualDirectory);

			// Save current constructorString
			string serviceKey = constructorString;

            _Log.DebugFormat("Constructor string: {0}", constructorString);

			// If virtualDirectory is not empty remove it from the serviceKey.
            if (virtualDirectory != "/")
                serviceKey = serviceKey.Replace(virtualDirectory, string.Empty);
            else
                serviceKey = serviceKey.ToLower();

			if (!_SvcManager.TryGet(serviceKey, out sdesc))
                throw new ArgumentException(String.Format("Invalid service name {0}", serviceKey));

			host = new ServiceHost(sdesc.Container, sdesc.Type, baseAddresses);
			binding1 = SetupWSHttpBinding(host);
			binding2 = SetupBasicHttpBinding(host);

			// Add each service endpoint to service host!
			foreach (Type iface in sdesc.Type.GetInterfaces())
			{
				var attr = (ServiceContractAttribute)Attribute.GetCustomAttribute(iface, typeof(ServiceContractAttribute));

				if (attr != null)
				{
					host.AddServiceEndpoint(iface, binding1, "");
					ServiceEndpoint e = host.AddServiceEndpoint(iface, binding2, binding2.Name);

					// This line causes that the proxy generates ProtectionLevel = None for operations.
					e.Contract.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

					has_contract = true;
				}
			}

			if (!has_contract)
				throw new InvalidOperationException("WebService class does not have a ServiceContract attribute declared!");

            if (IsValid(host))
            {
                SetupServiceCredentials(host); // Setup ServiceHost.Credentials
                SetupDebugBehavior(host); // Setup includeDebugInFaults, etc.
                SetupServiceAuthorization(host); // Setup ServiceAuthorizationBehavior..
            }

			ServiceMetadataBehavior metadataBehavior = new ServiceMetadataBehavior();
			Binding mexbinding = new WSHttpBinding();

			if (HttpEnabled(baseAddresses))
				metadataBehavior.HttpGetEnabled = true;
			if (HttpsEnabled(baseAddresses))
				metadataBehavior.HttpsGetEnabled = true;

			if (!host.Description.Behaviors.Contains(metadataBehavior))
			{
				host.Description.Behaviors.Add(metadataBehavior);
				host.AddServiceEndpoint(typeof(IMetadataExchange), mexbinding, "mex");
			}

			return host;
		}

        private bool IsValid(ServiceHost host)
        {
            foreach (var srv in host.BaseAddresses)
            {
                foreach (Url excluded in _Configuration.ExcludedServices)
                {
                    if (srv.AbsolutePath.ToLower().Contains(excluded.Name.ToLower()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
	}
}