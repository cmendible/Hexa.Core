namespace Hexa.Core.Domain
{
    using NHibernate;

    public class TransactionWrapper : ITransactionWrapper
    {
        #region Constructors

        public TransactionWrapper(ITransaction transaction)
        {
            this.Transaction = transaction;
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
            this.Transaction.Commit();
        }

        public void Rollback()
        {
            if (this.Transaction.WasRolledBack)
            {
                return;
            }

            this.Transaction.Rollback();
        }

        #endregion Methods
    }
}
