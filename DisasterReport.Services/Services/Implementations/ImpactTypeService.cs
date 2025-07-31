using DisasterReport.Data.Domain;
using DisasterReport.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisasterReport.Data.Repositories;
using DisasterReport.Services.Models;
using Microsoft.Extensions.Caching.Memory;
namespace DisasterReport.Services.Services.Implementations
{

    public class ImpactTypeService : IImpactTypeService
    {
        private readonly IImpactTypeRepo _impactTypeRepo;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "ImpactTypes";

        public ImpactTypeService(IImpactTypeRepo impactTypeRepo, IMemoryCache cache)
        {
            _impactTypeRepo = impactTypeRepo;
            _cache = cache;
        }

        public async Task<List<ImpactTypeDto>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out List<ImpactTypeDto> cachedData))
            {
                var impactTypes = await _impactTypeRepo.GetAllAsync();
                cachedData = impactTypes.Select(st => new ImpactTypeDto { Id = st.Id, Name = st.Name }).ToList();

                _cache.Set(CacheKey, cachedData, TimeSpan.FromMinutes(5)); // Cache for 5 minutes
            }

            return cachedData;
        }

        public async Task<ImpactTypeDto?> GetByIdAsync(int id)
        {
            var entity = await _impactTypeRepo.GetByIdAsync(id);
            return entity == null ? null : new ImpactTypeDto { Id = entity.Id, Name = entity.Name };
        }

        public async Task<ImpactTypeDto> AddAsync(ImpactTypeDto dto)
        {
            var entity = new ImpactType { Name = dto.Name };
            var added = await _impactTypeRepo.AddAsync(entity);

            _cache.Remove(CacheKey); // Clear cache on write
            return new ImpactTypeDto { Id = added.Id, Name = added.Name };
        }

        public async Task<ImpactTypeDto?> UpdateAsync(ImpactTypeDto dto)
        {
            var entity = new ImpactType { Id = dto.Id, Name = dto.Name };
            var updated = await _impactTypeRepo.UpdateAsync(entity);
            if (updated == null) return null;

            _cache.Remove(CacheKey);
            return new ImpactTypeDto { Id = updated.Id, Name = updated.Name };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _impactTypeRepo.DeleteAsync(id);
            if (result)
            {
                _cache.Remove(CacheKey);
            }
            return result;
        }
    }

}
