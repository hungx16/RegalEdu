using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Vu_UpdateStudent_12_01_25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AvatarImage",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "TotalAvailableAmount",
                table: "Students",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarImage",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "TotalAvailableAmount",
                table: "Students");
        }
    }
}
