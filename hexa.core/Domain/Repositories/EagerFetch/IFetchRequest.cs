using System.Linq;

namespace Hexa.Core.Domain
{
    public interface IFetchRequest<TQueried, TFetch> : IOrderedQueryable<TQueried>
    {
    }
}