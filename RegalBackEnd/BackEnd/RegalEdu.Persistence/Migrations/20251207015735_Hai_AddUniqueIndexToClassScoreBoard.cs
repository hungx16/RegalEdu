using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    public partial class Hai_AddUniqueIndexToClassScoreBoard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ClassScoreBoard_Unique",
                table: "ClassScoreBoard",
                columns: new[] { "ClassId", "StudentId", "ScoreType", "CategoryId" },
                unique: true
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClassScoreBoard_Unique",
                table: "ClassScoreBoard"
            );
        }
    }
}
