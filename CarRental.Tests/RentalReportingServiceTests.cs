using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CarRental.DB.Entities;
using CarRental.DB.Repositories;
using CarRental.Domain.Exceptions;
using CarRental.Domain.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CarRental.Tests
{
    [TestFixture]
    public class RentalReportingServiceTests
    {
        private Mock<ICarRentalDbRepository<Rental>> _rentalRepositoryMock;
        private Mock<ICarRentalDbRepository<Car>> _carRepositoryMock;
        private Mock<ICarRentalDbUnitOfWork> _unitOfWorkMock;
        private RentalReportingService _rentalReportingService;

        [SetUp]
        public void SetUp()
        {
            _rentalRepositoryMock = new Mock<ICarRentalDbRepository<Rental>>();
            _carRepositoryMock = new Mock<ICarRentalDbRepository<Car>>();
            _unitOfWorkMock = new Mock<ICarRentalDbUnitOfWork>();

            _unitOfWorkMock.SetupProperty(p => p.Rentals, _rentalRepositoryMock.Object);
            _unitOfWorkMock.SetupProperty(p => p.Cars, _carRepositoryMock.Object);

            _rentalReportingService = new RentalReportingService(_unitOfWorkMock.Object);
        }

        [Category("GetMostRentedCarTypeAsync")]
        [Test]
        public async Task GetMostRentedCarTypeAsync_ShouldReturnMostRentedCarTypeAndPercentage()
        {
            // Arrange
            var start = DateTime.UtcNow.Date;
            var end = start.AddDays(10);

            var rentals = new List<Rental>
            {
                new Rental { Car = new Car { Type = "SUV" } },
                new Rental { Car = new Car { Type = "SUV" } },
                new Rental { Car = new Car { Type = "Sedan" } }
            };

            _rentalRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rental, bool>>>(), It.IsAny<Expression<Func<Rental, object>>[]>()))
                .ReturnsAsync(rentals);

            // Act
            var result = await _rentalReportingService.GetMostRentedCarTypeAsync(start, end);

            // Assert
            result.CarType.Should().Be("SUV");
            result.UtilizationPercentage.Should().Be(66.67);
        }

        [Category("GetMostRentedCarTypeAsync")]
        [Test]
        public async Task GetMostRentedCarTypeAsync_ShouldReturnDefault_WhenNoRentals()
        {
            // Arrange
            var start = DateTime.UtcNow.Date;
            var end = start.AddDays(10);

            _rentalRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Rental, bool>>>(), It.IsAny<Expression<Func<Rental, object>>[]>()))
                .ReturnsAsync(new List<Rental>());

            // Act
            var result = await _rentalReportingService.GetMostRentedCarTypeAsync(start, end);

            // Assert
            result.CarType.Should().Be("None");
            result.UtilizationPercentage.Should().Be(0);
        }

        [Category("GetMostRentedCarTypeAsync")]
        [Test]
        public void GetMostRentedCarTypeAsync_ShouldThrowException_WhenEndDateIsBeforeStartDate()
        {
            // Arrange
            var start = DateTime.UtcNow.Date;
            var end = start.AddDays(-1);

            // Act
            Func<Task> act = async () => await _rentalReportingService.GetMostRentedCarTypeAsync(start, end);

            // Assert
            act.Should().ThrowAsync<InvalidRentDatesException>();
        }

        [Category("GetScheduledServicesNextTwoWeeksAsync")]
        [Test]
        public async Task GetScheduledServicesNextTwoWeeksAsync_ShouldReturnSortedServicesWithin2Weeks()
        {
            // Arrange
            var today = DateTime.UtcNow.Date;

            var cars = new List<Car>
            {
                new Car
                {
                    Id = Guid.NewGuid(),
                    Type = "SUV",
                    Model = "Honda CR-V",
                    Services = new List<Service>
                    {
                        new Service { StartDate = today.AddDays(10), DurationInDays = 2 }
                    }
                },
                new Car
                {
                    Id = Guid.NewGuid(),
                    Type = "Sedan",
                    Model = "Toyora Corolla",
                    Services = new List<Service>
                    {
                        new Service { StartDate = today.AddDays(1), DurationInDays = 1 }
                    }
                }
            };

            _carRepositoryMock
                .Setup(c => c.FindAsync(It.IsAny<Expression<Func<Car, bool>>>(), It.IsAny<Expression<Func<Car, object>>[]>()))
                .ReturnsAsync(cars);

            // Act
            var result = (await _rentalReportingService.GetScheduledServicesNextTwoWeeksAsync()).ToList();

            // Assert
            result.Should().HaveCount(2);
            result.Should().BeInAscendingOrder(x => x.ServiceStartDate);
        }

        [Category("GetScheduledServicesNextTwoWeeksAsync")]
        [Test]
        public async Task GetScheduledServicesNextTwoWeeksAsync_ShouldReturnEmpty_WhenNoServicesInNext2Weeks()
        {
            // Arrange
            _carRepositoryMock
                .Setup(c => c.FindAsync(It.IsAny<Expression<Func<Car, bool>>>(), It.IsAny<Expression<Func<Car, object>>[]>()))
                .ReturnsAsync(new List<Car>());

            // Act
            var result = await _rentalReportingService.GetScheduledServicesNextTwoWeeksAsync();

            // Assert
            result.Should().BeEmpty();
        }

    }
}