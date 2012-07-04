namespace Hexa.Core.Tests.Data
{
    using Core.Domain;

    using Domain;

    using Logging;

    public class HumanRepository : BaseRepository<Human>, IHumanRepository
    {
        #region Constructors

        public HumanRepository(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        #endregion Constructors
    }
}