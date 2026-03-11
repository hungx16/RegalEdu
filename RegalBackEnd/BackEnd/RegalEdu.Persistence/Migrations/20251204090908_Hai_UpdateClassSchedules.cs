using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Hai_UpdateClassSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSchedules_Employees_Attender",
                table: "ClassSchedules");

            migrationBuilder.DropIndex(
                name: "IX_ClassSchedules_Attender",
                table: "ClassSchedules");

            migrationBuilder.DropColumn(
                name: "Attender",
                table: "ClassSchedules");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Attender",
                table: "ClassSchedules",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_Attender",
                table: "ClassSchedules",
                column: "Attender");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSchedules_Employees_Attender",
                table: "ClassSchedules",
                column: "Attender",
                principalTable: "Employees",
                principalColumn: "Id");
        }
    }
}
