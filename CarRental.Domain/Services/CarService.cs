using CarRental.DB.Repositories;

namespace CarRental.Domain.Services
{
    public class CarService : ICarService
    {
        
        private ICarRentalDbUnitOfWork _unitOfWork;
        public CarService(ICarRentalDbUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Returns the list of supported car types. For simplicity I'm returning the distinct values populated in the DB
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetCarTypes()
        {
            return (await _unitOfWork.Cars.FindAsync(c => true)).Select(c => c.Type).Distinct();
        }

        /// <summary>
        /// Returns the list of supported models. For simplicity, I'm returning a fixed set of models found in the DB
        /// Ideally in Prod there should be a relation between car types and models
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetCarModels()
        {
            return (await _unitOfWork.Cars.FindAsync(c => true)).Select(c => c.Model).Distinct();
        }
    }
}
