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

/// <summary>
///
/// </summary>
    public class SeoUrlInfo
    {
        private string _key;
        private string _url;
        private string _changeFrequency;
        private string _priority;

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

            _key = key;
            _url = url;
            _changeFrequency = changeFrequency;
            _priority = ((double)priorityPercentage / 100).ToString();
        }

        /// <summary>
        ///
        /// </summary>
        public string Url
        {
            get
                {
                    return _url;
                }
            set
                {
                    _url = value;
                }
        }

        /// <summary>
        ///
        /// </summary>
        public string Key
        {
            get
                {
                    return _key;
                }
            set
                {
                    _key = value;
                }
        }

        /// <summary>
        ///
        /// </summary>
        public string ChangeFrequency
        {
            get
                {
                    return _changeFrequency;
                }
            set
                {
                    _changeFrequency = value;
                }
        }

        /// <summary>
        ///
        /// </summary>
        public string Priority
        {
            get
                {
                    return _priority;
                }
            set
                {
                    _priority = value;
                }
        }
    }

    public interface ISeoSiteMapBuilderService
    {
        void AddUrl(SeoUrlInfo url);
        void AddUrl(SeoUrlInfo url, SeoUrlInfo parent);
        void AddUrl(SeoUrlInfo url, SeoUrlInfo parent, int preferredDisplayOrder);
        void AddUrl(SeoUrlInfo url, int preferredDisplayOrder);
        System.Collections.ObjectModel.ReadOnlyCollection<SeoUrlInfo> GetChildren(string urlKey);
        SeoUrlInfo RootUrl
        {
            get;
        }
        string SeoXml();
    }
}

