using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BaseRest.Core.Model.Migrations
{
    public partial class NewMigration5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2023, 4, 5, 18, 18, 50, 591, DateTimeKind.Local).AddTicks(3793));

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2023, 4, 5, 18, 18, 50, 592, DateTimeKind.Local).AddTicks(2204));

            migrationBuilder.UpdateData(
                table: "Customer",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2023, 4, 5, 18, 18, 50, 592, DateTimeKind.Local).AddTicks(2222));

            migrationBuilder.UpdateData(
                table: "Gender",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created",
                value: new DateTime(2023, 4, 5, 18, 18, 50, 593, DateTimeKind.Local).AddTicks(3325));

            migrationBuilder.UpdateData(
                table: "Gender",
                keyColumn: "Id",
                keyValue: 2,
                column: "Created",
                value: new DateTime(2023, 4, 5, 18, 18, 50, 593, DateTimeKind.Local).AddTicks(3335));

            migrationBuilder.UpdateData(
                table: "Gender",
                keyColumn: "Id",
                keyValue: 3,
                column: "Created",
                value: new DateTime(2023, 4, 5, 18, 18, 50, 593, DateTimeKind.Local).AddTicks(3336));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
