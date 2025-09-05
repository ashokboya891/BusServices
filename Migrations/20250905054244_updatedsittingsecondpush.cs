using BusServices.Helpers;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Text.Json;

#nullable disable

namespace BusServices.Migrations
{
    /// <inheritdoc />
    public partial class updatedsittingsecondpush : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TotalSeats",
                table: "Events",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "AvailableSeats",
                table: "Events",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AvailableSeats", "TotalSeats", "TravelDate" },
                values: new object[] { 0, 0, new DateTime(2025, 9, 8, 5, 42, 44, 389, DateTimeKind.Utc).AddTicks(5785) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AvailableSeats", "TotalSeats", "TravelDate" },
                values: new object[] { 0, 0, new DateTime(2025, 9, 10, 5, 42, 44, 389, DateTimeKind.Utc).AddTicks(5798) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AvailableSeats", "TotalSeats", "TravelDate" },
                values: new object[] { 0, 0, new DateTime(2025, 9, 7, 5, 42, 44, 389, DateTimeKind.Utc).AddTicks(5801) });
            // Dictionary of Id → Name
            var busTypes = new Dictionary<int, string>
    {
        {1, "AC 2+2 Seater"},
        {2, "AC Sleeper"},
        {3, "Volvo Express"},
        {4, "Intercity"},
        {5, "garuda"},
        {6, "indra"},
        {7, "Express"},
        {8, "Deluxe"},
        {9, "Courier(heavy)"},
        {10, "Night Sleeper"},
        {12, "Economy Seater"},
        {13, "Night Express"}
    };

            foreach (var bt in busTypes)
            {
                var layout = JsonSerializer.Serialize(SeatLayouts.Generate(bt.Value));
                migrationBuilder.Sql($"""
            UPDATE "BusTypes"
            SET "SeatLayoutJson" = '{layout}'::jsonb
            WHERE "Id" = {bt.Key};
        """);
            }
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "TotalSeats",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<int>(
                name: "AvailableSeats",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AvailableSeats", "TotalSeats", "TravelDate" },
                values: new object[] { 35, 40, new DateTime(2025, 9, 7, 7, 14, 8, 641, DateTimeKind.Utc).AddTicks(3267) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AvailableSeats", "TotalSeats", "TravelDate" },
                values: new object[] { 28, 30, new DateTime(2025, 9, 9, 7, 14, 8, 641, DateTimeKind.Utc).AddTicks(3280) });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AvailableSeats", "TotalSeats", "TravelDate" },
                values: new object[] { 40, 45, new DateTime(2025, 9, 6, 7, 14, 8, 641, DateTimeKind.Utc).AddTicks(3283) });
        }
    }
}
