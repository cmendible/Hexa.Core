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
namespace Hexa.Core.Web.Seo
{
    using System.Collections.ObjectModel;

    public interface ISeoSiteMapBuilderService
    {
        #region Properties

        SeoUrlInfo RootUrl
        {
            get;
        }

        #endregion Properties

        #region Methods

        void AddUrl(SeoUrlInfo url);

        void AddUrl(SeoUrlInfo url, SeoUrlInfo parent);

        void AddUrl(SeoUrlInfo url, SeoUrlInfo parent, int preferredDisplayOrder);

        void AddUrl(SeoUrlInfo url, int preferredDisplayOrder);

        ReadOnlyCollection<SeoUrlInfo> GetChildren(string urlKey);

        string SeoXml();

        #endregion Methods
    }

    /// <summary>
    ///
    /// </summary>
    public class SeoUrlInfo
    {
        #region Constructors

        public SeoUrlInfo(string key)
            : this(key, "daily", 100)
        {
        }

        public SeoUrlInfo(string key, string changeFrequency, int priorityPercentage)
            : this(key, key, changeFrequency, priorityPercentage)
        {
        }

        public SeoUrlInfo(string key, string url, string changeFrequency, int priorityPercentage)
        {
            Guard.IsNotNull(key, "key");
            Guard.IsNotNull(url, "url");

            this.Key = key;
            this.Url = url;
            this.ChangeFrequency = changeFrequency;
            this.Priority = ((double)priorityPercentage/100).ToString();
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        ///
        /// </summary>
        public string ChangeFrequency
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public string Key
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public string Priority
        {
            get;
            set;
        }

        /// <summary>
        ///
        /// </summary>
        public string Url
        {
            get;
            set;
        }

        #endregion Properties
    }
}