using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mos3ef.BLL.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _Cache;

        public MemoryCacheService(IMemoryCache cache)
        {
            _Cache = cache;
        }

        public Task<T?> GetAsync<T>(string key)
            => Task.FromResult(_Cache.Get<T>(key));

        public Task SetAsync<T>(string key, T value, TimeSpan duration)
        {
            _Cache.Set(key, value, duration);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _Cache.Remove(key);
            return Task.CompletedTask;
        }
    }
}
