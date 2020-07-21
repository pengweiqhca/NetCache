using Microsoft.Extensions.DependencyInjection;
#if !NET46
using Microsoft.Extensions.Hosting;
#endif
using NetCache.Tests.TestHelpers;
using System;

namespace NetCache.Demo
{
    public static class Program
    {
        private static void Main(string[] args)
        {
#if NET46
            var services = new ServiceCollection();
#else
            using var host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices(services =>
                    {
#endif
                        services.AddMemoryCache();

                        services.AddScoped<A>();

                        services.AddNetCache()
                        .AddCacheType<Int64Cache>(_ => new object[] { _ })
                        .UseMemoryCache()
#if NET46
                .UseNewtonsoftJsonSerializer();

            var root = services.BuildServiceProvider(true);
#else
                        .UseSystemTextJsonSerializer();
                    })
                    .Build();

            var root = host.Services;
#endif
            using var scope = root.CreateScope();

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
