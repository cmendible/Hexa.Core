using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Hosting;

namespace Hexa.Core.WebServices
{
	/// <summary>
	/// Manager (singleton) of any published Web Services.
	/// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public sealed class WebServicesManager : IWebServicesManager
	{
		private Dictionary<string, WebServiceDescriptor> _Services = new Dictionary<string, WebServiceDescriptor>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.Type.InvokeMember(System.String,System.Reflection.BindingFlags,System.Reflection.Binder,System.Object,System.Object[])"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Type.InvokeMember")]
        public WebServicesManager()
		{
			// TODO: Move this to a Service registered at RootContainer??

			VirtualPathProvider providerInstance = new VirtualPathProvider(this);
			// any settings about your VirtualPathProvider may go here.

			// we get the current instance of HostingEnvironment class. We can't create a new one
			// because it is not allowed to do so. An AppDomain can only have one HostingEnvironment
			// instance.
			HostingEnvironment hostingEnvironmentInstance = (HostingEnvironment)typeof(HostingEnvironment).InvokeMember("_theHostingEnvironment", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField, null, null, null);
			if (hostingEnvironmentInstance == null)
				return;

			// we get the MethodInfo for RegisterVirtualPathProviderInternal method which is internal
			// and also static.
			MethodInfo mi = typeof(HostingEnvironment).GetMethod("RegisterVirtualPathProviderInternal", BindingFlags.NonPublic | BindingFlags.Static);
			if (mi == null)
				return;

			// finally we invoke RegisterVirtualPathProviderInternal method with one argument which
			// is the instance of our own VirtualPathProvider.
			mi.Invoke(hostingEnvironmentInstance, new object[] { (VirtualPathProvider)providerInstance });

			//VirtualPathProvider provider = new VirtualPathProvider(this);
			//System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(provider);
		}

		#region IEnumerable...

		IEnumerator<WebServiceDescriptor> IEnumerable<WebServiceDescriptor>.GetEnumerator()
		{
			return _Services.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _Services.Values.GetEnumerator();
		}

		public IEnumerator<KeyValuePair<string, WebServiceDescriptor>> GetEnumerator()
		{
			return _Services.GetEnumerator();
		}

		#endregion

		/// <summary>
		/// Registers a new WebService object
		/// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.StartsWith(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.EndsWith(System.String)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        public void Register(WebServiceDescriptor descriptor)
        {
            if (!descriptor.AbsoluteUri.StartsWith("/"))
				throw new ArgumentException("Service Uri does not starts with /");

            if (!descriptor.AbsoluteUri.ToLower().EndsWith(".svc"))
				throw new ArgumentException("Service name does not ends with .svc");

            _Services.Add(descriptor.AbsoluteUri.ToLower(), descriptor);
		}

		/// <summary>
		/// De-registers a WebService
		/// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        public void Deregister(string uri) {
			if (!_Services.Remove(uri.ToLower()))
				throw new ArgumentOutOfRangeException(uri);
		}

		/// <summary>
		/// Determines whether the specified URI is a registered web service
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns>
		/// 	<c>true</c> if the specified URI is registered; otherwise, <c>false</c>.
		/// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        public bool IsRegistered(string uri)
		{
			return _Services.ContainsKey(uri.ToLower());
		}

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1304:SpecifyCultureInfo", MessageId = "System.String.ToLower")]
        public bool TryGet(string key, out WebServiceDescriptor descriptor)
		{
            return _Services.TryGetValue(key.ToLower(), out descriptor);
		}
	}
}
