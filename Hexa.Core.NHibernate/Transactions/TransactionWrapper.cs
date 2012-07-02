
namespace Hexa.Core.Domain
{
    public class TransactionWrapper : ITransactionWrapper
    {
        public TransactionWrapper(global::NHibernate.ITransaction transaction)
        {
            Transaction = transaction;
        }

        protected global::NHibernate.ITransaction Transaction
        {
            get;
            set;
        }

        #region ITransaction Members

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
