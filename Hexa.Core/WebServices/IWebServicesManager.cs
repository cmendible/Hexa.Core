using System.Collections.Generic;

namespace Hexa.Core.WebServices
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public interface IWebServicesManager : IEnumerable<WebServiceDescriptor>, IEnumerable<KeyValuePair<string, WebServiceDescriptor>>
	{
        void Register(WebServiceDescriptor descriptor);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        void Deregister(string uri);
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "0#")]
        bool IsRegistered(string uri);
		bool TryGet(string key, out WebServiceDescriptor descriptor);
	}
}
