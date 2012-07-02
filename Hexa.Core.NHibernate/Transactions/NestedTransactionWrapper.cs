namespace Hexa.Core.Domain
{
    using NHibernate;

    public class NestedTransactionWrapper : TransactionWrapper
    {
        public NestedTransactionWrapper(ITransaction transaction)
            : base(transaction)
        {
        }

        public override void Commit()
        {
            // Do nothing, let the outermost transaction commit.
        }
    }
}