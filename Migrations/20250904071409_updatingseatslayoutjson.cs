using BusServices.Helpers;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Text.Json;

#nullable disable

namespace BusServices.Migrations
{
    /// <inheritdoc />
    public partial class updatingseatslayoutjson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "TravelDate",
                value: new DateTime(2025, 9, 7, 7, 14, 8, 641, DateTimeKind.Utc).AddTicks(3267));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2,
                column: "TravelDate",
                value: new DateTime(2025, 9, 9, 7, 14, 8, 641, DateTimeKind.Utc).AddTicks(3280));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3,
                column: "TravelDate",
                value: new DateTime(2025, 9, 6, 7, 14, 8, 641, DateTimeKind.Utc).AddTicks(3283));

            migrationBuilder.Sql($"""
UPDATE "BusTypes"
SET "SeatLayoutJson" = '{JsonSerializer.Serialize(SeatLayouts.Sleeper(30))}'::jsonb
WHERE "Id" = 2;
""");

            migrationBuilder.Sql($"""
UPDATE "BusTypes"
SET "SeatLayoutJson" = '{JsonSerializer.Serialize(SeatLayouts.Seater(45))}'::jsonb
WHERE "Id" = 3;
""");

            migrationBuilder.Sql($"""
UPDATE "BusTypes"
SET "SeatLayoutJson" = '{JsonSerializer.Serialize(SeatLayouts.SemiSleeper(40))}'::jsonb
WHERE "Id" = 4;
""");

            migrationBuilder.Sql($"""
UPDATE "BusTypes"
SET "SeatLayoutJson" = '{JsonSerializer.Serialize(SeatLayouts.SemiSleeper(50))}'::jsonb
WHERE "Id" = 7;
""");

            migrationBuilder.Sql($"""
UPDATE "BusTypes"
SET "SeatLayoutJson" = '{JsonSerializer.Serialize(SeatLayouts.SemiSleeper(28))}'::jsonb
WHERE "Id" = 8;
""");

            migrationBuilder.Sql($"""
UPDATE "BusTypes"
SET "SeatLayoutJson" = '{JsonSerializer.Serialize(SeatLayouts.SemiSleeper(20))}'::jsonb
WHERE "Id" = 9;
""");

            migrationBuilder.Sql($"""
UPDATE "BusTypes"
SET "SeatLayoutJson" = '{JsonSerializer.Serialize(SeatLayouts.SemiSleeper(30))}'::jsonb
WHERE "Id" = 10;
""");

            migrationBuilder.Sql($"""
UPDATE "BusTypes"
SET "SeatLayoutJson" = '{JsonSerializer.Serialize(SeatLayouts.SemiSleeper(45))}'::jsonb
WHERE "Id" = 12;
""");

            migrationBuilder.Sql($"""
UPDATE "BusTypes"
SET "SeatLayoutJson" = '{JsonSerializer.Serialize(SeatLayouts.SemiSleeper(40))}'::jsonb
WHERE "Id" = 13;
""");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "TravelDate",
                value: new DateTime(2025, 9, 7, 7, 7, 42, 387, DateTimeKind.Utc).AddTicks(2220));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2,
                column: "TravelDate",
                value: new DateTime(2025, 9, 9, 7, 7, 42, 387, DateTimeKind.Utc).AddTicks(2236));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3,
                column: "TravelDate",
                value: new DateTime(2025, 9, 6, 7, 7, 42, 387, DateTimeKind.Utc).AddTicks(2240));
        }
    }
}
