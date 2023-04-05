using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BaseRest.Core.Model.Migrations
{
    public partial class NewMigration4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2023, 4, 5, 18, 15, 16, 815, DateTimeKind.Local).AddTicks(6663));

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2023, 4, 5, 18, 15, 16, 817, DateTimeKind.Local).AddTicks(1221));

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2023, 4, 5, 18, 15, 16, 817, DateTimeKind.Local).AddTicks(1236));

            migrationBuilder.UpdateData(
                table: "Gender",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2023, 4, 5, 18, 15, 16, 818, DateTimeKind.Local).AddTicks(2197));

            migrationBuilder.UpdateData(
                table: "Gender",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2023, 4, 5, 18, 15, 16, 818, DateTimeKind.Local).AddTicks(2205));

            migrationBuilder.UpdateData(
                table: "Gender",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2023, 4, 5, 18, 15, 16, 818, DateTimeKind.Local).AddTicks(2207));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
