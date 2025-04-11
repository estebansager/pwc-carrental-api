using CarRental.Api.Middlewares;
using CarRental.DB.Contexts;
using CarRental.DB.Entities;
using CarRental.DB.Repositories;
using CarRental.Domain.Models.Cars;
using CarRental.Domain.Models.Customers;
using CarRental.Domain.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



ConfigureDatabaseAccess();
ConfigureDependencies();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//for simplicity on testing the app, this line is commented
//app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthorization();

app.MapControllers();

app.Run();



void ConfigureDependencies()
{

    /*DB*/
    builder.Services.AddScoped<DbContext, CarRentalDbContext>();
    builder.Services.AddScoped<ICarRentalDbRepository<Car>, CarRentalDbRepository<Car>>();
    builder.Services.AddScoped<ICarRentalDbRepository<Service>, CarRentalDbRepository<Service>>();
    builder.Services.AddScoped<ICarRentalDbRepository<Rental>, CarRentalDbRepository<Rental>>();
    builder.Services.AddScoped<ICarRentalDbRepository<Customer>, CarRentalDbRepository<Customer>>();
    builder.Services.AddScoped<ICarRentalDbUnitOfWork, CarRentalDbUnitOfWork>();


    /*Services*/
    builder.Services.AddScoped<ICarRentalService, CarRentalService>();
    builder.Services.AddScoped<ICustomerService, CustomerService>();
    builder.Services.AddScoped<IRentalReportingService, RentalReportingService>();

}


/* Database configurations */
void ConfigureDatabaseAccess()
{
    builder.Services.AddDbContext<CarRentalDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetValue<string>("DB:ConnectionString")
    )
);

}
