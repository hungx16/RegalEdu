using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_141225_evaluateTeacher_ratingStar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAnonymous",
                table: "EvaluateTeachers");

            migrationBuilder.AlterColumn<double>(
                name: "StarRating",
                table: "EvaluateTeachers",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "RatingScore",
                table: "EvaluateTeachers",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StarRating",
                table: "EvaluateTeachers",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RatingScore",
                table: "EvaluateTeachers",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<bool>(
                name: "IsAnonymous",
                table: "EvaluateTeachers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
