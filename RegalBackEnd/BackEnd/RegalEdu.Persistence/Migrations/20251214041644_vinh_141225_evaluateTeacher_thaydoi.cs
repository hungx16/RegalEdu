using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_141225_evaluateTeacher_thaydoi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvaluateContent",
                table: "EvaluateTeachers");

            migrationBuilder.DropColumn(
                name: "IsResponded",
                table: "EvaluateTeachers");

            migrationBuilder.DropColumn(
                name: "RatingScore",
                table: "EvaluateTeachers");

            migrationBuilder.DropColumn(
                name: "ResponseDate",
                table: "EvaluateTeachers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EvaluateContent",
                table: "EvaluateTeachers",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsResponded",
                table: "EvaluateTeachers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "RatingScore",
                table: "EvaluateTeachers",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ResponseDate",
                table: "EvaluateTeachers",
                type: "datetime2",
                nullable: true);
        }
    }
}
