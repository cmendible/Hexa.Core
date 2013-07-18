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
namespace Hexa.Core.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    ///
    /// </summary>
    public class SeoSiteMapBuilderService : ISeoSiteMapBuilderService
    {
        #region Fields

        private readonly Dictionary<string, List<SeoUrlInfo>> _childurls;
        private readonly Dictionary<string, SeoUrlInfo> _keyIndex;
        private readonly SeoUrlInfo _rooturl;
        private readonly Dictionary<string, int> _urlPreferredOrder;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initialize a new instance of <see cref="SeoSiteMapBuilderService"/>.
        /// </summary>
        public SeoSiteMapBuilderService()
        {
            this._rooturl = new SeoUrlInfo("_ROOT_");
            this._keyIndex = new Dictionary<string, SeoUrlInfo>();
            this._childurls = new Dictionary<string, List<SeoUrlInfo>>();
            this._urlPreferredOrder = new Dictionary<string, int>();

            this._childurls.Add(this._rooturl.Key, new List<SeoUrlInfo>());
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the current root url.
        /// </summary>
        public SeoUrlInfo RootUrl
        {
            get
            {
                return this._rooturl;
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Adds a url as child of the root url.
        /// </summary>
        /// <param name="url">The url to add.</param>
        public void AddUrl(SeoUrlInfo url)
        {
            this.AddUrl(url, int.MaxValue);
            //this._childurls[Rooturl.Key].Add(url);
        }

        /// <summary>
        /// Adds a url as a child of the root url and sets the order with which the url should be displayed.
        /// </summary>
        /// <param name="url">The url to add.</param>
        /// <param name="preferredDisplayOrder">The url display order.</param>
        public void AddUrl(SeoUrlInfo url, int preferredDisplayOrder)
        {
            this.SafeAddurl(url);
            this.AddurlWithOrder(this.RootUrl.Key, url, preferredDisplayOrder);
        }

        /// <summary>
        /// Adds a url as a child of another url.
        /// </summary>
        /// <param name="url">The url to add.</param>
        /// <param name="parent">The url under which to add the new url.</param>
        public void AddUrl(SeoUrlInfo url, SeoUrlInfo parent)
        {
            this.AddUrl(url, parent, int.MaxValue);
        }

        /// <summary>
        /// Adds a url as a child of another url, and sets the order with which to display the url.
        /// </summary>
        /// <param name="url">The url to add.</param>
        /// <param name="parent">The url under which to add the new url.</param>
        /// <param name="preferredDisplayOrder">The url display order.</param>
        public void AddUrl(SeoUrlInfo url, SeoUrlInfo parent, int preferredDisplayOrder)
        {
            this.SafeAddurl(url);

            if (!this._childurls.ContainsKey(parent.Key))
            {
                this._childurls.Add(parent.Key, new List<SeoUrlInfo>());
            }

            this.AddurlWithOrder(parent.Key, url, preferredDisplayOrder);
        }

        /// <summary>
        /// Gets the children of the specified url.
        /// </summary>
        /// <param name="urlKey">The key of the parent url.</param>
        /// <returns>A <see cref="ReadOnlyCollection{T}"/> collection of the child urls.</returns>
        public ReadOnlyCollection<SeoUrlInfo> GetChildren(string urlKey)
        {
            if (this._childurls.ContainsKey(urlKey))
            {
                return this._childurls[urlKey].AsReadOnly();
            }

            return new List<SeoUrlInfo>().AsReadOnly();
        }

        public string SeoXml()
        {
            //instantiate the XML Text Writer for writing the SiteMap document
            using (StringWriter stringWriter = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(stringWriter))
                {
                    //write out the header
                    //start off the site map
                    writer.WriteProcessingInstruction("xml", "version=\"1.0\" encoding=\"UTF-8\"");
                    writer.WriteStartElement("urlset");
                    writer.WriteAttributeString("xmlns", "http://www.sitemaps.org/schemas/sitemap/0.9");
                    writer.WriteAttributeString("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
                    writer.WriteAttributeString("xsi:schemaLocation", "http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

                    SeoUrlInfo rooturl = this.RootUrl;
                    this.AddChildurls(writer, rooturl, this.GetChildren(rooturl.Key));

                    //write the footer and close.
                    writer.WriteEndElement();
                    return stringWriter.ToString();
                }
            }
        }

        internal static string FormatISODate(DateTime date)
        {
            var sb = new StringBuilder();
            sb.Append(date.Year);
            sb.Append("-");
            if (date.Month < 10)
            {
                sb.Append("0");
            }
            sb.Append(date.Month);
            sb.Append("-");
            if (date.Day < 10)
            {
                sb.Append("0");
            }
            sb.Append(date.Day);

            return sb.ToString();
        }

        internal static void WriteSiteMapurlEntry(XmlWriter writer, SeoUrlInfo url)
        {
            writer.WriteStartElement("url");
            byte[] locBytes;
            locBytes = Encoding.UTF8.GetBytes(url.Url);
            writer.WriteElementString("loc", Encoding.UTF8.GetString(locBytes));
            writer.WriteElementString("lastmod", FormatISODate(DateTime.Today));
            writer.WriteElementString("changefreq", url.ChangeFrequency);
            writer.WriteElementString("priority", url.Priority);
            writer.WriteEndElement();
        }

        private void AddChildurls(XmlWriter writer, SeoUrlInfo parent, ReadOnlyCollection<SeoUrlInfo> children)
        {
            if (children.Count > 0)
            {
                foreach (SeoUrlInfo info in children)
                {
                    WriteSiteMapurlEntry(writer, info);
                    this.AddChildurls(writer, info, this.GetChildren(info.Key));
                }
            }
        }

        private void AddurlWithOrder(string parentKey, SeoUrlInfo url, int preferredDisplayOrder)
        {
            this._urlPreferredOrder.Add(url.Key, preferredDisplayOrder);
            for (int i = 0; i < this._childurls[parentKey].Count; i++)
            {
                string key = this._childurls[parentKey][i].Key;
                if (this._urlPreferredOrder[key] > preferredDisplayOrder)
                {
                    this._childurls[parentKey].Insert(i, url);
                    return;
                }
            }

            this._childurls[parentKey].Add(url);
        }

        private void SafeAddurl(SeoUrlInfo url)
        {
            Guard.IsNotNull(url, "url");

            if (this._keyIndex.ContainsKey(url.Key))
            {
                throw new Exception("");
            }

            this._keyIndex.Add(url.Key, url);
        }

        #endregion Methods
    }
}