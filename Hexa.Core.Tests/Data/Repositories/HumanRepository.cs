
using Hexa.Core.Domain;
using Hexa.Core;
using Hexa.Core.Tests.Domain;

namespace Hexa.Core.Tests.Data
{
    public class HumanRepository : BaseRepository<Human>, IHumanRepository
    {
        public HumanRepository(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }
    }
}
