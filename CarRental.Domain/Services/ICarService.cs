namespace CarRental.Domain.Services
{
    public interface ICarService
    {
        Task<IEnumerable<string>> GetCarTypes();
        Task<IEnumerable<string>> GetCarModels();
    }
}
