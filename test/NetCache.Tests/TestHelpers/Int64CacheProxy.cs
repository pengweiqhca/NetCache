using Microsoft.Extensions.Options;
using Microsoft.IO;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace NetCache.Tests.TestHelpers
{
    public class Int64CacheProxy : Int64Cache
    {
        private readonly CacheHelper _helper;

        public Int64CacheProxy(object any,
            ICacheProviderFactory cpf,
            IDistributedLockFactory factory,
            IKeyFormatter formatter,
            IValueSerializer serializer,
            RecyclableMemoryStreamManager manager,
            IOptionsMonitor<CacheOptions> options)
            : base(any) =>
            _helper = new CacheHelper(CacheName, cpf, factory, formatter, serializer, manager, options, DefaultTtl);

        public override long? Get(string key) =>
            _helper.GetOrSet(key, new FuncAdapter<string, long?>(new Func<string, long?>(base.Get)).Wrap1, TimeSpan.FromSeconds(DefaultTtl), default);

        public override long? Get(string key, TimeSpan ttl, CancellationToken cancellationToken) =>
            _helper.GetOrSet(key, base.Get, ttl, cancellationToken);
        public override Task<ICacheResult<long>> GetAsync(string key) =>
            _helper.Get2Async<string, long>(key, default).AsTask();
        public override Task<long> GetAsync(string key, Func<long> func) =>
            _helper.GetOrSetAsync(key, new FuncAdapter<string, long>(func).Wrap16, TimeSpan.FromSeconds(DefaultTtl), default).AsTask();
        public override Task<long> GetAsync(string key, Func<ValueTask<long>> func) =>
            _helper.GetOrSetAsync(key, new FuncAdapter<string, long>(func).Wrap48, TimeSpan.FromSeconds(DefaultTtl), default).AsTask();

        public override Task<long> GetAsync(string key, TimeSpan ttl, CancellationToken cancellationToken, Func<string, TimeSpan, CancellationToken, ValueTask<long>> func) =>
            _helper.GetOrSetAsync(key, func, ttl, cancellationToken).AsTask();

        public override void Set(string key, string value, int ttlSecond) =>
            _helper.Set(key, value, TimeSpan.FromSeconds(ttlSecond), When.Always, default);
        public override void Set(string key, int ttlSecond, string value) =>
            _helper.Set(key, value, TimeSpan.FromSeconds(ttlSecond), When.Always, default);

        public override void Set(string key, string value, int ttlSecond, CancellationToken cancellationToken) =>
            _helper.Set(key, value, TimeSpan.FromSeconds(ttlSecond), When.Always, cancellationToken);

        public override ValueTask<bool> SetAsync(string key, string value) =>
            _helper.SetAsync(key, value, TimeSpan.FromSeconds(DefaultTtl), When.Always, default);

        public override ValueTask SetAsync(string key, string value, CancellationToken cancellationToken)
        {
            var task = _helper.SetAsync(key, value, TimeSpan.FromSeconds(DefaultTtl), When.Always, cancellationToken);

            return task.IsCompletedSuccessfully ? default : new ValueTask(task.AsTask());
        }

        public override void Delete(string key) => _helper.Remove(key, default);
        public override Task RemoveAsync(string key) => _helper.RemoveAsync(key, default).AsTask();
        public override void Set(string key, int value, byte ttl) => _helper.Set(key, value, TimeSpan.FromSeconds(ttl), default, default);

        public override void Set(string key, int value, sbyte ttl) => _helper.Set(key, value, TimeSpan.FromSeconds(ttl), default, default);

        public override void Set(string key, int value, short ttl) => _helper.Set(key, value, TimeSpan.FromSeconds(ttl), default, default);

        public override void Set(string key, int value, ushort ttl) => _helper.Set(key, value, TimeSpan.FromSeconds(ttl), default, default);

        public override void Set(string key, int value, uint ttl) => _helper.Set(key, value, TimeSpan.FromSeconds(ttl), default, default);

        public override void Set(string key, int? value, long expiry) => _helper.Set(key, value, TimeSpan.FromSeconds(expiry), default, default);

        public override void Set(string key, int value, ulong expiry) => _helper.Set(key, value, TimeSpan.FromSeconds(expiry), default, default);

        public override void Set(string key, int value, decimal expiry) => _helper.Set(key, value, TimeSpan.FromSeconds((double)expiry), default, default);

        public override void Set(string key, int value, float expiry) => _helper.Set(key, value, TimeSpan.FromSeconds(expiry), default, default);

        public override void Set(string key, int value, double expiry) => _helper.Set(key, value, TimeSpan.FromSeconds(expiry), default, default);

        public override void Set(string key, int value, TimeSpan expiry) => _helper.Set(key, value, expiry, default, default);

        public override void Set(string key, int value, DateTime expiry) => _helper.Set(key, value, expiry - DateTime.Now, default, default);

        public override void Set(string key, int value, DateTimeOffset ttl) => _helper.Set(key, value, ttl - DateTimeOffset.Now, default, default);

        public override void Set(string key, int ttl, [CacheExpiry] int value) => _helper.Set(key, ttl, TimeSpan.FromSeconds(value), default, default);

        public override void Set(string key, int value, byte? ttl) => _helper.Set(key, value, TimeSpan.FromSeconds(ttl.GetValueOrDefault()), default, default);

        public override void Set(string key, int? value, sbyte? ttl) => _helper.Set(key, value, TimeSpan.FromSeconds(ttl.GetValueOrDefault()), default, default);

        public override void Set(string key, int value, short? ttl) => _helper.Set(key, value, TimeSpan.FromSeconds(ttl.GetValueOrDefault()), default, default);

        public override void Set(string key, int value, ushort? ttl) => _helper.Set(key, value, TimeSpan.FromSeconds(ttl.GetValueOrDefault()), default, default);

        public override void Set(string key, int? value, uint? ttl) => _helper.Set(key, value, TimeSpan.FromSeconds(ttl.GetValueOrDefault()), default, default);

        public override void Set(string key, int value, long? expiry) => _helper.Set(key, value, TimeSpan.FromSeconds(expiry.GetValueOrDefault()), default, default);

        public override void Set(string key, int value, ulong? expiry) => _helper.Set(key, value, TimeSpan.FromSeconds(expiry.GetValueOrDefault()), default, default);

        public override void Set(string key, int value, decimal? expiry) => _helper.Set(key, value, TimeSpan.FromSeconds((double)expiry.GetValueOrDefault()), default, default);

        public override void Set(string key, int value, float? expiry) => _helper.Set(key, value, TimeSpan.FromSeconds(expiry.GetValueOrDefault()), default, default);

        public override void Set(string key, int value, double? expiry) => _helper.Set(key, value, TimeSpan.FromSeconds(expiry.GetValueOrDefault()), default, default);

        public override void Set(string key, int value, TimeSpan? expiry) => _helper.Set(key, value, expiry.GetValueOrDefault(), default, default);

        public override void Set(string key, int value, DateTime? expiry) => _helper.Set(key, value, expiry == null ? default : expiry.Value - DateTime.Now, default, default);

        public override void Set(string key, int value, DateTimeOffset? ttl) => _helper.Set(key, value, ttl == null ? default : ttl.Value - DateTimeOffset.Now, default, default);

        public override void Set(string key, string p, int? value) => _helper.Set(key, p, TimeSpan.FromSeconds(value.GetValueOrDefault()), default, default);

        public override void Set(string key, int ttl, [CacheExpiry] int? value) => _helper.Set(key, ttl, TimeSpan.FromSeconds(value.GetValueOrDefault()), default, default);
    }
}

namespace NetCache
{
    public class FuncAdapter<TK, TV>
    {
        private readonly object _func;

        public FuncAdapter(object func) => _func = func;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TV Wrap1(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
            ((Func<TK, TV>)_func)(key);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<TV> Wrap16(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
            new(((Func<TV>)_func)());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ValueTask<TV> Wrap48(TK key, TimeSpan expiry, CancellationToken cancellationToken) =>
            ((Func<ValueTask<TV>>)_func)();
    }
}
