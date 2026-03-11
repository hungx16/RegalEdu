using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Hai_AddClassSeriesFieldsToClass_and_RemoveSubstituteTeacherId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubstituteTeacherId",
                table: "ClassSchedules");

            migrationBuilder.AddColumn<Guid>(
                name: "ClassSeriesId",
                table: "Class",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassSeriesId",
                table: "Class");

            migrationBuilder.AddColumn<Guid>(
                name: "SubstituteTeacherId",
                table: "ClassSchedules",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
