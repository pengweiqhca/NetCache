using Microsoft.Extensions.DependencyInjection;
using NetCache.Tests.TestHelpers;
using System;

namespace NetCache.Demo
{
    public static class Program
    {
        private static void Main(string[] args)
        {
            var services = new ServiceCollection().AddMemoryCache();

            services.AddNetCache()
                .AddProxy<Int64Cache>(_ => new object[] { _ })
#if NET46
                .UseNewtonsoftJsonSerializer()
#else
                .UseSystemTextJsonSerializer()
#endif
                .UseMemoryCache();

            using var scope = services.BuildServiceProvider().CreateScope();

            var obj = scope.ServiceProvider.GetRequiredService<Int64Cache>();

            var key = Guid.NewGuid().ToString();
            var value = new Random().Next();

            Console.WriteLine(obj.Get(key));

            obj.Delete(key);
            obj.RemoveAsync(key);

            obj.Set(key, value, 3);

            Console.WriteLine(obj.Get(key));
        }
    }
}
