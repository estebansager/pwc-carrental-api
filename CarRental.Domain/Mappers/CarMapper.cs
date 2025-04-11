using CarRental.DB.Entities;
using CarRental.Domain.Models.Cars;

namespace CarRental.Domain.Mappers
{
    internal class CarMapper
    {

            public static CarDto MapToDto(Car car)
            {
                return new CarDto()
                {
                    Id = car.Id,
                    Model = car.Model,
                    Type = car.Type
                };

            }

    }


}
