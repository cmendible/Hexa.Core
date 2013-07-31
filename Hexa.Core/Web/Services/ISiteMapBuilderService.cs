namespace Hexa.Core.Web.Services
{
    using System.Collections.ObjectModel;

    public interface ISiteMapBuilderService
    {
        // Properties
        SiteMapNodeInfo RootNode
        {
            get;
        }

        // Methods
        void AddNode(SiteMapNodeInfo node);

        void AddNode(SiteMapNodeInfo node, SiteMapNodeInfo parent);

        void AddNode(SiteMapNodeInfo node, int preferredDisplayOrder);

        void AddNode(SiteMapNodeInfo node, string authorizationRule);

        void AddNode(SiteMapNodeInfo node, SiteMapNodeInfo parent, int preferredDisplayOrder);

        void AddNode(SiteMapNodeInfo node, SiteMapNodeInfo parent, string authorizationRule);

        void AddNode(SiteMapNodeInfo node, string authorizationRule, int preferredDisplayOrder);

        void AddNode(SiteMapNodeInfo node, SiteMapNodeInfo parent, string authorizationRule, int preferredDisplayOrder);

        string GetAuthorizationRule(string nodeKey);

        ReadOnlyCollection<SiteMapNodeInfo> GetChildren(string nodeKey);
    }
}