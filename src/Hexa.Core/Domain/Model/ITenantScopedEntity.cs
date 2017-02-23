using System;

namespace Hexa.Core.Domain
{
    public interface ITenantScopedEntity
    {
        Guid? TenantId { get; }

        void SetTenantId(Guid? tenantId);
    }
}
