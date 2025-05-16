using Dotnet_Dietitian.Application.Interfaces;
using Dotnet_Dietitian.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dotnet_Dietitian.Application.Decorators
{
    public class CachingRepositoryDecorator<T> : RepositoryDecorator<T> where T : BaseEntity
    {
        private readonly IMemoryCache _cache;
        private readonly string _entityTypeName;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(30);

        public CachingRepositoryDecorator(IRepository<T> repository, IMemoryCache cache) 
            : base(repository)
        {
            _cache = cache;
            _entityTypeName = typeof(T).Name;
        }

        public override async Task<T> GetByIdAsync(Guid id)
        {
            string cacheKey = $"{_entityTypeName}_{id}";
            
            if (_cache.TryGetValue(cacheKey, out T cachedEntity))
            {
                return cachedEntity;
            }
            
            var entity = await base.GetByIdAsync(id);
            
            if (entity != null)
            {
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(_cacheExpiration);
                
                _cache.Set(cacheKey, entity, cacheOptions);
            }
            
            return entity;
        }

        public override async Task<IReadOnlyList<T>> GetAllAsync()
        {
            string cacheKey = $"{_entityTypeName}_All";
            
            if (_cache.TryGetValue(cacheKey, out IReadOnlyList<T> cachedEntities))
            {
                return cachedEntities;
            }
            
            var entities = await base.GetAllAsync();
            
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(_cacheExpiration);
            
            _cache.Set(cacheKey, entities, cacheOptions);
            
            return entities;
        }

        public override async Task<T> AddAsync(T entity)
        {
            var result = await base.AddAsync(entity);
            
            // Yeni eklemede cache'i temizle
            InvalidateCacheForType();
            
            return result;
        }

        public override async Task UpdateAsync(T entity)
        {
            await base.UpdateAsync(entity);
            
            // Güncelleme sonrası cache'i temizle
            string cacheKey = $"{_entityTypeName}_{entity.Id}";
            _cache.Remove(cacheKey);
            InvalidateCacheForType();
        }

        public override async Task DeleteAsync(T entity)
        {
            await base.DeleteAsync(entity);
            
            // Silme sonrası cache'i temizle
            string cacheKey = $"{_entityTypeName}_{entity.Id}";
            _cache.Remove(cacheKey);
            InvalidateCacheForType();
        }

        private void InvalidateCacheForType()
        {
            string allCacheKey = $"{_entityTypeName}_All";
            _cache.Remove(allCacheKey);
        }
    }
}