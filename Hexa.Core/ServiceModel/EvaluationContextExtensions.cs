using System.Collections.Generic;
using System.IdentityModel.Policy;
using System.Security.Principal;

namespace Hexa.Core.ServiceModel
{
	public static class EvaluationContextExtensions
	{
		public static IIdentity GetPrimaryIdentity(this EvaluationContext context)
		{
			IList<IIdentity> identities = GetIdentities(context);
			if (identities != null && identities.Count > 0)
			{
				return identities[0];
			}
			return null;
		}

		public static IList<IIdentity> TryGetIdentities(this EvaluationContext context)
		{
			object list;
			if ((context != null) && context.Properties.TryGetValue("Identities", out list))
			{
				return (list as IList<IIdentity>);
			}

			return null;
		}

		public static IList<IIdentity> GetIdentities(this EvaluationContext context)
		{
			IList<IIdentity> list = TryGetIdentities(context);
			if (list == null)
				context.Properties["Identities"] = list = new List<IIdentity>();

			return list;
		}
	}
}
