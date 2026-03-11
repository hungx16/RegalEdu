using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_201225_CompanyEvent_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CompanyEvent",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CompanyEvent",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CompanyEvent",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "CompanyEvent",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CompanyEvent",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CompanyEvent",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CompanyEvent");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CompanyEvent");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CompanyEvent");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CompanyEvent");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CompanyEvent");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CompanyEvent");
        }
    }
}
