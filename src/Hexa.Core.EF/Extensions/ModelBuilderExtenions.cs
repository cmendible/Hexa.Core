using System.Linq;
using System.Reflection;
using Hexa.Core.Domain;

namespace Microsoft.EntityFrameworkCore
{
    public static class ModelBuilderExtenions
    {
        public static void AddEntityConfigurationsFromAssembly(this ModelBuilder modelBuilder, Assembly[] assembly)
        {
            assembly.SelectMany(a => a.GetTypes().Where(x => !x.GetTypeInfo().IsAbstract && x.GetInterfaces()
                .Any(y => y.GetTypeInfo().IsGenericType && y.GetGenericTypeDefinition() == typeof(IEntity<>))))
                .ToList()
                .ForEach(t =>
                {
                    var entityTypeBuilder = modelBuilder.Entity(t);
                    entityTypeBuilder.ToTable(Inflector.Underscore(t.Name).ToUpper());

                    if (typeof(BaseEntity<,>).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
                    {
                        entityTypeBuilder.Property("Version")
                            .IsRowVersion();
                    }
                });
        }
    }
}