## Car Rental API

This repository contains a solution for the Car Rental Api Challenge. It manages a car rental system with features including registering customers, renting cars, listing existing rentals, modifying and cancelling rentals, basic service maintenance tracking and reporting.

## Solution Architecture

The solution is built with .NET 8 and follows a layered architecture:

- CarRental.Api: Hosts the API controllers, configuration, and dependency injection setup.

- CarRental.Domain: Contains business logic, domain models, interfaces, custom exceptions, etc

- CarRental.DB: Manages data access using Entity Framework Core with SQL Server.

- CarRental.Tests: Includes unit tests for the domain layer using NUnit, Moq and FluentAssertion.

## How to Run Locally

- Clone the Repository
    
    git clone https://github.com/estebansager/pwc-carrental-api.git

- Set Up the Database
    
    Considering the context of the code challenge, I have used a local Sql Server connection and the connection string is stored in the appsettings.json file in plain text. This should not be the case for Production, where any type of connection string should be encrypted or moved to a safer place like Secrets Manager in AWS.

    With that being said, ensure you have a SQL Server instance running and update the connection string in CarRental.Api/appsettings.json to point to your SQL Server instance if needed.


- Apply migrations and seed the database

    The DB was created with a Code First approach. I have included the Migrations folder with the initial script file that can be run with: dotnet ef database update --project CarRental.DB --startup-project CarRental.Api

    I also included a .sql script that can be used as a second option for creating the DB if running the script directly in SQL Server is preferred (Path: CarRental.DB/Migrations/InitialCreate.sql)
    
    Either option will create the necessary tables and seed initial data, including predefined cars and service records.

- Run the Application

    You can use Visual Studio to run the application, which will be launched in https://localhost:7137
    You can check the Swagger UI spec page with the endpoints in https://localhost:7137/swagger/index.html

## Assumptions and Settings

    Service Scheduling: Each car undergoes maintenance every two months, lasting two days. Cars become available for rental one day after service completion.

    Database Seeding: The initial migration seeds the database with a predefined set of cars and associated service records.

    Authentication: Not implemented, as per the challenge requirements. Would be ideal to implement in a future iteration, thinking about a login and users for the app.

## Caching
    
    To enhance read performance, I decided to put in place a basic caching system with .NET MemoryCache. In a real case it is suggested to move it to a distributed system like Redis.

## Race Conditions
    
    I have spotted one place in the Api where race conditions could happen, and that is when trying to rent a car. If two concurrent requests try to rent the same car, we could have a data problem. To solve this, I have decided to use transactions via the Unit Of Work with Isolation Level Serializable in the routine that rents a car, to make sure the impacted tables are locked and race conditions are prevented. This is done in the ExecuteTransactionAsync method of ICarRentalDbUnitOfWork interface and respective implementation.


## Sample requests 

- I have included a Postman collection file (CarRental.postman_collection.json) that you can import to test the endpoints.



