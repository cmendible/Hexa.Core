namespace Hexa.Core.Web.SiteMap
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;

    using Hexa.Core;

    public class SiteMapNodeInfo
    {
        #region Fields

        // Fields
        private NameValueCollection _attributes;
        private string _description;
        private NameValueCollection _explicitResourcesKey;
        private string _implicitResourceKey;
        private string _key;
        private IList _roles;
        private string _title;
        private string _url;

        #endregion Fields

        #region Constructors

        // Methods
        public SiteMapNodeInfo(string key)
        : this(key, null, null, null, null, null, null, null)
        {
        }

        public SiteMapNodeInfo(string key, string url)
        : this(key, url, null, null, null, null, null, null)
        {
        }

        public SiteMapNodeInfo(string key, string url, string title)
        : this(key, url, title, null, null, null, null, null)
        {
        }

        public SiteMapNodeInfo(string key, string url, string title, string description)
        : this(key, url, title, description, null, null, null, null)
        {
        }

        public SiteMapNodeInfo(string key, string url, string title, string description, IList roles, NameValueCollection attributes, NameValueCollection explicitResourceKeys, string implicitResourceKey)
        {
            Guard.IsNotNull(key, "key");
            this._key = key;
            this._url = url;
            this._title = title;
            this._description = description;
            this._roles = roles;
            this._attributes = attributes;
            this._explicitResourcesKey = explicitResourceKeys;
            this._implicitResourceKey = implicitResourceKey;
        }

        public SiteMapNodeInfo(string key, string url, Func<string> title, Func<string> description)
        : this(key, url, title.Invoke(), description.Invoke())
        {
            TitleCallBack = title;
            DescriptionCallBack = description;
        }

        #endregion Constructors

        #region Properties

        // Properties
        public NameValueCollection Attributes
        {
            get
            {
                return this._attributes;
            }
        }

        public string Description
        {
            get
            {
                return this._description;
            }
            set
            {
                this._description = value;
            }
        }

        public Func<string> DescriptionCallBack
        {
            get;
            protected set;
        }

        public NameValueCollection ExplicitResourcesKey
        {
            get
            {
                return this._explicitResourcesKey;
            }
        }

        public string ImplicitResourceKey
        {
            get
            {
                return this._implicitResourceKey;
            }
            set
            {
                this._implicitResourceKey = value;
            }
        }

        public string Key
        {
            get
            {
                return this._key;
            }
            set
            {
                this._key = value;
            }
        }

        public IList Roles
        {
            get
            {
                return this._roles;
            }
        }

        public string Title
        {
            get
            {
                return this._title;
            }
            set
            {
                this._title = value;
            }
        }

        public Func<string> TitleCallBack
        {
            get;
            protected set;
        }

        public string Url
        {
            get
            {
                return this._url;
            }
            set
            {
                this._url = value;
            }
        }

        #endregion Properties
    }
}