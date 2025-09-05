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
   
    public class SupportTypeService : ISupportTypeService
    {
        private readonly ISupportTypeRepo _supportTypeRepo;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "SupportTypes";

        public SupportTypeService(ISupportTypeRepo supportTypeRepo, IMemoryCache cache)
        {
            _supportTypeRepo = supportTypeRepo;
            _cache = cache;
        }

        public async Task<List<SupportTypeDto>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out List<SupportTypeDto> cachedData))
            {
                var supportTypes = await _supportTypeRepo.GetAllAsync();
                cachedData = supportTypes.Select(st => new SupportTypeDto { Id = st.Id, Name = st.Name }).ToList();

                _cache.Set(CacheKey, cachedData, TimeSpan.FromMinutes(5)); 
            }

            return cachedData;
        }

        public async Task<SupportTypeDto?> GetByIdAsync(int id)
        {
            var entity = await _supportTypeRepo.GetByIdAsync(id);
            return entity == null ? null : new SupportTypeDto { Id = entity.Id, Name = entity.Name };
        }

        public async Task<SupportTypeDto> AddAsync(SupportTypeDto dto)
        {
            var entity = new SupportType { Name = dto.Name };
            var added = await _supportTypeRepo.AddAsync(entity);

            _cache.Remove(CacheKey); 
            return new SupportTypeDto { Id = added.Id, Name = added.Name };
        }

        public async Task<SupportTypeDto?> UpdateAsync(SupportTypeDto dto)
        {
            var entity = new SupportType { Id = dto.Id, Name = dto.Name };
            var updated = await _supportTypeRepo.UpdateAsync(entity);
            if (updated == null) return null;

            _cache.Remove(CacheKey);
            return new SupportTypeDto { Id = updated.Id, Name = updated.Name };
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _supportTypeRepo.DeleteAsync(id);
            if (result)
            {
                _cache.Remove(CacheKey);
            }
            return result;
        }
    }

}
