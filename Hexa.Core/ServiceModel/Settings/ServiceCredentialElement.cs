using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Hexa.Core.ServiceModel
{

    public sealed class ServiceCredentialsElement : ConfigurationElement
    {
        public ServiceCredentialsElement()
        {
        }

        [ConfigurationProperty("findValue", DefaultValue = ""), StringValidator(MinLength = 0)]
        public string FindValue
        {
            get
                {
                    return (string)base["findValue"];
                }
            set
                {
                    if (string.IsNullOrEmpty(value))
                        {
                            value = string.Empty;
                        }
                    base["findValue"] = value;
                }
        }

        [ConfigurationProperty("storeLocation", DefaultValue = StoreLocation.LocalMachine)]
        public System.Security.Cryptography.X509Certificates.StoreLocation StoreLocation
        {
            get
                {
                    return (System.Security.Cryptography.X509Certificates.StoreLocation)base["storeLocation"];
                }
            set
                {
                    base["storeLocation"] = value;
                }
        }

        [ConfigurationProperty("storeName", DefaultValue = StoreName.My)]
        public System.Security.Cryptography.X509Certificates.StoreName StoreName
        {
            get
                {
                    return (System.Security.Cryptography.X509Certificates.StoreName)base["storeName"];
                }
            set
                {
                    base["storeName"] = value;
                }
        }

        [ConfigurationProperty("x509FindType", DefaultValue = X509FindType.FindByFile)]
        public X509FindType X509FindType
        {
            get
                {
                    return (X509FindType)base["x509FindType"];
                }
            set
                {
                    base["x509FindType"] = value;
                }
        }
    }
}
