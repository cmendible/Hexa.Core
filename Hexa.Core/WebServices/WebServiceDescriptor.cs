using System;

namespace Hexa.Core.WebServices
{

	public class WebServiceDescriptor
	{
		internal string AbsoluteUri;
		internal Type Type;
		internal IoCContainer Container;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceDescriptor"/> class.
		/// </summary>
		/// <param name="owner">The ICoreContainer of this web service</param>
		/// <param name="Name">The service name.</param>
		/// <param name="Type">The service implementor's type.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#")]
        public WebServiceDescriptor(IoCContainer container, string uri, Type type)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			if (string.IsNullOrEmpty(uri))
				throw new ArgumentNullException("uri");

			if (type == null)
				throw new ArgumentNullException("type");

			this.AbsoluteUri = uri;
			this.Type = type;
			this.Container = container;
		}
	}
}