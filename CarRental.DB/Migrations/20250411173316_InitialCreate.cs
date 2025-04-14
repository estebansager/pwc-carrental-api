using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.DB.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TABLE IF EXISTS Rentals;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS Services;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS Customers;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS Cars;");

            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonalIdNumber = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DurationInDays = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Services_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rentals_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rentals_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CarId",
                table: "Rentals",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CustomerId",
                table: "Rentals",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_CarId",
                table: "Services",
                column: "CarId");


            migrationBuilder.InsertData(table: "Cars",
                columns: new[] { "Id", "Type", "Model" },
                values: new object[,]
                {
                    { "00000000-0000-0000-0000-000000000000", "Sedan", "Toyota Corolla" },
                    { "6F9619FF-8B86-D011-B42D-00CF4FC964FF", "Sedan", "Toyota Corolla" },
                    { "3F2504E0-4F89-11D3-9A0C-0305E82C3301", "Sedan", "Toyota Corolla" },
                    { "48284652-77ED-4872-996E-04BB0F0030AE", "SUV", "Honda CR-V" },
                    { "C9BF9E57-1685-4C89-BAAF-07DA1C8D8F9E", "SUV", "Honda CR-V" },
                    { "9A499940-92D4-4A3F-A6B1-6D192F56106D", "SUV", "Honda CR-V" },
                    { "B8C26292-DED9-4031-BB3D-92436E72E152", "SUV", "Honda CR-V" },
                    { "7AC5C6D2-CD71-4F2F-81B5-13701F6FE99F", "Hatchback", "Volkswagen Gol" },
                    { "52F06FA4-4C87-4F5A-8D27-179BC83B98A6", "Hatchback", "Volkswagen Gol" },
                    { "846BDF8F-C8B2-4A44-A7E1-9E258679F366", "Hatchback", "Volkswagen Gol" },
                    { "B5A3F8DE-A57D-4A5B-9A3F-DEC43FDE2CF8", "Hatchback", "Volkswagen Gol" },
                    { "9276F7CB-8A29-4E5E-8727-23FC93E24319", "Pickup", "Ford Ranger" },
                    { "B47F2C19-3C44-4DC4-94D1-F1B5EFAB7B0A", "Pickup", "Ford Ranger" },
                    { "268A1E25-E91F-4B4A-9C4B-59CFE7634764", "Pickup", "Volkswagen Amarok" },
                    { "FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF", "Pickup", "Volkswagen Amarok" },
                    { "10FC3540-6D9B-4968-B648-5DB508A2296A", "Sedan", "Volkswagen Vento" },
                    { "7D444840-9DC0-11D1-B245-5FFDCE74FAD2", "Sedan", "Volkswagen Vento" },
                    { "C111402F-ECF3-4FE6-8421-6AEB117A2939", "Sedan", "Volkswagen Vento" },
                });

            migrationBuilder.InsertData(table: "Services",
                columns: new[] { "Id", "StartDate", "CarId", "DurationInDays", "IsCompleted" },
                values: new object[,]
                {
                    { "11111111-1111-1111-1111-111111111111", "2025-03-30", "00000000-0000-0000-0000-000000000000", 2, true },
                    { "22222222-2222-2222-2222-222222222222", "2025-03-30", "6F9619FF-8B86-D011-B42D-00CF4FC964FF", 2, true },
                    { "33333333-3333-3333-3333-333333333333", "2025-04-20", "3F2504E0-4F89-11D3-9A0C-0305E82C3301", 2, false },
                    { "44444444-4444-4444-4444-444444444444", "2025-04-20", "48284652-77ED-4872-996E-04BB0F0030AE", 2, false },
                    {"55555555-5555-5555-5555-555555555555", "2025-04-20", "C9BF9E57-1685-4C89-BAAF-07DA1C8D8F9E", 2, false },
                    { "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "2025-04-20", "9A499940-92D4-4A3F-A6B1-6D192F56106D", 2, false },
                    { "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb", "2025-04-25", "B8C26292-DED9-4031-BB3D-92436E72E152", 2, false },
                    {"cccccccc-cccc-cccc-cccc-cccccccccccc", "2025-04-25", "7AC5C6D2-CD71-4F2F-81B5-13701F6FE99F", 2, false },
                    { "dddddddd-dddd-dddd-dddd-dddddddddddd", "2025-04-30", "52F06FA4-4C87-4F5A-8D27-179BC83B98A6", 2, false },
                    { "eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee", "2025-05-02", "846BDF8F-C8B2-4A44-A7E1-9E258679F366", 2, false },
                    { "ffffffff-ffff-ffff-ffff-ffffffffffff", "2025-05-02", "B5A3F8DE-A57D-4A5B-9A3F-DEC43FDE2CF8", 2, false }

                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Cars");
        }
    }
}
