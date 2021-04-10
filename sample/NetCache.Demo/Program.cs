using Microsoft.Extensions.DependencyInjection;
#if !NET46
using Microsoft.Extensions.Hosting;
#endif
using NetCache.Tests.TestHelpers;
using System;
using System.Threading.Tasks;

namespace NetCache.Demo
{
    public static class Program
    {
        private static async Task Main(string[] args)
        {
#if NET46
            var services = new ServiceCollection();
#else
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices(services =>
                {
#endif
                    services.AddMemoryCache()
                        .AddNetCache()
                        .AddCacheType<Int64Cache>(_ => new object[] { _ })
#if NETCOREAPP3_1_OR_GREATER
                        .AddCacheType<IDefaultImplementation>()
#endif
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
            var scope = root.CreateScope();
            try
            {
                var obj = scope.ServiceProvider.GetRequiredService<Int64Cache>();

                var key = Guid.NewGuid().ToString();
                var value = new Random().Next();

                Console.WriteLine(obj.Get(key));

                obj.Delete(key);
                await obj.RemoveAsync(key).ConfigureAwait(false);

                obj.Set(key, value, 3);

                Console.WriteLine(obj.Get(key));
#if NETCOREAPP3_1_OR_GREATER
                Console.WriteLine(scope.ServiceProvider.GetRequiredService<IDefaultImplementation>().Get("abc"));
#endif
            }
            finally
            {
#if NET46
                scope.Dispose();
                (root as IDisposable)?.Dispose();
#else
                if (scope is IAsyncDisposable ad)
                    await ad.DisposeAsync().ConfigureAwait(false);
                else scope.Dispose();

                if (host is IAsyncDisposable ad2)
                    await ad2.DisposeAsync().ConfigureAwait(false);
                else host.Dispose();
#endif
            }
        }
#if NETCOREAPP3_1_OR_GREATER
        public interface IDefaultImplementation
        {
            long Get(string key) => DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
#endif
    }
}
