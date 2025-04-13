using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Domain.Services;
using CarRental.Domain.Models.Rentals;
using CarRental.Domain.Models.Cars;
using CarRental.Domain.Exceptions;
using CarRental.DB.Repositories;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Microsoft.Extensions.Logging;
using CarRental.DB.Entities;
using System.Linq.Expressions;
using System.Data;

namespace CarRental.Tests
{
    [TestFixture]
    public class CarRentalServiceTests
    {
        private Mock<ICarRentalDbUnitOfWork> _unitOfWorkMock;
        private Mock<ICarRentalDbRepository<Rental>> _rentalRepositoryMock;
        private Mock<ICarRentalDbRepository<Car>> _carRepositoryMock;

        private Mock<ILogger<CarRentalService>> _mockLogger;
        private CarRentalService _carRentalService;

        [SetUp]
        public void Setup()
        {
            _rentalRepositoryMock = new Mock<ICarRentalDbRepository<Rental>>();
            _carRepositoryMock = new Mock<ICarRentalDbRepository<Car>>();
            _unitOfWorkMock = new Mock<ICarRentalDbUnitOfWork>();

            _unitOfWorkMock.SetupProperty(p => p.Rentals, _rentalRepositoryMock.Object);
            _unitOfWorkMock.SetupProperty(p => p.Cars, _carRepositoryMock.Object);
            _mockLogger = new Mock<ILogger<CarRentalService>>();
            _carRentalService = new CarRentalService(_unitOfWorkMock.Object, _mockLogger.Object);
        }

        [Category("Cancel Rental")]
        [Test]
        public async Task CancelRentalAsync_ShouldThrowRentalCancellationException_WhenStartDateIsInPast()
        {
            // Arrange
            var rental = new Rental { StartDate = DateTime.UtcNow.AddDays(-1) };
            _rentalRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rental, bool>>>(), It.IsAny<Expression<Func<Rental, object>>[]>())).ReturnsAsync(new List<Rental>() { rental });

            // Act
            Func<Task> act = async () => await _carRentalService.CancelRentalAsync(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<RentalCancellationException>()
                .WithMessage("The provided rental cannot be cancelled. Make sure it has a starting rental date in the future");
        }

