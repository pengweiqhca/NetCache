// ReSharper disable once CheckNamespace
namespace System.Reflection
{
    internal static class Utilities
    {
        public static MethodInfo? GetGetMethod(this Type type, string property) =>
            type.GetProperty(property)?.GetMethod;
    }
}
