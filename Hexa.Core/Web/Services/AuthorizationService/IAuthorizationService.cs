namespace Hexa.Core.Web.Security
{
    using System.Collections.Generic;

    public interface IAuthorizationService
    {
        #region Methods

        string[] GetAuthorizationRules(string urlPath);

        bool IsAuthorized(string rule);

        bool IsAuthorized(string role, string rule);

        void RegisterAuthorizationRule(string urlPath, string rule);

        #endregion Methods
    }
}