using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexa.Core.Domain
{
    public class NestedTransactionWrapper : TransactionWrapper
    {
        public NestedTransactionWrapper(global::NHibernate.ITransaction transaction)
            : base(transaction)
        {
        }

        public override void Commit()
        {
            // Do nothing, let the outermost transaction commit.
        }
    }
}