        [Category("Cancel Rental")]
        [Test]
        public async Task CancelRentalAsync_ShouldThrowRentalNotFoundException_WhenPassingInvalidRentalId()
        {
            // Arrange
            _rentalRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rental, bool>>>(), It.IsAny<Expression<Func<Rental, object>>[]>())).ReturnsAsync(new List<Rental>() {  });

            // Act
            Func<Task> act = async () => await _carRentalService.CancelRentalAsync(Guid.NewGuid());

            // Assert
            await act.Should().ThrowAsync<RentalNotFoundException>();
        }

        [Category("Get Rentals")]
        [Test]
        public async Task GetRentalsAsync_ShouldReturnListOfRentalDtos()
        {
            // Arrange
            var rentals = new List<Rental> { new Rental { Id = Guid.NewGuid(), Customer = new Customer() { }, Car = new Car() { } } };
            _rentalRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rental, bool>>>(), It.IsAny<Expression<Func<Rental, object>>[]>())).ReturnsAsync(rentals);


            // Act
            var result = await _carRentalService.GetRentalsAsync();

            // Assert
            result.Should().NotBeEmpty();
            result.First().Should().BeOfType<RentalDto>();
        }

        [Category("Get Rentals")]
        [Test]
        public async Task GetRentalByIdAsync_ShouldReturnRental_WhenRentalExists()
        {
            // Arrange
            var rentals = new List<Rental> { new Rental { Id = Guid.NewGuid(), Customer = new Customer() { }, Car = new Car() { } } };
            var rentalId = new Guid();
            _rentalRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rental, bool>>>(), It.IsAny<Expression<Func<Rental, object>>[]>())).ReturnsAsync(rentals);


            // Act
            var result = await _carRentalService.GetRentalByIdAsync(rentalId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<RentalDto>();
        }

        [Category("Get Rentals")]
        [Test]
        public async Task GetRentalByIdAsync_ShouldThrowRentalNotFoundException_WhenRentalDoesNotExist()
        {
            // Arrange
            var rentals = new List<Rental> {  };
            var rentalId = new Guid();
            _rentalRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rental, bool>>>(), It.IsAny<Expression<Func<Rental, object>>[]>())).ReturnsAsync(rentals);


            // Act
            Func<Task> act = async () => await _carRentalService.GetRentalByIdAsync(rentalId);

            // Assert
            act.Should().NotBeNull(); 
            await act.Should().ThrowAsync<RentalNotFoundException>();
        }

        [Category("Modify Rental")]
        [Test]
        public async Task ModifyReservationAsync_ShouldUpdateRental_WhenValidChangesProvided()
        {
            // Arrange
            var rentalId = Guid.NewGuid();
            var newStart = DateTime.UtcNow.AddDays(5);
            var newEnd = newStart.AddDays(2);
            var newCarModel = "Volkwagen Vento";
            var newCarType = CarType.Sedan;

            var existingRental = new Rental
            {
                Id = rentalId,
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(4),
                Car = new Car { Id = Guid.NewGuid(), Model = "Toyota Corolla", Type = "Sedan" },
                CarId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                Customer = new Customer() { },
            };

            var availableCar = new Car { Id = Guid.NewGuid(), Model = newCarModel, Type = newCarType.ToString() };

            _carRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>(), It.IsAny<Expression<Func<Car, object>>[]>())).ReturnsAsync(new List<Car>() { availableCar });
            _rentalRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rental, bool>>>(), It.IsAny<Expression<Func<Rental, object>>[]>())).ReturnsAsync(new List<Rental>() { existingRental });

            // Act
            var result = await _carRentalService.ModifyReservationAsync(rentalId, newStart, newEnd, newCarModel, newCarType);

            // Assert
            result.Should().NotBeNull();
            result.StartDate.Should().Be(newStart);
            result.EndDate.Should().Be(newEnd);
            result.CarId.Should().Be(availableCar.Id);

            _rentalRepositoryMock.Verify(r => r.Update(It.IsAny<Rental>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Category("Modify Rental")]
        [TestCase(-2, 2, TestName = "Modify Rental - Start date in the past")]
        [TestCase(2, 1, TestName = "Modify Rental - End date before start")]
        public async Task ModifyReservationAsync_ShouldThrowInvalidRentDatesException_WhenDatesAreInvalid(int startDateDays, int endDatesDays)
        {
            var rentalId = Guid.NewGuid();
            var start = DateTime.UtcNow.Date.AddDays(startDateDays);
            var end = DateTime.UtcNow.Date.AddDays(endDatesDays);

            var rental = new Rental
            {
                Id = rentalId,
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(3),
                Car = new Car { Id = Guid.NewGuid(), Model = "Corolla", Type = "Sedan" },
                Customer = new Customer() { }
            };

            _rentalRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rental, bool>>>(), It.IsAny<Expression<Func<Rental, object>>[]>())).ReturnsAsync(new List<Rental>() { rental });


            // Act
            Func<Task> act = () => _carRentalService.ModifyReservationAsync(rentalId, start, end);

            // Assert
            await act.Should().ThrowAsync<InvalidRentDatesException>();
                
        }

        [Category("Modify Rental")]
        [Test]
        public async Task ModifyReservationAsync_ShouldReturnEarly_WhenNoChangesProvided()
        {
            var rentalId = Guid.NewGuid();
            var start = DateTime.UtcNow.AddDays(3);
            var end = start.AddDays(2);

            var rental = new Rental
            {
                Id = rentalId,
                StartDate = start,
                EndDate = end,
                Car = new Car { Id = Guid.NewGuid(), Model = "Toyota Corolla", Type = "Sedan" },
                CarId = Guid.NewGuid(),
                Customer = new Customer() { }
            };

            _rentalRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rental, bool>>>(), It.IsAny<Expression<Func<Rental, object>>[]>())).ReturnsAsync(new List<Rental>() { rental });


            // Act
            var result = await _carRentalService.ModifyReservationAsync(rentalId, start, end, "Toyota Corolla", CarType.Sedan);

            // Assert
            result.Should().NotBeNull();
            result.StartDate.Should().Be(start);
            result.EndDate.Should().Be(end);

            _rentalRepositoryMock.Verify(r => r.Update(It.IsAny<Rental>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        }

        [Category("Modify Rental")]
        [Test]
        public async Task ModifyReservationAsync_ShouldThrowCarNotAvailableException_WhenNoCarAvailable()
        {
            var rentalId = Guid.NewGuid();
            var newStart = DateTime.UtcNow.AddDays(4);
            var newEnd = newStart.AddDays(2);
            var newModel = "Volkswagen Vento";
            var newType = CarType.Sedan;

            var rental = new Rental
            {
                StartDate = DateTime.UtcNow.AddDays(2),
                EndDate = DateTime.UtcNow.AddDays(3),
                Id = rentalId,
                Car = new Car { Id = Guid.NewGuid(), Model = "Volkswagen Vento", Type = "Sedan" },
                CustomerId = Guid.NewGuid(),
                Customer = new Customer() { }
            };

            _rentalRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rental, bool>>>(), It.IsAny<Expression<Func<Rental, object>>[]>())).ReturnsAsync(new List<Rental>() { rental });

            _carRepositoryMock.Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>(), It.IsAny<Expression<Func<Car, object>>[]>())).ReturnsAsync(new List<Car>());

            // Act
            Func<Task> act = () => _carRentalService.ModifyReservationAsync(rentalId, newStart, newEnd, newModel, newType);

            // Assert
            await act.Should().ThrowAsync<CarNotAvailableException>();
        }

        [Category("Register Rental")]
        [Test]
        public async Task RegisterRentalAsync_ShouldCreateRental_WhenValidInput()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var carModel = "Toyota Corolla";
            var carType = CarType.Sedan;
            var startDate = DateTime.UtcNow.AddDays(2);
            var endDate = startDate.AddDays(3);

            var availableCar = new Car
            {
                Id = Guid.NewGuid(),
                Model = carModel,
                Type = carType.ToString()
            };

            _carRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>(), It.IsAny<Expression<Func<Car, object>>[]>()))
                .ReturnsAsync(new List<Car> { availableCar });

            _rentalRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Rental>()))
                .Callback<Rental>(r => { r.Customer = new Customer(); r.Car = new Car(); })
                .Returns(Task.CompletedTask);

            _unitOfWorkMock
              .Setup(uow => uow.ExecuteTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<IsolationLevel>()))
              .Returns<Func<Task>, IsolationLevel>((func, _) => func());

            // Act
            var result = await _carRentalService.RegisterRentalAsync(customerId, carModel, carType, startDate, endDate);

            // Assert
            result.Should().NotBeNull();
            result.CustomerId.Should().Be(customerId);
            result.CarId.Should().Be(availableCar.Id);
            result.StartDate.Should().Be(startDate);
            result.EndDate.Should().Be(endDate);

            _rentalRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Rental>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Category("Register Rental")]
        [TestCase(-2, 3, TestName = "Start date is in the past")]
        [TestCase(3, 1, TestName = "End date is before start date")]
        public async Task RegisterRentalAsync_ShouldThrowInvalidRentDatesException_WhenDatesAreInvalid(int startOffset, int endOffset)
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var carModel = "Toyota Corolla";
            var carType = CarType.Sedan;
            var startDate = DateTime.UtcNow.Date.AddDays(startOffset);
            var endDate = DateTime.UtcNow.Date.AddDays(endOffset);

            // Act
            Func<Task> act = async () => await _carRentalService.RegisterRentalAsync(customerId, carModel, carType, startDate, endDate);

            // Assert
            await act.Should().ThrowAsync<InvalidRentDatesException>();
        }

        [Category("Register Rental")]
        [Test]
        public async Task RegisterRentalAsync_ShouldThrowCarNotAvailableException_WhenNoAvailableCarFound()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var carModel = "Honda CR-V";
            var carType = CarType.Sedan;
            var startDate = DateTime.UtcNow.AddDays(2);
            var endDate = startDate.AddDays(2);

            _unitOfWorkMock
                .Setup(uow => uow.ExecuteTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<IsolationLevel>()))
                .Returns<Func<Task>, IsolationLevel>((func, _) => func());

            _carRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>(), It.IsAny<Expression<Func<Car, object>>[]>()))
                .ReturnsAsync(new List<Car>()); 

            // Act
            Func<Task> act = async () => await _carRentalService.RegisterRentalAsync(customerId, carModel, carType, startDate, endDate);

            // Assert
            await act.Should().ThrowAsync<CarNotAvailableException>();

        }

        [Category("Register Rental")]
        [Test]
        public async Task RegisterRentalAsync_ShouldThrowCarNotAvailableException_WhenCarIsNotAvailable()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = DateTime.UtcNow.AddDays(2);

            _unitOfWorkMock.Setup(u => u.Cars.FindAsync(It.IsAny<Expression<Func<Car, bool>>>(), null, null))
                .ReturnsAsync(new List<Car>());

            _unitOfWorkMock
              .Setup(uow => uow.ExecuteTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<IsolationLevel>()))
              .Returns<Func<Task>, IsolationLevel>((func, _) => func());

            // Act
            Func<Task> act = async () => await _carRentalService.RegisterRentalAsync(customerId, "Honda CR-V", CarType.SUV, startDate, endDate);

            // Assert
            await act.Should().ThrowAsync<CarNotAvailableException>()
                .WithMessage("The selected car model and type is not available for renting for the provided dates");
        }

        [Category("Check Availability")]
        [Test]
        public async Task CheckAvailabilityAsync_ShouldThrowInvalidRentDatesException_WhenDatesAreInvalid()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-1);
            var endDate = DateTime.UtcNow.AddDays(3);

            // Act
            Func<Task> act = async () => await _carRentalService.CheckAvailabilityAsync(startDate, endDate);

            // Assert
            await act.Should().ThrowAsync<InvalidRentDatesException>();
        }

        [Category("Check Availability")]
        [Test]
        public async Task CheckAvailabilityAsync_ShouldApplyFilters_WhenTypeAndModelProvided()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = startDate.AddDays(3);

            var cars = new List<Car>
            {
                new Car
                {
                    Id = Guid.NewGuid(),
                    Model = "Volkswagen Vento",
                    Type = CarType.Sedan.ToString(),
                    Rentals = new List<Rental>(),
                    Services = new List<Service>()
                },
                new Car
                {
                    Id = Guid.NewGuid(),
                    Model = "Volkswagen Vento",
                    Type = CarType.Sedan.ToString(),
                    Rentals = new List<Rental>(),
                    Services = new List<Service>()
                }
            };

            _carRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>(), It.IsAny<Expression<Func<Car, object>>[]>()))
                .ReturnsAsync(cars);

            // Act
            var result = await _carRentalService.CheckAvailabilityAsync(startDate, endDate, CarType.Sedan, "Volkswagen Vento");

            // Assert
            result.Should().HaveCount(1);
            var available = result.First();
            available.Model.Should().Be("Volkswagen Vento");
            available.Type.Should().Be(CarType.Sedan.ToString());
            available.Count.Should().Be(2);
        }

        [Test]
        public async Task CheckAvailabilityAsync_ShouldExcludeCarsWithOverlappingRentalsOrServices()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(10);
            var endDate = startDate.AddDays(5);

            var overlappingRental = new Rental
            {
                StartDate = startDate.AddDays(1),
                EndDate = endDate.AddDays(1)
            };

            var overlappingService = new Service
            {
                StartDate = startDate.AddDays(1),
                DurationInDays = 3
            };

            var cars = new List<Car>
            {
                new Car
                {
                    Id = Guid.NewGuid(),
                    Model = "Honda CR-V",
                    Type = CarType.SUV.ToString(),
                    Rentals = new List<Rental> { overlappingRental },
                    Services = new List<Service>()
                },
                new Car
                {
                    Id = Guid.NewGuid(),
                    Model = "Honda CR-V",
                    Type = CarType.SUV.ToString(),
                    Rentals = new List<Rental>(),
                    Services = new List<Service> { overlappingService }
                }
            };

            _carRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>(), It.IsAny<Expression<Func<Car, object>>[]>()))
                .ReturnsAsync(new List<Car>());

            // Act
            var result = await _carRentalService.CheckAvailabilityAsync(startDate, endDate);

            // Assert
            result.Should().BeEmpty();
        }

    }
}