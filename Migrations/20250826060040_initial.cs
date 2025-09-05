using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BusServices.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    DefaultSeats = table.Column<int>(type: "integer", nullable: false),
                    Features = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    Owner = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OwnerContact = table.Column<long>(type: "bigint", nullable: false),
                    BusCategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusTypes_BusCategory_BusCategoryId",
                        column: x => x.BusCategoryId,
                        principalTable: "BusCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    FromPlace = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ToPlace = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TravelDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalSeats = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    AvailableSeats = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Price = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0.0m),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "Upcoming"),
                    Organizer = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ContactInfo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    BusTypeId = table.Column<int>(type: "integer", nullable: false),
                    BusCategoryId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_BusCategory_BusCategoryId",
                        column: x => x.BusCategoryId,
                        principalTable: "BusCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Events_BusTypes_BusTypeId",
                        column: x => x.BusTypeId,
                        principalTable: "BusTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ImageUrl = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Caption = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EventId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusImages_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BusCategory",
                columns: new[] { "Id", "Description", "IsActive", "Name" },
                values: new object[,]
                {
                    { 1, "Standard seater buses", true, "Seater" },
                    { 2, "Comfortable sleeper buses", true, "Sleeper" },
                    { 3, "High speed express buses", true, "Express" }
                });

            migrationBuilder.InsertData(
                table: "BusTypes",
                columns: new[] { "Id", "BusCategoryId", "DefaultSeats", "Description", "Features", "IsActive", "Name", "Owner", "OwnerContact" },
                values: new object[,]
                {
                    { 1, 1, 40, "40 Seater AC Bus with 2+2 layout", "AC, WiFi, Charging Port", true, "AC 2+2 Seater", "ABC Travels", 9876543210L },
                    { 2, 2, 30, "30 Sleeper AC Bus with comfortable beds", "AC, Blanket, Charging Port", true, "AC Sleeper", "XYZ Travels", 9123456780L },
                    { 3, 3, 45, "Luxury Volvo Express Bus", "AC, WiFi, TV, Charging Port", true, "Volvo Express", "PQR Travels", 9988776655L }
                });

            migrationBuilder.InsertData(
                table: "Events",
                columns: new[] { "Id", "AvailableSeats", "BusCategoryId", "BusTypeId", "ContactInfo", "Description", "FromPlace", "IsActive", "Organizer", "Price", "Status", "Title", "ToPlace", "TotalSeats", "TravelDate" },
                values: new object[,]
                {
                    { 1, 35, 1, 1, "abc@gmail.com", "Comfortable overnight journey", "Hyderabad", true, "ABC Travels", 1200.50m, "Upcoming", "Hyderabad to Bangalore", "Bangalore", 40, new DateTime(2025, 8, 29, 6, 0, 40, 298, DateTimeKind.Utc).AddTicks(8269) },
                    { 2, 28, 2, 2, "xyz@gmail.com", "Luxury sleeper bus service", "Chennai", true, "XYZ Travels", 1800.75m, "Upcoming", "Chennai to Pune", "Pune", 30, new DateTime(2025, 8, 31, 6, 0, 40, 298, DateTimeKind.Utc).AddTicks(8280) },
                    { 3, 40, 3, 3, "pqr@gmail.com", "Fast express Volvo service", "Delhi", true, "PQR Travels", 900.00m, "Upcoming", "Delhi to Jaipur", "Jaipur", 45, new DateTime(2025, 8, 28, 6, 0, 40, 298, DateTimeKind.Utc).AddTicks(8285) }
                });

            migrationBuilder.InsertData(
                table: "BusImages",
                columns: new[] { "Id", "Caption", "EventId", "ImageUrl" },
                values: new object[,]
                {
                    { 1, "Front View - Seater Bus", 1, "https://ts1.explicit.bing.net/th?id=OIP.7IFURQmvycxLNYYig8TiSgHaE8&pid=15.1" },
                    { 2, "Inside View - Seater Bus", 1, "https://thfvnext.bing.com/th/id/OIP.TasbJC0pWiRaD-8dTvgi8wHaGW?w=226&h=193&c=7&r=0&o=7&cb=thfvnext&dpr=1.3&pid=1.7&rm=3" },
                    { 3, "Side View - Sleeper Bus", 2, "https://thfvnext.bing.com/th/id/OIP.Z4oSl51ufLg-gKLKOAwgvwHaEK?w=333&h=187&c=7&r=0&o=7&cb=thfvnext&dpr=1.3&pid=1.7&rm=3" },
                    { 4, "Front View - Volvo Express", 3, "https://picsum.photos/seed/bus4/600/400" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusImages_EventId",
                table: "BusImages",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_BusTypes_BusCategoryId",
                table: "BusTypes",
                column: "BusCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_BusCategoryId",
                table: "Events",
                column: "BusCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_BusTypeId",
                table: "Events",
                column: "BusTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusImages");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "BusTypes");

            migrationBuilder.DropTable(
                name: "BusCategory");
        }
    }
}
