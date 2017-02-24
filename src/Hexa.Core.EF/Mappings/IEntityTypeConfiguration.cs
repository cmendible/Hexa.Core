using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore
{
    public interface IEntityTypeConfiguration
    {
        void Map(ModelBuilder b);
    }

    public interface IEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration where TEntity : class
    {
        void Map(EntityTypeBuilder<TEntity> builder);
    }
}