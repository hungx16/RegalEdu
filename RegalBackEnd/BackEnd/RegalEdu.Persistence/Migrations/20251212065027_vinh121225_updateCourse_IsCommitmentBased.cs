using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh121225_updateCourse_IsCommitmentBased : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCommitmentBased",
                table: "Course",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCommitmentBased",
                table: "Course");
        }
    }
}
