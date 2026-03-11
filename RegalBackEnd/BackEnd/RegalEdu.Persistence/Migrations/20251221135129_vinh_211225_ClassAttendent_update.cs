using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_211225_ClassAttendent_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DisciplineLevel",
                table: "ClassAttendent",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LearningAbsorptionLevel",
                table: "ClassAttendent",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "ParticipationLevel",
                table: "ClassAttendent",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisciplineLevel",
                table: "ClassAttendent");

            migrationBuilder.DropColumn(
                name: "LearningAbsorptionLevel",
                table: "ClassAttendent");

            migrationBuilder.DropColumn(
                name: "ParticipationLevel",
                table: "ClassAttendent");
        }
    }
}
