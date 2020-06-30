using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IO;
using Moq;
using NetCache.Tests.TestHelpers;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace NetCache.Tests
{
    public class CacheHelperTest
    {
        private readonly IMemoryCache _cache = new MemoryCache(Options.Create(new MemoryCacheOptions()));

        [Fact]
        public void CacheProviderFactoryTest()
        {
            string? name = null;

            var mock = new Mock<ICacheProviderFactory>();

            mock.Setup(x => x.Create(It.IsAny<string>()))
                .Returns((string str) =>
                {
                    name = str;

                    return new MemoryCacheProvider(str, _cache);
                });

            _ = new CacheHelper(Int64Cache.CacheName,
                mock.Object,
                new Mock<IDistributedLockFactory>().Object,
                new KeyFormatter(),
#if NET46
                new NewtonsoftJsonSerializer(),
#else
                new SystemTextJsonSerializer(),
#endif
                new RecyclableMemoryStreamManager(),
                new CacheOptions(), Int64Cache.DefaultTtl);

            Assert.Equal($"{(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName().Name}/Cache/{Int64Cache.CacheName}", name);

            mock.Verify(x => x.Create(name!));
        }

        [Fact]
        public void DistributedLockFactoryTest()
        {
            string? name = null;
            string? key = null;

            var mock = new Mock<IDistributedLockFactory>();

            mock.Setup(x => x.CreateLock(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string str, string _) =>
                {
                    name = str;
                    key = _;

                    return new LocalLock(str);
                });

            var helper = new CacheHelper(Int64Cache.CacheName,
                new MemoryCacheProviderFactory(new MemoryCache(Options.Create(new MemoryCacheOptions()))),
                mock.Object,
                new KeyFormatter(),
#if NET46
                new NewtonsoftJsonSerializer(),
#else
                new SystemTextJsonSerializer(),
#endif
                new RecyclableMemoryStreamManager(),
                new CacheOptions(), Int64Cache.DefaultTtl);

            helper.GetOrSet(Guid.NewGuid().ToString(), (_1, _2, token) => "", TimeSpan.FromSeconds(Int64Cache.DefaultTtl), default);

            Assert.Equal($"{(Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName().Name}/Lock/{Int64Cache.CacheName}", name);

            mock.Verify(x => x.CreateLock(name!, key!));
        }

        [Fact]
        public async Task GetTest()
        {
            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToByteArray();

            var provider = new Mock<ICacheProvider>();
            provider.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(value);
            provider.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<ReadOnlyMemory<byte>?>(value));

            string? name = null;

            provider.SetupGet(x => x.Name).Returns(() => name!);

            var factory = new Mock<ICacheProviderFactory>();

            factory.Setup(x => x.Create(It.IsAny<string>()))
                .Returns<string>(_ =>
                {
                    name = _;

                    return provider.Object;
                });

            var helper = new CacheHelper(Int64Cache.CacheName, factory.Object, new Mock<IDistributedLockFactory>().Object,
                new KeyFormatter(),
#if NET46
                new NewtonsoftJsonSerializer(),
#else
                new SystemTextJsonSerializer(),
#endif
                new RecyclableMemoryStreamManager(),
                new CacheOptions(), Int64Cache.DefaultTtl);

            Assert.Equal(value, helper.Get<string, byte[]>(key, default));
            Assert.Equal(value, await helper.GetAsync<string, byte[]>(key, default).ConfigureAwait(false));

            provider.Verify(x => x.Get(key, It.IsAny<CancellationToken>()));
            provider.Verify(x => x.GetAsync(key, It.IsAny<CancellationToken>()));
            provider.VerifyGet(x => x.Name);

            provider.VerifyNoOtherCalls();
        }

        public interface IMockProvider : ICacheProvider, IDisposable
#if !NET46
            , IAsyncDisposable
#endif
        { }

        [Fact]
        public async Task GetOrSetAsync()
        {
            byte[]? result = null;

            var provider = new Mock<IMockProvider>();
            provider.Setup(x => x.Get(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => result == null ? new ReadOnlyMemory<byte>?() : new ReadOnlyMemory<byte>(result));
            provider.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(() => result == null ? default : new ValueTask<ReadOnlyMemory<byte>?>(result));
            provider.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<TimeSpan>(), It.IsAny<When>(), It.IsAny<CancellationToken>()))
                .Callback<string, ReadOnlyMemory<byte>, TimeSpan, When, CancellationToken>((s, v, t, w, token) =>
                {
                    result = v.Span.ToArray();
                })
                .Returns(new ValueTask<bool>());

            var semaphore = new SemaphoreSlim(1, 1);

            var dl = new Mock<IDistributedLock>();
            dl.Setup(x => x.Lock(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns<int, CancellationToken>((t, token) =>
                {
                    try
                    {
                        return semaphore.Wait(t * 1000, token);
                    }
                    catch (OperationCanceledException)
                    {
                    }

                    return false;
                });
            dl.Setup(x => x.LockAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .Returns<int, CancellationToken>(async (t, token) =>
                    await semaphore.WaitAsync(t * 1000, token).ConfigureAwait(false));
            dl.Setup(x => x.Dispose()).Callback(() => semaphore.Release());
            dl.Setup(x => x.DisposeAsync())
                .Returns(() =>
                {
                    semaphore.Release();

                    return default;
                });

            var dlf = new Mock<IDistributedLockFactory>();
            dlf.Setup(x => x.CreateLock(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(dl.Object);

            string? name = null;

            provider.SetupGet(x => x.Name).Returns(() => name!);

            var factory = new Mock<ICacheProviderFactory>();

            factory.Setup(x => x.Create(It.IsAny<string>()))
                .Returns<string>(_ =>
                {
                    name = _;

                    return provider.Object;
                });

            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToByteArray();
            var ttl = TimeSpan.FromSeconds(30);

            var helper = new CacheHelper(Int64Cache.CacheName, factory.Object, dlf.Object,
                new KeyFormatter(),
#if NET46
                new NewtonsoftJsonSerializer(),
#else
                new SystemTextJsonSerializer(),
#endif
                new RecyclableMemoryStreamManager(),
                new CacheOptions(), Int64Cache.DefaultTtl);

            var task = helper.GetOrSetAsync(key, async (s, t, token) =>
            {
                await Task.Delay(TimeSpan.FromMilliseconds(500), token).ConfigureAwait(false);

                return value;
            }, ttl, default);

            Assert.False(task.IsCompleted);

            Assert.Equal(value, helper.GetOrSet(key, (s, t, token) => value, ttl, default));

            Assert.True(task.IsCompleted);

            Assert.Equal(value, result);
            Assert.Equal(value, await task.ConfigureAwait(false));
            Assert.Equal(value, helper.GetOrSet(key, (s, t, token) => value, ttl, default));

            provider.Verify(x => x.Get(key, It.IsAny<CancellationToken>()));
            provider.Verify(x => x.GetAsync(key, It.IsAny<CancellationToken>()));
            provider.Verify(x => x.SetAsync(key, value, ttl, It.IsAny<When>(), It.IsAny<CancellationToken>()));
            provider.VerifyGet(x => x.Name);

            dl.Verify(x => x.Lock(new CacheOptions().LockTimeout * 1000, It.IsAny<CancellationToken>()));
            dl.Verify(x => x.LockAsync(new CacheOptions().LockTimeout * 1000, It.IsAny<CancellationToken>()));
            dl.Verify(x => x.ReleaseAsync());
            dl.Verify(x => x.Dispose());
            dl.Verify(x => x.DisposeAsync());

            provider.VerifyNoOtherCalls();
            dl.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task SetTest()
        {
            var provider = new Mock<ICacheProvider>();
            provider.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<TimeSpan>(), It.IsAny<When>(), It.IsAny<CancellationToken>()));
            provider.Setup(x => x.SetAsync(It.IsAny<string>(), It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<TimeSpan>(), It.IsAny<When>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<bool>());

            string? name = null;

            provider.SetupGet(x => x.Name).Returns(() => name!);

            var factory = new Mock<ICacheProviderFactory>();

            factory.Setup(x => x.Create(It.IsAny<string>()))
                .Returns<string>(_ =>
                {
                    name = _;

                    return provider.Object;
                });

            var helper = new CacheHelper(Int64Cache.CacheName, factory.Object, new Mock<IDistributedLockFactory>().Object,
                new KeyFormatter(),
#if NET46
                new NewtonsoftJsonSerializer(),
#else
                new SystemTextJsonSerializer(),
#endif
                new RecyclableMemoryStreamManager(),
                new CacheOptions(), Int64Cache.DefaultTtl);

            var key = Guid.NewGuid().ToString();
            var value = new ReadOnlyMemory<byte>(Guid.NewGuid().ToByteArray());
            var ttl = TimeSpan.FromSeconds(30);

            helper.Set(key, value, ttl, When.NotExists, default);
            await helper.SetAsync(key, value, ttl, When.Exists, default).ConfigureAwait(false);

            provider.Verify(x => x.Set(key, value, ttl, When.NotExists, It.IsAny<CancellationToken>()));
            provider.Verify(x => x.SetAsync(key, value, ttl, When.Exists, It.IsAny<CancellationToken>()));
            provider.VerifyGet(x => x.Name);

            provider.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task RemoveTest()
        {
            var key = Guid.NewGuid().ToString();

            var provider = new Mock<ICacheProvider>();
            provider.Setup(x => x.Remove(It.IsAny<string>(), It.IsAny<CancellationToken>()));
            provider.Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<bool>());

            string? name = null;

            provider.SetupGet(x => x.Name).Returns(() => name!);

            var factory = new Mock<ICacheProviderFactory>();

            factory.Setup(x => x.Create(It.IsAny<string>()))
                .Returns<string>(_ =>
                {
                    name = _;

                    return provider.Object;
                });

            var helper = new CacheHelper(Int64Cache.CacheName, factory.Object, new Mock<IDistributedLockFactory>().Object,
                new KeyFormatter(),
#if NET46
                new NewtonsoftJsonSerializer(),
#else
                new SystemTextJsonSerializer(),
#endif
                new RecyclableMemoryStreamManager(),
                new CacheOptions(), Int64Cache.DefaultTtl);

            helper.Remove(key, default);
            await helper.RemoveAsync(key, default).ConfigureAwait(false);

            provider.Verify(x => x.Remove(key, It.IsAny<CancellationToken>()));
            provider.Verify(x => x.RemoveAsync(key, It.IsAny<CancellationToken>()));
            provider.VerifyGet(x => x.Name);

            provider.VerifyNoOtherCalls();
        }

        [Fact]
        public void TtlTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CacheOptions { DefaultTtl = 0 });

            var provider = new Mock<ICacheProvider>();

            var ttl = TimeSpan.Zero;
            provider.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<TimeSpan>(), It.IsAny<When>(), It.IsAny<CancellationToken>()))
                .Callback<string, ReadOnlyMemory<byte>, TimeSpan, When, CancellationToken>((s, memory, arg3, arg4, arg5) =>
                {
                    ttl = arg3;
                });

            string? name = null;

            provider.SetupGet(x => x.Name).Returns(() => name!);

            var factory = new Mock<ICacheProviderFactory>();

            factory.Setup(x => x.Create(It.IsAny<string>()))
                .Returns<string>(_ =>
                {
                    name = _;

                    return provider.Object;
                });

            var key = Guid.NewGuid().ToString();
            var value = new ReadOnlyMemory<byte>(Guid.NewGuid().ToByteArray());

            #region CacheAttribute.TtlSecond
            var helper = new CacheHelper(Int64Cache.CacheName, factory.Object, new Mock<IDistributedLockFactory>().Object,
                new KeyFormatter(),
#if NET46
                new NewtonsoftJsonSerializer(),
#else
                new SystemTextJsonSerializer(),
#endif
                new RecyclableMemoryStreamManager(),
                new CacheOptions(), Int64Cache.DefaultTtl);

            helper.Set(key, value, TimeSpan.FromSeconds(20), When.NotExists, default);
            Assert.Equal(TimeSpan.FromSeconds(20), ttl);

            helper.Set(key, value, TimeSpan.Zero, When.NotExists, default);
            Assert.Equal(TimeSpan.FromSeconds(Int64Cache.DefaultTtl), ttl);
            #endregion

            #region CacheOptions.Ttl
            helper = new CacheHelper(Int64Cache.CacheName, factory.Object, new Mock<IDistributedLockFactory>().Object,
                new KeyFormatter(),
#if NET46
                new NewtonsoftJsonSerializer(),
#else
                new SystemTextJsonSerializer(),
#endif
                new RecyclableMemoryStreamManager(),
                new CacheOptions { Ttl = { { Int64Cache.CacheName, 20 } } }, 0);

            helper.Set(key, value, TimeSpan.FromSeconds(15), When.NotExists, default);
            Assert.Equal(TimeSpan.FromSeconds(15), ttl);

            helper.Set(key, value, TimeSpan.Zero, When.NotExists, default);
            Assert.Equal(TimeSpan.FromSeconds(20), ttl);
            #endregion

            #region CacheOptions.DefaultTtl
            helper = new CacheHelper(Int64Cache.CacheName, factory.Object, new Mock<IDistributedLockFactory>().Object,
                new KeyFormatter(),
#if NET46
                new NewtonsoftJsonSerializer(),
#else
                new SystemTextJsonSerializer(),
#endif
                new RecyclableMemoryStreamManager(),
                new CacheOptions(), 0);

            helper.Set(key, value, TimeSpan.FromSeconds(20), When.NotExists, default);
            Assert.Equal(TimeSpan.FromSeconds(20), ttl);

            helper.Set(key, value, TimeSpan.Zero, When.NotExists, default);
            Assert.Equal(TimeSpan.FromSeconds(30), ttl);
            #endregion

            #region CacheOptions.MaxTll
            helper = new CacheHelper(Int64Cache.CacheName, factory.Object, new Mock<IDistributedLockFactory>().Object,
                new KeyFormatter(),
#if NET46
                new NewtonsoftJsonSerializer(),
#else
                new SystemTextJsonSerializer(),
#endif
                new RecyclableMemoryStreamManager(),
                new CacheOptions { MaxTll = 5 }, Int64Cache.DefaultTtl);

            helper.Set(key, value, TimeSpan.FromSeconds(20), When.NotExists, default);
            Assert.Equal(TimeSpan.FromSeconds(5), ttl);

            helper.Set(key, value, TimeSpan.Zero, When.NotExists, default);
            Assert.Equal(TimeSpan.FromSeconds(5), ttl);
            #endregion
        }
    }
}
