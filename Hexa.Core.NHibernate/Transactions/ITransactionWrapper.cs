using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexa.Core.Domain
{
    public interface ITransactionWrapper
    {
        void Commit();
        void Rollback();
    }
}
