namespace Hexa.Core.Domain.Filters
{
    using FluentNHibernate.Mapping;
    using NHibernate.Type;

    public class TenantFilter : FilterDefinition
    {
        public TenantFilter()
        {
            WithName("TenantFilter");
            WithCondition("TenantId = :tenantId");

            AddParameter("tenantId", new GuidType());
        }
    }
}
