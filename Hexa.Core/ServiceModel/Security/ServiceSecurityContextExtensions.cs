namespace Hexa.Core.ServiceModel
{
    using System.Collections.Generic;
    using System.IdentityModel.Policy;
    using System.Security.Principal;
    using System.ServiceModel;

    public static class ServiceSecurityContextExtensions
    {
        public static IList<IIdentity> GetIdentities(this ServiceSecurityContext context)
        {
            return GetIdentities(context.AuthorizationContext);
        }

        public static IIdentity GetPrimaryIdentity(this ServiceSecurityContext context)
        {
            IList<IIdentity> identities = GetIdentities(context.AuthorizationContext);
            if (identities.Count > 0)
            {
                return identities[0];
            }
            else
            {
                return new GenericIdentity(string.Empty);
            }
        }

        private static IList<IIdentity> GetIdentities(AuthorizationContext authorizationContext)
        {
            object list;
            if ((authorizationContext != null) && authorizationContext.Properties.TryGetValue("Identities", out list))
            {
                return list as IList<IIdentity>;
            }

            return new List<IIdentity>();
        }
    }
}