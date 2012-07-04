namespace Hexa.Core.Domain
{
    using NHibernate;

    public class TransactionWrapper : ITransactionWrapper
    {
        #region Constructors

        public TransactionWrapper(ITransaction transaction)
        {
            Transaction = transaction;
        }

        #endregion Constructors

        #region Properties

        protected ITransaction Transaction
        {
            get;
            set;
        }

        #endregion Properties

        #region Methods

        public virtual void Commit()
        {
            Transaction.Commit();
        }

        public void Rollback()
        {
            if (Transaction.WasRolledBack)
            {
                return;
            }

            Transaction.Rollback();
        }

        #endregion Methods
    }
}