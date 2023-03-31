using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BaseRest.Core.Model.Migrations
{
    public partial class NewMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true),
                    Password = table.Column<string>(type: "VARCHAR(128)", maxLength: 128, nullable: true),
                    CreatedBy = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true),
                    UpdatedBy = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true),
                    DeletedBy = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gender",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(128)", maxLength: 128, nullable: true),
                    CreatedBy = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true),
                    UpdatedBy = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true),
                    DeletedBy = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gender", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "VARCHAR(128)", maxLength: 128, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CUIL = table.Column<string>(type: "VARCHAR(16)", maxLength: 16, nullable: true),
                    GenderId = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "VARCHAR(32)", maxLength: 32, nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    CreatedBy = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true),
                    UpdatedBy = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true),
                    DeletedBy = table.Column<string>(type: "VARCHAR(64)", maxLength: 64, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Updated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Deleted = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customer_Gender_GenderId",
                        column: x => x.GenderId,
                        principalTable: "Gender",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AppUser",
                columns: new[] { "Id", "Created", "CreatedBy", "Deleted", "DeletedBy", "Name", "Password", "Updated", "UpdatedBy" },
                values: new object[] { 1, null, null, null, null, "admin", "admin", null, null });

            migrationBuilder.InsertData(
                table: "Gender",
                columns: new[] { "Id", "Created", "CreatedBy", "Deleted", "DeletedBy", "Name", "Updated", "UpdatedBy" },
                values: new object[,]
                {
                    { 1, new DateTime(2022, 11, 1, 1, 35, 24, 278, DateTimeKind.Local).AddTicks(2866), null, null, null, "Femenino", null, null },
                    { 2, new DateTime(2022, 11, 1, 1, 35, 24, 278, DateTimeKind.Local).AddTicks(2880), null, null, null, "Masculino", null, null },
                    { 3, new DateTime(2022, 11, 1, 1, 35, 24, 278, DateTimeKind.Local).AddTicks(2881), null, null, null, "Otro", null, null }
                });

            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "Id", "BirthDate", "CUIL", "Created", "CreatedBy", "Deleted", "DeletedBy", "GenderId", "Name", "Phone", "Updated", "UpdatedBy" },
                values: new object[] { 1, new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "12345678910", new DateTime(2022, 11, 1, 1, 35, 24, 276, DateTimeKind.Local).AddTicks(3922), null, null, null, 1, "Juana Maria Perez", "364412345678", null, null });

            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "Id", "BirthDate", "CUIL", "Created", "CreatedBy", "Deleted", "DeletedBy", "GenderId", "Name", "Phone", "Updated", "UpdatedBy" },
                values: new object[] { 2, new DateTime(1994, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "12345678910", new DateTime(2022, 11, 1, 1, 35, 24, 277, DateTimeKind.Local).AddTicks(2037), null, null, null, 2, "Carlos Canto", "364412345678", null, null });

            migrationBuilder.InsertData(
                table: "Customer",
                columns: new[] { "Id", "BirthDate", "CUIL", "Created", "CreatedBy", "Deleted", "DeletedBy", "GenderId", "Name", "Phone", "Updated", "UpdatedBy" },
                values: new object[] { 3, new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "12345678910", new DateTime(2022, 11, 1, 1, 35, 24, 277, DateTimeKind.Local).AddTicks(2056), null, null, null, 3, "Manuel Sosa", "364412345678", null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Customer_GenderId",
                table: "Customer",
                column: "GenderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUser");

            migrationBuilder.DropTable(
                name: "Customer");

            migrationBuilder.DropTable(
                name: "Gender");
        }
    }
}
