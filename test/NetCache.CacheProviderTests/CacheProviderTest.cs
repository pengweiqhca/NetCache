using System;
using System.Threading.Tasks;
using Xunit;

namespace NetCache.CacheProviderTests
{
    public abstract class CacheProviderTest
    {
        private readonly ICacheProviderFactory _factory;
        private readonly bool _supportMultipleKey;

        protected CacheProviderTest(ICacheProviderFactory factory, bool supportMultipleKey)
        {
            _factory = factory;
            _supportMultipleKey = supportMultipleKey;
        }

        [Fact]
        public void IsIMultipleKeyCacheProvider() =>
            Assert.Equal(_supportMultipleKey, _factory.Create(Guid.NewGuid().ToString()) is IMultipleKeyCacheProvider);

        [Fact]
        public void SingleKeyTest()
        {
            var provider = _factory.Create(Guid.NewGuid().ToString());

            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToByteArray();

            Assert.Null(provider.Get(key, default));
            Assert.False(provider.Remove(key, default));

            Assert.False(provider.Set(key, value, TimeSpan.FromSeconds(1), When.Exists, default));
            Assert.True(provider.Set(key, value, TimeSpan.FromSeconds(1), When.NotExists, default));
            Assert.True(provider.Set(key, value, TimeSpan.FromSeconds(1), When.Exists, default));
            Assert.True(provider.Set(key, value, TimeSpan.FromSeconds(1), When.Always, default));

            Assert.Equal(value, provider.Get(key, default).GetValueOrDefault().ToArray());
            Assert.True(provider.Remove(key, default));
        }

        [Fact]
        public async Task SingleKeyAsyncTest()
        {
            var provider = _factory.Create(Guid.NewGuid().ToString());

            var key = Guid.NewGuid().ToString();
            var value = Guid.NewGuid().ToByteArray();

            Assert.Null(await provider.GetAsync(key, default).ConfigureAwait(false));
            Assert.False(await provider.RemoveAsync(key, default).ConfigureAwait(false));

            Assert.False(await provider.SetAsync(key, value, TimeSpan.FromSeconds(1), When.Exists, default).ConfigureAwait(false));
            Assert.True(await provider.SetAsync(key, value, TimeSpan.FromSeconds(1), When.NotExists, default).ConfigureAwait(false));
            Assert.True(await provider.SetAsync(key, value, TimeSpan.FromSeconds(1), When.Exists, default).ConfigureAwait(false));
            Assert.True(await provider.SetAsync(key, value, TimeSpan.FromSeconds(1), When.Always, default).ConfigureAwait(false));

            Assert.Equal(value, (await provider.GetAsync(key, default).ConfigureAwait(false)).GetValueOrDefault().ToArray());
            Assert.True(await provider.RemoveAsync(key, default).ConfigureAwait(false));
        }

        [Fact]
        public void EmptyTest()
        {
            var provider = _factory.Create(Guid.NewGuid().ToString());

            var key = Guid.NewGuid().ToString();
            var value = new ReadOnlyMemory<byte>(Array.Empty<byte>());

            provider.Set(key, value, TimeSpan.FromSeconds(1), When.Always, default);

            var result = provider.Get(key, default);

            Assert.NotNull(result);
            Assert.Empty(result!.Value.ToArray());
        }
    }
}
