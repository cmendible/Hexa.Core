namespace Hexa.Core.Tests.Data
{
    using Core.Domain;
    using Domain;
    using Logging;

    public class HumanRepository : BaseRepository<Human>, IHumanRepository
    {
        public HumanRepository(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }
    }
}