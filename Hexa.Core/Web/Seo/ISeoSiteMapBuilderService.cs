//===================================================================================
//Copyright 2010 HexaSystems Corporation
//===================================================================================
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//http://www.apache.org/licenses/LICENSE-2.0
//===================================================================================
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
//===================================================================================

namespace Hexa.Core.Web.Seo
{

    /// <summary>
    /// Holds information for a <see cref="System.Web.SiteMapurl"/> published by a module.
    /// </summary>
    public class SeoUrlInfo
    {
        private string _key;
        private string _url;

        public SeoUrlInfo(string key)
            : this(key, key)
        {
        }

        public SeoUrlInfo(string key, string url)
        {
            //Guard.ArgumentNotNull(key, "key");

            _key = key;
            _url = url;
        }

        /// <summary>
        /// Gets or sets the url of the page represented by the url.
        /// </summary>  
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>
        /// Gets or sets the lookup key for the url.
        /// </summary>
        public string Key
        {
            get { return _key; }
            set { _key = value; }
        }
    }

    public interface ISeoSiteMapBuilderService
    {
        void AddUrl(SeoUrlInfo url);
        void AddUrl(SeoUrlInfo url, SeoUrlInfo parent);
        void AddUrl(SeoUrlInfo url, SeoUrlInfo parent, int preferredDisplayOrder);
        void AddUrl(SeoUrlInfo url, int preferredDisplayOrder);
        System.Collections.ObjectModel.ReadOnlyCollection<SeoUrlInfo> GetChildren(string urlKey);
        SeoUrlInfo RootUrl { get; }
        string SeoXml();
    }
}
