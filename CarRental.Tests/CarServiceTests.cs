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

namespace CarRental.Tests
{
    [TestFixture]
    public class CarServiceTests
    {
        private Mock<ICarRentalDbUnitOfWork> _unitOfWorkMock;
        private Mock<ICarRentalDbRepository<Car>> _carRepositoryMock;
        private CarService _carService;

        [SetUp]
        public void Setup()
        {
            _carRepositoryMock = new Mock<ICarRentalDbRepository<Car>>();
            _unitOfWorkMock = new Mock<ICarRentalDbUnitOfWork>();
            _unitOfWorkMock.SetupProperty(p => p.Cars, _carRepositoryMock.Object);
            _carService = new CarService(_unitOfWorkMock.Object);
        }
        [Category("Car Service")]
        [Test]
        public async Task GetCarTypes_ShouldReturnDistinctTypesFromDatabase()
        {
            // Arrange
            var cars = new List<Car>
            {
                new Car { Type = "Sedan", Model = "Toyota Corolla" },
                new Car { Type = "SUV", Model = "Honda CR-V" },
                new Car { Type = "Sedan", Model = "Volkswagen Vento" }
            };

            _carRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>()))
                .ReturnsAsync(cars);

            // Act
            var result = await _carService.GetCarTypes();

            // Assert
            result.Should().BeEquivalentTo(new[] { "Sedan", "SUV" });
        }

        [Category("Car Service")]
        [Test]
        public async Task GetCarModels_ShouldReturnDistinctModelsFromDatabase()
        {
            // Arrange
            var cars = new List<Car>
            {
                new Car { Type = "Sedan", Model = "Toyota Corolla" },
                new Car { Type = "SUV", Model = "Honda CR-V" },
                new Car { Type = "Sedan", Model = "Volkswagen Vento" }
            };


            _carRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>()))
                .ReturnsAsync(cars);

            // Act
            var result = await _carService.GetCarModels();

            // Assert
            result.Should().BeEquivalentTo(new[] { "Toyota Corolla", "Honda CR-V", "Volkswagen Vento" });
        }

        [Category("Car Service")]
        [Test]
        public async Task GetCarTypes_ShouldReturnEmptyList_WhenNoCarsExist()
        {
            // Arrange
            _carRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>()))
                .ReturnsAsync(new List<Car>());

            // Act
            var result = await _carService.GetCarTypes();

            // Assert
            result.Should().BeEmpty();
        }

        [Category("Car Service")]
        [Test]
        public async Task GetCarModels_ShouldReturnEmptyList_WhenNoCarsExist()
        {
            // Arrange
            _carRepositoryMock
                .Setup(r => r.FindAsync(It.IsAny<Expression<Func<Car, bool>>>()))
                .ReturnsAsync(new List<Car>());

            // Act
            var result = await _carService.GetCarModels();

            // Assert
            result.Should().BeEmpty();
        }
    }
}