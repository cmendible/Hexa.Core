namespace Hexa.Core.ServiceModel.Security
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IdentityModel.Claims;
    using System.IdentityModel.Policy;
    using System.Linq;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using System.Security.Principal;

    using log4net;

    /// <summary>
    ///
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors"
                    )]
    public abstract class BaseX509AuthorizationPolicy : BaseAuthorizationPolicy
    {
        #region Fields

        private static readonly ILog _Log = 
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseX509AuthorizationPolicy"/> class.
        /// </summary>
        public BaseX509AuthorizationPolicy()
        {
            _Log.DebugFormat("New instance {0} created.", Id);
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Evaluates whether a user meets the requirements for this authorization policy.
        /// </summary>
        /// <param name="evaluationContext">An <see cref="T:System.IdentityModel.Policy.EvaluationContext"/> that contains the claim set that the authorization policy evaluates.</param>
        /// <param name="state">A <see cref="T:System.Object"/>, passed by reference that represents the custom state for this authorization policy.</param>
        /// <returns>
        /// false if the <see cref="M:System.IdentityModel.Policy.IAuthorizationPolicy.Evaluate(System.IdentityModel.Policy.EvaluationContext,System.Object@)"/> method for this authorization policy must be called if additional claims are added by other authorization policies to <paramref name="evaluationContext"/>; otherwise, true to state no additional evaluation is required by this authorization policy.
        /// </returns>
        public override bool Evaluate(EvaluationContext evaluationContext, ref object state)
        {
            IList<IIdentity> identities = evaluationContext.TryGetIdentities();

            // Sleep no identities found yet.
            if (identities == null || identities.Count == 0)
            {
                _Log.Debug("identities == null or identities.Count == 0; sleeping..");
                return false;
            }

            // Sleep no identities of type X509.
            if (!identities.Any(i => i.AuthenticationType == "X509"))
            {
                _Log.Debug("No identity authenticated by X509 certificate; sleeping..");
                return false;
            }

            if (state == null)
            {
                state = 0;
            }
            else
            {
                state = (int)state + 1;
            }

            // Should not evaluate policy twice.
            if ((int)state > 0)
            {
                return true;
            }

            X509Certificate2 certificate = GetClientCertificate(evaluationContext);
            if (certificate == null)
            {
                _Log.Debug("No valid X509CertificateClaimSet was found.");
                return true;
            }

            IPrincipal principal = GetPrincipal(evaluationContext, certificate);
            if (principal == null)
            {
                _Log.Warn("User not authorized.");
                return true;
            }

            SetupEvaluationContext(evaluationContext, principal);

            return true;
        }

        /// <summary>
        /// Get client certificate.
        /// </summary>
        /// <param name="evaluationContext">The evaluation context.</param>
        /// <returns></returns>
        protected static X509Certificate2 GetClientCertificate(EvaluationContext evaluationContext)
        {
            X509CertificateClaimSet claimset = evaluationContext.ClaimSets
                                               .Where(cs => cs is X509CertificateClaimSet)
                                               .Cast<X509CertificateClaimSet>()
                                               .FirstOrDefault();

            if (claimset != null)
            {
                return claimset.X509Certificate;
            }

            return null;
        }

        protected abstract IPrincipal GetPrincipal(EvaluationContext evaluationContext, X509Certificate2 certificate);

        #endregion Methods
    }
}