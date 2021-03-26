using System;

namespace NetCache.Tests.TestHelpers
{
    public abstract class GenericMethodCache
    {
        public virtual T Get<[Test("3"), Test("3", Abc = "4")]T>(string key) where T : struct => default;
        public abstract void Set<T>(string key, T value, int ttl) where T : class, IComparable;
    }

    [AttributeUsage(AttributeTargets.GenericParameter, AllowMultiple = true)]
    public class TestAttribute : Attribute
    {
        public TestAttribute(string def) => Def = def;

        public string? Abc { get; set; }
        public string Def { get; }
    }
}
