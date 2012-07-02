namespace Hexa.Core.Domain
{
/// <summary>
/// Base Aggregated Entity class.
/// </summary>
/// <remarks>
/// This class derives from NCommon.DomainModel.BaseEntityWithId, and explicitly
/// implements NCommon.Validation.IValidatable.
/// </remarks>
    public class Entity<TEntity> : BaseEntityWithId<TEntity>
    {

    }
}

