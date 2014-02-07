namespace Hexa.Core.Tests.Unity
{
    using Hexa.Core.Domain;
    using Microsoft.Practices.Unity;

    /// <summary>
    /// UnitOfWork Per Test LifeTimeManager
    /// </summary>
    public class UnitOfWorkPerTestLifeTimeManager : LifetimeManager
    {
        IUnitOfWork unitOfWork;

        /// <summary>
        /// Retrieve a value from the backing store associated with this Lifetime policy.
        /// </summary>
        /// <returns>
        /// the object desired, or null if no such object is currently stored.
        /// </returns>
        public override object GetValue()
        {
            return unitOfWork;
        }

        /// <summary>
        /// Remove the given object from backing store.
        /// </summary>
        public override void RemoveValue()
        {
            unitOfWork = null;
        }

        /// <summary>
        /// Stores the given value into backing store for retrieval later.
        /// </summary>
        /// <param name="newValue">The object being stored.</param>
        public override void SetValue(object newValue)
        {
            unitOfWork = newValue as IUnitOfWork;
        }
    }
}
