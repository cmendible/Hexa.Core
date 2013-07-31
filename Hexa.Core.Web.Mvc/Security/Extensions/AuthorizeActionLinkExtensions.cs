namespace Hexa.Core.Web.Mvc.Extensions
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Mvc.Html;
    using System.Web.Routing;

    public static class AuthorizeActionLinkExtensions
    {
        public static MvcHtmlString AuthorizeActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            if (HasActionPermission(helper, actionName, controllerName))
            {
                return helper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
            }

            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString AuthorizeActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName)
        {
            if (HasActionPermission(helper, actionName, controllerName))
            {
                return helper.ActionLink(linkText, actionName, controllerName);
            }

            return MvcHtmlString.Empty;
        }

        public static MvcHtmlString AuthorizeActionLink(this HtmlHelper helper, string linkText, string actionName, string controllerName, RouteValueDictionary routeValues, IDictionary<string, object> htmlAttributes)
        {
            if (HasActionPermission(helper, actionName, controllerName))

            {
                return helper.ActionLink(linkText, actionName, controllerName, routeValues, htmlAttributes);
            }

            return MvcHtmlString.Empty;
        }

        private static bool ActionIsAuthorized(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            if (actionDescriptor == null)
            {
                return false;
            }

            AuthorizationContext authContext = new AuthorizationContext(controllerContext, actionDescriptor);
            foreach (Filter authFilter in FilterProviders.Providers.GetFilters(authContext, actionDescriptor))
            {
                if (authFilter.Instance is System.Web.Mvc.AuthorizeAttribute)
                {
                    ((IAuthorizationFilter)authFilter.Instance).OnAuthorization(authContext);

                    if (authContext.Result != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static ControllerBase GetControllerByName(HtmlHelper helper, string controllerName)
        {
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();

            IController controller = factory.CreateController(helper.ViewContext.RequestContext, controllerName);

            if (controller == null)
            {
                throw new System.InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentUICulture,
                        "Controller factory {0} controller {1} returned null",
                        factory.GetType(),
                        controllerName));
            }

            return (ControllerBase)controller;
        }

        private static bool HasActionPermission(this HtmlHelper htmlHelper, string actionName, string controllerName)
        {
            ControllerBase controllerToLinkTo = string.IsNullOrEmpty(controllerName)
                                                ? htmlHelper.ViewContext.Controller
                                                : GetControllerByName(htmlHelper, controllerName);

            ControllerContext controllerContext = new ControllerContext(htmlHelper.ViewContext.RequestContext, controllerToLinkTo);

            ReflectedControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controllerToLinkTo.GetType());
            ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(controllerContext, actionName);

            return ActionIsAuthorized(controllerContext, actionDescriptor);
        }
    }
}