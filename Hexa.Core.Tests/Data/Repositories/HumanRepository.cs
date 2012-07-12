namespace Hexa.Core.Tests.Data
{
    using System.ComponentModel.Composition;

    using Core.Domain;

    using Domain;

    using Logging;

    [Export(typeof(IHumanRepository))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HumanRepository : BaseRepository<Human>, IHumanRepository
    {
        #region Constructors

        [ImportingConstructor]
        public HumanRepository(ILoggerFactory loggerFactory)
            : base(loggerFactory)
        {
        }

        #endregion Constructors
    }
}
