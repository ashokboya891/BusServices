using BusServices.Helpers;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Text.Json;

#nullable disable

namespace BusServices.Migrations
{
    /// <inheritdoc />
    public partial class AddSeatLayoutJsonToBusTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatLayoutJson",
                table: "BusTypes",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "BusImages",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.UpdateData(
                table: "BusTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "SeatLayoutJson",
                value: "{}");

            migrationBuilder.UpdateData(
                table: "BusTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "SeatLayoutJson",
                value: "{}");

            migrationBuilder.UpdateData(
                table: "BusTypes",
                keyColumn: "Id",
                keyValue: 3,
                column: "SeatLayoutJson",
                value: "{}");

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

            // 3️⃣ Update existing BusTypes rows with actual seat layout
            migrationBuilder.Sql($"""
    UPDATE "BusTypes"
    SET "SeatLayoutJson" = '{JsonSerializer.Serialize(SeatLayouts.Sleeper(30))}'::jsonb
    WHERE "Id" = 5;
    """);

            migrationBuilder.Sql($"""
    UPDATE "BusTypes"
    SET "SeatLayoutJson" = '{JsonSerializer.Serialize(SeatLayouts.Seater(40))}'::jsonb
    WHERE "Id" = 1;
    """);

            migrationBuilder.Sql($"""
    UPDATE "BusTypes"
    SET "SeatLayoutJson" = '{JsonSerializer.Serialize(SeatLayouts.SemiSleeper(35))}'::jsonb
    WHERE "Id" = 6;
    """);

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatLayoutJson",
                table: "BusTypes");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "BusImages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "TravelDate",
                value: new DateTime(2025, 8, 30, 12, 5, 19, 887, DateTimeKind.Utc).AddTicks(8181));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2,
                column: "TravelDate",
                value: new DateTime(2025, 9, 1, 12, 5, 19, 887, DateTimeKind.Utc).AddTicks(8202));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3,
                column: "TravelDate",
                value: new DateTime(2025, 8, 29, 12, 5, 19, 887, DateTimeKind.Utc).AddTicks(8207));
        }
    }
}
