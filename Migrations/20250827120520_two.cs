using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusServices.Migrations
{
    /// <inheritdoc />
    public partial class two : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Caption",
                table: "BusImages");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "BusImages",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "BusImages",
                newName: "PhotoId");

            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "BusImages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "BusImages",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "BusImages",
                keyColumn: "PhotoId",
                keyValue: 1,
                columns: new[] { "IsPrimary", "PublicId" },
                values: new object[] { true, "OIP_ma4xfs" });

            migrationBuilder.UpdateData(
                table: "BusImages",
                keyColumn: "PhotoId",
                keyValue: 2,
                columns: new[] { "IsPrimary", "PublicId" },
                values: new object[] { false, "OIP_3_jqikgp" });

            migrationBuilder.UpdateData(
                table: "BusImages",
                keyColumn: "PhotoId",
                keyValue: 3,
                columns: new[] { "EventId", "IsPrimary", "PublicId" },
                values: new object[] { 3, true, "OIP_2_wlazak" });

            migrationBuilder.UpdateData(
                table: "BusImages",
                keyColumn: "PhotoId",
                keyValue: 4,
                columns: new[] { "EventId", "IsPrimary", "PublicId" },
                values: new object[] { 4, true, "bus_ifrwmm" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "BusImages");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "BusImages");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "BusImages",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "PhotoId",
                table: "BusImages",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Caption",
                table: "BusImages",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "BusImages",
                keyColumn: "Id",
                keyValue: 1,
                column: "Caption",
                value: "Front View - Seater Bus");

            migrationBuilder.UpdateData(
                table: "BusImages",
                keyColumn: "Id",
                keyValue: 2,
                column: "Caption",
                value: "Inside View - Seater Bus");

            migrationBuilder.UpdateData(
                table: "BusImages",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Caption", "EventId" },
                values: new object[] { "Side View - Sleeper Bus", 2 });

            migrationBuilder.UpdateData(
                table: "BusImages",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Caption", "EventId" },
                values: new object[] { "Front View - Volvo Express", 3 });

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 1,
                column: "TravelDate",
                value: new DateTime(2025, 8, 29, 6, 0, 40, 298, DateTimeKind.Utc).AddTicks(8269));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 2,
                column: "TravelDate",
                value: new DateTime(2025, 8, 31, 6, 0, 40, 298, DateTimeKind.Utc).AddTicks(8280));

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Id",
                keyValue: 3,
                column: "TravelDate",
                value: new DateTime(2025, 8, 28, 6, 0, 40, 298, DateTimeKind.Utc).AddTicks(8285));
        }
    }
}
