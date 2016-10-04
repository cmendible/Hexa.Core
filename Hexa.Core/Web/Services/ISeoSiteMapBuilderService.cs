//----------------------------------------------------------------------------------------------
// <copyright file="ISeoSiteMapBuilderService.cs" company="HexaSystems Inc">
// Copyright (c) HexaSystems Inc. Licensed under the Apache License, Version 2.0 (the "License")
// </copyright>
//-----------------------------------------------------------------------------------------------
namespace Hexa.Core.Web.Services
{
    using System.Collections.ObjectModel;

    public interface ISeoSiteMapBuilderService
    {
        SeoUrlInfo RootUrl
        {
            get;
        }

        void AddUrl(SeoUrlInfo url);

        void AddUrl(SeoUrlInfo url, SeoUrlInfo parent);

        void AddUrl(SeoUrlInfo url, SeoUrlInfo parent, int preferredDisplayOrder);

        void AddUrl(SeoUrlInfo url, int preferredDisplayOrder);

        ReadOnlyCollection<SeoUrlInfo> GetChildren(string urlKey);

        string SeoXml();
    }

    /// <summary>
    ///
    /// </summary>
    public class SeoUrlInfo
    {
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
            this.Priority = ((double)priorityPercentage / 100).ToString();
        }

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
    }
}