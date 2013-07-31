namespace Hexa.Core.Web.Services
{
    using System.Collections.Generic;

    public interface IAuthorizationService
    {
        string[] GetAuthorizationRules(string urlPath);

        bool IsAuthorized(string action);

        bool IsAuthorized(string role, string action);

        void RegisterAuthorizationRule(string urlPath, string action);
    }
}