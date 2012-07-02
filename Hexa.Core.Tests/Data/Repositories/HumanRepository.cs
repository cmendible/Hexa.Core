
using Hexa.Core.Domain;
using Hexa.Core.Logging;
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
