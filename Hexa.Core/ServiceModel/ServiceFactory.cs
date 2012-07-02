using System;
using System.Collections.ObjectModel;
using System.IdentityModel.Policy;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
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

using System.ServiceModel.Activation;
using System.ServiceModel.Description;
using Hexa.Core.Security;
using log4net;

namespace Hexa.Core.ServiceModel
{
	public class ServiceFactory : ServiceHostFactory
	{
		private const int MaxTransferSize = 10485760;
		private Settings _Configuration;
        private static readonly ILog _Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public ServiceFactory()
		{
            _Configuration = Settings.Get();
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
            var binding = new WSHttpBinding();
			binding.Name = "default"; 
			binding.MaxReceivedMessageSize = MaxTransferSize;
			binding.CloseTimeout = new TimeSpan(0, 0, 30);
			binding.MessageEncoding = WSMessageEncoding.Mtom; 
			binding.TransactionFlow = true; 

			binding.Namespace = host.Description.Namespace;
            
            if (_ExcludeHost(host))
            {
                switch (_Configuration.SecurityMode)
                {
                    case Hexa.Core.ServiceModel.Security.SecurityMode.Transport:
                        binding.Security.Mode = System.ServiceModel.SecurityMode.Transport;
                        binding.Security.Transport.Realm = "Hexa Core WebServices";
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                        break;
                    case Hexa.Core.ServiceModel.Security.SecurityMode.Message:
                        binding.Security.Mode = System.ServiceModel.SecurityMode.Message;
                        binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
                        binding.Security.Message.NegotiateServiceCredential = false;
                        binding.Security.Message.EstablishSecurityContext = false;
                        break;
                    case Hexa.Core.ServiceModel.Security.SecurityMode.TransportWithMessage:
                        binding.Security.Mode = System.ServiceModel.SecurityMode.TransportWithMessageCredential;
                        binding.Security.Transport.Realm = "Hexa Core WebServices";
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                        binding.Security.Message.ClientCredentialType = MessageCredentialType.Certificate;
                        binding.Security.Message.NegotiateServiceCredential = false;
                        binding.Security.Message.EstablishSecurityContext = false;
                        break;
                    case Hexa.Core.ServiceModel.Security.SecurityMode.None:
                        binding.Security.Mode = System.ServiceModel.SecurityMode.None;
                        break;
                }
            }

			_Log.DebugFormat("wsHttpBinding binding is ready with Security Mode: {0}", _Configuration.SecurityMode);

			return binding;
		}

		protected BasicHttpBinding SetupBasicHttpBinding(ServiceHost host)
		{
			var binding = new BasicHttpBinding();
			binding.Name = "Basic"; 
			binding.MaxReceivedMessageSize = MaxTransferSize;
			binding.CloseTimeout = new TimeSpan(0, 0, 30);
			binding.MessageEncoding = WSMessageEncoding.Text; // TODO#28: Configurable..			
			binding.Namespace = host.Description.Namespace;
            
            if (_ExcludeHost(host))
            {
                binding.Security.Transport.Realm = "Hexa Core WebServices";
                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;

                switch (_Configuration.SecurityMode)
                {
                    case Hexa.Core.ServiceModel.Security.SecurityMode.Transport:
                        binding.Security.Mode = BasicHttpSecurityMode.Transport;
                        binding.TransferMode = TransferMode.Streamed; // TODO#28: Configurable..
                        binding.Security.Transport.Realm = "Hexa Core WebServices";
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                        break;
                    case Hexa.Core.ServiceModel.Security.SecurityMode.Message:
                        binding.Security.Mode = BasicHttpSecurityMode.Message;
                        binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;
                        break;
                    case Hexa.Core.ServiceModel.Security.SecurityMode.TransportWithMessage:
                        binding.Security.Mode = BasicHttpSecurityMode.TransportWithMessageCredential;
                        binding.Security.Transport.Realm = "Hexa Core WebServices";
                        binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
                        binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.Certificate;
                        break;
                    case Hexa.Core.ServiceModel.Security.SecurityMode.None:
                        binding.Security.Mode = BasicHttpSecurityMode.None;
                        break;
                }
            }
			_Log.DebugFormat("BasicHttpBinding binding is ready with Security Mode: {0}", _Configuration.SecurityMode);

			return binding;
		}

        protected static void SetupClientCredentials(ServiceCredentials behavior)
		{
            var serviceCredentials = ((ServiceCredentials)behavior);

            serviceCredentials.ClientCertificate.Authentication.CertificateValidationMode =
				System.ServiceModel.Security.X509CertificateValidationMode.None;

            serviceCredentials.ClientCertificate.Authentication.RevocationMode =
				System.Security.Cryptography.X509Certificates.X509RevocationMode.NoCheck;
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes")]
        protected void SetupServerCredentials(ServiceCredentials behavior)
		{
			var cfg = _Configuration.ServiceCredentials;

			if (_Configuration.SecurityMode != Hexa.Core.ServiceModel.Security.SecurityMode.Transport && cfg == null)
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
			var policies = ServiceLocator.GetAllInstances<IAuthorizationPolicy>().ToArray();

            if (policies.Length == 0)
                _Log.Warn("No authorization policies are in place.");
			
			var authBehavior = host.Description.Behaviors.Find<ServiceAuthorizationBehavior>();

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
        protected override ServiceHost CreateServiceHost(Type serviceType, Uri[] baseAddresses)
		{
			var host = default(ServiceHost);
            var binding1 = default(WSHttpBinding);
            var binding2 = default(BasicHttpBinding);
            var hasContract = false;

            _Log.DebugFormat("Type: {0}", serviceType.ToString());

            host = new ServiceHost(serviceType, baseAddresses);
			binding1 = SetupWSHttpBinding(host);
			binding2 = SetupBasicHttpBinding(host);

			// Add each service endpoint to service host!
            foreach (Type iface in serviceType.GetInterfaces())
			{
				var attr = (ServiceContractAttribute)Attribute.GetCustomAttribute(iface, typeof(ServiceContractAttribute));

				if (attr != null)
				{
					host.AddServiceEndpoint(iface, binding1, "");
					ServiceEndpoint e = host.AddServiceEndpoint(iface, binding2, binding2.Name);

					// This line causes that the proxy generates ProtectionLevel = None for operations.
					e.Contract.ProtectionLevel = System.Net.Security.ProtectionLevel.None;

					hasContract = true;
				}
			}

			if (!hasContract)
				throw new InvalidOperationException("WebService class does not have a ServiceContract attribute declared!");

            if (!_ExcludeHost(host))
            {
                SetupServiceCredentials(host); // Setup ServiceHost.Credentials
                SetupDebugBehavior(host); // Setup includeDebugInFaults, etc.
                SetupServiceAuthorization(host); // Setup ServiceAuthorizationBehavior..
            }

			var metadataBehavior = new ServiceMetadataBehavior();
            var mexbinding = new WSHttpBinding();

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

        private bool _ExcludeHost(ServiceHost host)
        {
            foreach (var srv in host.BaseAddresses)
            {
                foreach (Url excluded in _Configuration.ExcludedServices)
                {
                    if (srv.AbsolutePath.ToLower().Contains(excluded.Name.ToLower()))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
	}
}