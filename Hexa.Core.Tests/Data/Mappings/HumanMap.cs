namespace Hexa.Core.Tests.Data
{
    using Core.Domain;

    using Domain;

    public class HumanMap : AuditableRootEntityMap<Human>
    {
        #region Constructors

        public HumanMap()
        {
            Map(h => h.Name);

            Map(h => h.isMale);
        }

        #endregion Constructors
    }
}