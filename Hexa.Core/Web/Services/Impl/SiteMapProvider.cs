namespace Hexa.Core.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Web;

    public class SiteMapProvider : StaticSiteMapProvider
    {
        #region Fields

        // Fields
        private IAuthorizationService _authorizationService;
        private bool _isInitialized;
        private object _lockObject = new object();
        private SiteMapNode _rootNode;
        private ISiteMapBuilderService _siteMapBuilder;

        #endregion Fields

        #region Properties

        // Properties
        public IAuthorizationService AuthorizationService
        {
            get
            {
                if (this._authorizationService == null)
                {
                    this._authorizationService = ServiceLocator.GetInstance<IAuthorizationService>();
                }

                return this._authorizationService;
            }
            set
            {
                this._authorizationService = value;
            }
        }

        public ISiteMapBuilderService SiteMapBuilder
        {
            get
            {
                if (this._siteMapBuilder == null)
                {
                    this._siteMapBuilder = ServiceLocator.GetInstance<ISiteMapBuilderService>();
                }

                return this._siteMapBuilder;
            }
            set
            {
                this._siteMapBuilder = value;
            }
        }

        #endregion Properties

        #region Methods

        public override SiteMapNode BuildSiteMap()
        {
            if (!this._isInitialized)
            {
                lock (this._lockObject)
                {
                    if (!this._isInitialized)
                    {
                        SiteMapNodeInfo rootNode = this.SiteMapBuilder.RootNode;
                        this._rootNode = this.CreateSiteMapNode(rootNode);
                        this.AddChildNodes(this._rootNode, this.SiteMapBuilder.GetChildren(rootNode.Key));
                        this._isInitialized = true;
                    }
                }
            }
            return this._rootNode;
        }

        public override bool IsAccessibleToUser(HttpContext context, SiteMapNode node)
        {
            return this.IsAccessibleToUser(node);
        }

        protected override SiteMapNode GetRootNodeCore()
        {
            return this.BuildSiteMap();
        }

        protected virtual bool IsAccessibleToUser(SiteMapNode node)
        {
            bool flag = true;
            if (this.AuthorizationService != null)
            {
                string authorizationRule = this.SiteMapBuilder.GetAuthorizationRule(node.Key);
                if (authorizationRule != null)
                {
                    flag = this.AuthorizationService.IsAuthorized(authorizationRule);
                }
                else
                {
                    flag = this.AuthorizationService.IsAuthorized(node.Key);    // If no auth rule was defined then use node.Key as rule name.
                }
            }
            return flag;
        }

        // Methods
        private void AddChildNodes(SiteMapNode parent, ReadOnlyCollection<SiteMapNodeInfo> children)
        {
            if (children.Count > 0)
            {
                foreach (SiteMapNodeInfo info in children)
                {
                    SiteMapNode node = this.CreateSiteMapNode(info);
                    this.AddNode(node, parent);
                    this.AddChildNodes(node, this.SiteMapBuilder.GetChildren(info.Key));
                }
            }
        }

        private SiteMapNode CreateSiteMapNode(SiteMapNodeInfo nodeInfo)
        {
            return new SiteMapNode(this, nodeInfo.Key, nodeInfo.Url, nodeInfo.Title,
                                   nodeInfo.Description, nodeInfo.Roles, nodeInfo.Attributes,
                                   nodeInfo.ExplicitResourcesKey, nodeInfo.ImplicitResourceKey
                                  );
        }

        #endregion Methods
    }
}