
namespace System.Reflection
{
    public static class ReflectionExtensions
    {
        public static bool IsSubclassOfGeneric(this Type source, Type generic)
        {
            while (source != null && source != typeof(object))
                {
                    var cur = source.IsGenericType ? source.GetGenericTypeDefinition() : source;
                    if (generic == cur)
                        {
                            return true;
                        }
                    source = source.BaseType;
                }
            return false;
        }
    }
}
