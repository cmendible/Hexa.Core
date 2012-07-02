namespace Hexa.Core.Domain
{
    public interface ITransactionWrapper
    {
        void Commit();
        void Rollback();
    }
}