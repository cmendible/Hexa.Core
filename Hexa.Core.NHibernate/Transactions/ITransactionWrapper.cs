namespace Hexa.Core.Domain
{
    public interface ITransactionWrapper
    {
        #region Methods

        void Commit();

        void Rollback();

        #endregion Methods
    }
}
