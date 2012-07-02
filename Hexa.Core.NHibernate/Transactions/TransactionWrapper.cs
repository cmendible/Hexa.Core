namespace Hexa.Core.Domain
{
    using NHibernate;

    public class TransactionWrapper : ITransactionWrapper
    {
        public TransactionWrapper(ITransaction transaction)
        {
            Transaction = transaction;
        }

        protected ITransaction Transaction { get; set; }

        #region ITransactionWrapper Members

        public virtual void Commit()
        {
            Transaction.Commit();
        }

        public void Rollback()
        {
            if (Transaction.WasRolledBack)
                return;

            Transaction.Rollback();
        }

        #endregion
    }
}