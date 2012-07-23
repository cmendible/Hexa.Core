namespace Hexa.Core.Domain
{
    using NHibernate;

    public class NestedTransactionWrapper : TransactionWrapper
    {
        #region Constructors

        public NestedTransactionWrapper(ITransaction transaction)
            : base(transaction)
        {
        }

        #endregion Constructors

        #region Methods

        public override void Commit()
        {
            // Do nothing, let the outermost transaction commit.
        }

        #endregion Methods
    }
}