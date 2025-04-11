using CarRental.DB.Entities;
using CarRental.Domain.Models.Rentals;

namespace CarRental.Domain.Mappers
{
    internal class RentalMapper
    {

            public static RentalDto MapToDto(Rental rental)
            {
                return new RentalDto()
                {
                    Id = rental.Id,
                    CarId = rental.CarId,
                    CustomerId = rental.CustomerId,
                    StartDate = rental.StartDate,
                    EndDate = rental.EndDate
                };

            }

    }

}
