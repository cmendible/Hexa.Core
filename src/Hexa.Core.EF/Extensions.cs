using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore
{
    public static class Extensions
    {
        public static TEntity Find<TEntity>(this DbSet<TEntity> set, params object[] keyValues) where TEntity : class
        {
            var context = ((IInfrastructure<IServiceProvider>)set).GetService<DbContext>();

            var entityType = context.Model.FindEntityType(typeof(TEntity));
            var key = entityType.FindPrimaryKey();

            var entries = context.ChangeTracker.Entries<TEntity>();

            var i = 0;
            foreach (var property in key.Properties)
            {
                entries = Enumerable.Where(entries, e => e.Property(property.Name).CurrentValue == keyValues[i]);
                i++;
            }

            var entry = entries.FirstOrDefault();
            if (entry != null)
            {
                // Return the local object if it exists.
                return entry.Entity;
            }

            // TODO: Build the real LINQ Expression
            // set.Where(x => x.Id == keyValues[0]);
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var query = Queryable.Where(set, (Expression<Func<TEntity, bool>>)
                Expression.Lambda(
                    Expression.Equal(
                        Expression.Property(parameter, "Id"),
                        Expression.Constant(keyValues[0])),
                    parameter));

            // Look in the database
            return query.FirstOrDefault();
        }
    }

    public interface IEntityTypeConfiguration
    {
        void Map(ModelBuilder b);
    }

    public interface IEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration where TEntity : class
    {
        void Map(EntityTypeBuilder<TEntity> builder);
    }

    public abstract class EntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
        where TEntity : class
    {
        public abstract void Map(EntityTypeBuilder<TEntity> b);

        public void Map(ModelBuilder b)
        {
            Map(b.Entity<TEntity>());
        }
    }

    public static class ModelBuilderExtensions
    {
        public static void AddConfiguration<TEntity>(this ModelBuilder modelBuilder, EntityTypeConfiguration<TEntity> configuration)
            where TEntity : class
        {
            configuration.Map(modelBuilder.Entity<TEntity>());
        }
    }

    public static class ModelBuilderExtenions
    {
        private static IEnumerable<Type> GetMappingTypes(this Assembly assembly, Type mappingInterface)
        {
            return assembly.GetTypes().Where(x => !x.GetTypeInfo().IsAbstract && x.GetInterfaces().Any(y => y.GetTypeInfo().IsGenericType && y.GetGenericTypeDefinition() == mappingInterface));
        }

        public static void AddEntityConfigurationsFromAssembly(this ModelBuilder modelBuilder, Assembly[] assembly)
        {
            var mappingTypes = assembly.SelectMany(a => a.GetMappingTypes(typeof(IEntityTypeConfiguration<>)));
            foreach (var config in mappingTypes.Select(Activator.CreateInstance).Cast<IEntityTypeConfiguration>())
            {
                config.Map(modelBuilder);
            }
        }
    }
}