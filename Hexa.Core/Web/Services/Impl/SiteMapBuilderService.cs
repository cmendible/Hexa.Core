namespace Hexa.Core.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;

    using Hexa.Core;

    public class SiteMapBuilderService : ISiteMapBuilderService
    {
        #region Fields

        // Fields
        private Dictionary<string, List<SiteMapNodeInfo>> _childNodes = new Dictionary<string, List<SiteMapNodeInfo>>();
        private Dictionary<string, SiteMapNodeInfo> _keyIndex = new Dictionary<string, SiteMapNodeInfo>();
        private Dictionary<string, string> _nodeAuthorization = new Dictionary<string, string>();
        private Dictionary<string, int> _nodePreferredOrder = new Dictionary<string, int>();
        private SiteMapNodeInfo _rootNode = new SiteMapNodeInfo("__ROOT__");

        #endregion Fields

        #region Constructors

        // Methods
        public SiteMapBuilderService()
        {
            this._childNodes.Add(this._rootNode.Key, new List<SiteMapNodeInfo>());
        }

        #endregion Constructors

        #region Properties

        // Properties
        public SiteMapNodeInfo RootNode
        {
            get
            {
                return this._rootNode;
            }
        }

        #endregion Properties

        #region Methods

        public void AddNode(SiteMapNodeInfo node)
        {
            this.AddNode(node, 0x7fffffff);
        }

        public void AddNode(SiteMapNodeInfo node, SiteMapNodeInfo parent)
        {
            this.AddNode(node, parent, 0x7fffffff);
        }

        public void AddNode(SiteMapNodeInfo node, int preferredDisplayOrder)
        {
            this.SafeAddNode(node);
            this.AddNodeWithOrder(this.RootNode.Key, node, preferredDisplayOrder);
        }

        public void AddNode(SiteMapNodeInfo node, string authorizationRule)
        {
            this.AddNode(node, authorizationRule, 0x7fffffff);
        }

        public void AddNode(SiteMapNodeInfo node, SiteMapNodeInfo parent, int preferredDisplayOrder)
        {
            this.SafeAddNode(node);
            if (!this._childNodes.ContainsKey(parent.Key))
            {
                this._childNodes.Add(parent.Key, new List<SiteMapNodeInfo>());
            }
            this.AddNodeWithOrder(parent.Key, node, preferredDisplayOrder);
        }

        public void AddNode(SiteMapNodeInfo node, SiteMapNodeInfo parent, string authorizationRule)
        {
            this.AddNode(node, parent, authorizationRule, 0x7fffffff);
        }

        public void AddNode(SiteMapNodeInfo node, string authorizationRule, int preferredDisplayOrder)
        {
            Guard.IsNotNullNorEmpty(authorizationRule, "authorizationRule");
            this.AddNode(node, preferredDisplayOrder);
            this._nodeAuthorization.Add(node.Key, authorizationRule);
        }

        public void AddNode(SiteMapNodeInfo node, SiteMapNodeInfo parent, string authorizationRule, int preferredDisplayOrder)
        {
            this.AddNode(node, parent, preferredDisplayOrder);
            this._nodeAuthorization.Add(node.Key, authorizationRule);
        }

        public string GetAuthorizationRule(string nodeKey)
        {
            if (this._nodeAuthorization.ContainsKey(nodeKey))
            {
                return this._nodeAuthorization[nodeKey];
            }
            return null;
        }

        public ReadOnlyCollection<SiteMapNodeInfo> GetChildren(string nodeKey)
        {
            if (this._childNodes.ContainsKey(nodeKey))
            {
                return this._childNodes[nodeKey].AsReadOnly();
            }
            return new List<SiteMapNodeInfo>().AsReadOnly();
        }

        private void AddNodeWithOrder(string parentKey, SiteMapNodeInfo node, int preferredDisplayOrder)
        {
            this._nodePreferredOrder.Add(node.Key, preferredDisplayOrder);
            for (int i = 0; i < this._childNodes[parentKey].Count; i++)
            {
                string key = this._childNodes[parentKey][i].Key;
                if (this._nodePreferredOrder[key] > preferredDisplayOrder)
                {
                    this._childNodes[parentKey].Insert(i, node);
                    return;
                }
            }
            this._childNodes[parentKey].Add(node);
        }

        private void SafeAddNode(SiteMapNodeInfo node)
        {
            Guard.IsNotNull(node, "node");
            if (this._keyIndex.ContainsKey(node.Key))
            {
                throw new ApplicationException("Duplicate key");
            }
            this._keyIndex.Add(node.Key, node);
        }

        #endregion Methods
    }
}