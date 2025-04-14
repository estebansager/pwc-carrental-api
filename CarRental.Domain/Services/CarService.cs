using CarRental.DB.Repositories;
using Microsoft.Extensions.Configuration;

namespace CarRental.Domain.Services
{
    public class CarService : ICarService
    {
        
        private readonly ICarRentalDbUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly string _carTypesCacheKey;
        private readonly string _carModelsCacheKey;
        private readonly int _carTypesCacheDurationInHours;
        private readonly int _carModelsCacheDurationInHours;

        public CarService(ICarRentalDbUnitOfWork unitOfWork, ICacheService cacheService, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _carTypesCacheKey = configuration.GetSection("Caching:CarTypeCacheKey").Value;
            _carModelsCacheKey = configuration.GetSection("Caching:CarModelCacheKey").Value;
            _carTypesCacheDurationInHours = int.Parse(configuration.GetSection("Caching:CarTypeCacheDurationInHours").Value);
            _carModelsCacheDurationInHours = int.Parse(configuration.GetSection("Caching:CarModelCacheDurationInHours").Value);
        }

        /// <summary>
        /// Returns the list of supported car types. For simplicity I'm returning the distinct values populated in the DB
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetCarTypes()
        {
            return await _cacheService.GetOrSetAsync(_carTypesCacheKey, async () =>
            {
                return (await _unitOfWork.Cars.FindAsync(c => true)).Select(c => c.Type).Distinct();
            }, TimeSpan.FromHours(_carTypesCacheDurationInHours));
        }

        /// <summary>
        /// Returns the list of supported models. For simplicity, I'm returning a fixed set of models found in the DB
        /// Ideally in Prod there should be a relation between car types and models
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetCarModels()
        {
            return await _cacheService.GetOrSetAsync(_carModelsCacheKey, async () =>
            {
                return (await _unitOfWork.Cars.FindAsync(c => true)).Select(c => c.Model).Distinct();
            }, TimeSpan.FromHours(_carModelsCacheDurationInHours));
        }
    }
}
