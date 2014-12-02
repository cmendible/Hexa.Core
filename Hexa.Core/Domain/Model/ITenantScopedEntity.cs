using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexa.Core.Domain
{
    public interface ITenantScopedEntity
    {
        Guid? TenantId { get; }

        void SetTenantId(Guid? tenantId);
    }
}
