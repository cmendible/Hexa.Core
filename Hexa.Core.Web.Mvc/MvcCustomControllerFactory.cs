namespace Hexa.Core.Web.Mvc
{
    using System;
    using System.Web.Mvc;

    /// <summary>
    /// Custom Controller Factory so we can use dependency injection in the ASP.Net MVC based UI
    /// Don't forget to reference System.Web.Routing or class won't build
    /// </summary>
    public class MvcCustomControllerFactory : DefaultControllerFactory
    {
        /// <summary>
        /// Retrieves the controller instance for the specified request context and controller type.
        /// </summary>
        /// <param name="requestContext">The context of the HTTP request, which includes the HTTP context and route data.</param>
        /// <param name="controllerType">The type of the controller.</param>
        /// <returns>The controller instance.</returns>
        /// <exception cref="T:System.Web.HttpException">
        ///     <paramref name="controllerType"/> is null.</exception>
        /// <exception cref="T:System.ArgumentException">
        ///     <paramref name="controllerType"/> cannot be assigned.</exception>
        /// <exception cref="T:System.InvalidOperationException">An instance of <paramref name="controllerType"/> cannot be created.</exception>
        protected override IController GetControllerInstance(System.Web.Routing.RequestContext requestContext, Type controllerType)
        {
            if (controllerType != null)
            {
                return ServiceLocator.GetInstance(controllerType) as IController;
            }
            else
            {
                return null;
            }
        }
    }
}