// -----------------------------------------------------------------------
// <copyright file="CustomAuthorizeAttribute.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------
namespace Hexa.Core.Web.Mvc.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    using Hexa.Core.Web.Security;

    public class CheckAuthorizationAttribute : AuthorizeAttribute
    {
        #region Methods

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //base.OnAuthorization(filterContext);

            // Try Get IAuthorizationService
            IAuthorizationService authorizationService = ServiceLocator.TryGetInstance<IAuthorizationService>();
            if (authorizationService != null)
            {
                // Get Action to Authorize
                string controllerName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.FullName;
                string actionName = filterContext.ActionDescriptor.ActionName;
                string ip = filterContext.HttpContext.Request.UserHostAddress;

                string actionToAuthorize = string.Format("{0}.{1}", controllerName, actionName);
                // If user is authorized return
                if (authorizationService.IsAuthorized(actionToAuthorize))
                {
                    return;
                }
            }
            else
            {
                // No IAuthorizationService is in place so return;
                return;
            }

            // The user is not allowed to execute the Action. An Unauthorized result is raised.
            filterContext.Result = new HttpUnauthorizedResult();
        }

        #endregion Methods
    }
}