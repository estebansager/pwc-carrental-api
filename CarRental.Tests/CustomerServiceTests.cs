using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CarRental.DB.Entities;
using CarRental.DB.Repositories;
using CarRental.Domain.Mappers;
using CarRental.Domain.Models.Customers;
using CarRental.Domain.Services;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CarRental.Tests.Domain.Services
{
    [TestFixture]
    public class CustomerServiceTests
    {
        private Mock<ICarRentalDbUnitOfWork> _unitOfWorkMock;
        private Mock<ICarRentalDbRepository<Customer>> _customerRepositoryMock;
        private CustomerService _customerService;

        [SetUp]
        public void SetUp()
        {
            _customerRepositoryMock = new Mock<ICarRentalDbRepository<Customer>>();
            _unitOfWorkMock = new Mock<ICarRentalDbUnitOfWork>();
            _unitOfWorkMock.SetupProperty(p => p.Customers, _customerRepositoryMock.Object);

            _customerService = new CustomerService(_unitOfWorkMock.Object);
        }

        [Category("RegisterCustomerAsync")]
        [Test]
        public async Task RegisterCustomerAsync_ShouldReturnCustomerDto_WhenValidCustomerDetails()
        {
            // Arrange
            var personalIdNumber = 12345;
            var fullName = "Esteban Sager";
            var address = "Calle Falsa 123";

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                PersonalIdNumber = personalIdNumber,
                FullName = fullName,
                Address = address
            };

            _unitOfWorkMock.Setup(uow => uow.Customers.AddAsync(It.IsAny<Customer>())).Returns(Task.CompletedTask);
            
            // Act
            var result = await _customerService.RegisterCustomerAsync(personalIdNumber, fullName, address);

            // Assert
            result.Should().NotBeNull();
            result.FullName.Should().Be(fullName);
            result.PersonalIdNumber.Should().Be(personalIdNumber);
            result.Address.Should().Be(address);
        }

        [Category("GerCustomerAsync")]
        [Test]
        public async Task GetCustomerAsync_ShouldReturnCustomerDto_WhenCustomerExists()
        {
            // Arrange
            var personalIdNumber = 12345;
            var fullName = "Esteban Sager";
            var address = "Calle Falsa 123";

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                PersonalIdNumber = personalIdNumber,
                FullName = fullName,
                Address = address
            };

            _unitOfWorkMock.Setup(uow => uow.Customers.FindAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new[] { customer });

            // Act
            var result = await _customerService.GetCustomerAsync(personalIdNumber);

            // Assert
            result.Should().NotBeNull();
            result.FullName.Should().Be(fullName);
            result.PersonalIdNumber.Should().Be(personalIdNumber);
            result.Address.Should().Be(address);
        }

        [Category("GerCustomerAsync")]
        [Test]
        public async Task GetCustomerAsync_ShouldReturnNull_WhenCustomerNotFound()
        {
            // Arrange
            var personalIdNumber = 12345;

            _unitOfWorkMock.Setup(uow => uow.Customers.FindAsync(It.IsAny<Expression<Func<Customer, bool>>>()))
                .ReturnsAsync(new List<Customer>());

            // Act
            var result = await _customerService.GetCustomerAsync(personalIdNumber);

            // Assert
            result.Should().BeNull();
        }

    }
}