
using Hexa.Core.Domain;
using Hexa.Core.Tests.Domain;

namespace Hexa.Core.Tests.Data
{
    public class HumanMap : AuditableRootEntityMap<Human>
    {
        public HumanMap()
        {
            Map(h => h.Name);

            Map(h => h.isMale);
        }
    }
}
