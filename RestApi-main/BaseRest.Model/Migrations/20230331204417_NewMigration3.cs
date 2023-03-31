using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BaseRest.Core.Model.Migrations
{
    public partial class NewMigration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2023, 3, 31, 17, 44, 16, 752, DateTimeKind.Local).AddTicks(1702));

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2023, 3, 31, 17, 44, 16, 752, DateTimeKind.Local).AddTicks(9965));

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2023, 3, 31, 17, 44, 16, 752, DateTimeKind.Local).AddTicks(9981));

            migrationBuilder.UpdateData(
                table: "Gender",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2023, 3, 31, 17, 44, 16, 754, DateTimeKind.Local).AddTicks(334));

            migrationBuilder.UpdateData(
                table: "Gender",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2023, 3, 31, 17, 44, 16, 754, DateTimeKind.Local).AddTicks(343));

            migrationBuilder.UpdateData(
                table: "Gender",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2023, 3, 31, 17, 44, 16, 754, DateTimeKind.Local).AddTicks(344));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2022, 11, 1, 1, 35, 24, 276, DateTimeKind.Local).AddTicks(3922));

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2022, 11, 1, 1, 35, 24, 277, DateTimeKind.Local).AddTicks(2037));

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2022, 11, 1, 1, 35, 24, 277, DateTimeKind.Local).AddTicks(2056));

            migrationBuilder.UpdateData(
                table: "Gender",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2022, 11, 1, 1, 35, 24, 278, DateTimeKind.Local).AddTicks(2866));

            migrationBuilder.UpdateData(
                table: "Gender",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2022, 11, 1, 1, 35, 24, 278, DateTimeKind.Local).AddTicks(2880));

            migrationBuilder.UpdateData(
                table: "Gender",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2022, 11, 1, 1, 35, 24, 278, DateTimeKind.Local).AddTicks(2881));
        }
    }
}
