using System;
using System.ComponentModel;
using System.Configuration;
using Hexa.Core.ServiceModel.Security;

namespace Hexa.Core.ServiceModel
{

/// <summary>
///
/// </summary>
    public class Settings : ConfigurationSection
    {
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        internal static Settings Get()
        {
            try
                {
                    return ConfigurationManager.GetSection("Hexa.Core.ServiceModel.Settings") as Settings;
                }
            catch
                {
                    return null;
                }
        }

        /// <summary>
        /// Gets a boolean value indicating whether debug should be enabled or not.
        /// </summary>
        /// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
        [ConfigurationProperty("Debug", DefaultValue="true")]
        public bool Debug
        {
            get
                {
                    return (bool)this["Debug"];
                }
        }

        [ConfigurationProperty("SecurityMode", IsRequired = false, DefaultValue = SecurityMode.Message)]
        internal SecurityMode SecurityMode
        {
            get
                {
                    if (String.IsNullOrEmpty(this["SecurityMode"] as string))
                        return SecurityMode.Message;

                    return (SecurityMode)Enum.Parse(typeof(SecurityMode), this["SecurityMode"] as string);
                }
        }

        [ConfigurationProperty("ServiceCredentials", IsRequired = true)]
        internal ServiceCredentialsElement ServiceCredentials
        {
            get
                {
                    return this["ServiceCredentials"] as ServiceCredentialsElement;
                }
        }

        [ConfigurationProperty("ExcludedServices", IsRequired = false)]
        public ServicesCollection ExcludedServices
        {
            get
                {
                    return this["ExcludedServices"] as ServicesCollection;
                }
            set
                {
                    this["ExcludedServices"] = value;
                }
        }
    }

    public class ServicesCollection : ConfigurationElementCollection
    {
        public Url this[int index]
        {
            get
                {
                    return base.BaseGet(index) as Url;
                }
            set
                {
                    if (base.BaseGet(index) != null)
                        {
                            base.BaseRemoveAt(index);
                        }
                    this.BaseAdd(index, value);
                }
        }

        protected override void BaseAdd(ConfigurationElement element)
        {
            base.BaseAdd(element);
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new Url();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Url)element).Name;
        }

        public void Add(ConfigurationElement element)
        {
            this.BaseAdd(element);
        }

        public void Delete(string element)
        {
            this.BaseRemove(element);
        }
    }

    [TypeConverter(typeof(Url))]
    public class Url : ConfigurationElement
    {
        [ConfigurationProperty("Name")]
        public string Name
        {
            get
                {
                    return this["Name"] as string;
                }

            set
                {
                    this["Name"] = value;
                }
        }

    }
}
