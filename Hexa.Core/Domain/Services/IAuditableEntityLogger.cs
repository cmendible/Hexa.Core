using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexa.Core.Domain
{
    public interface IAuditableEntityLogger
    {
        void Log(string tableName, string propertyName, object oldValue, object newValue);
    }
}
