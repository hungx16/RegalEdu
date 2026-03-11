using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_02122025_update_gopcode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "StudentInClass");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable (
                name: "StudentInClass",
                columns: table => new
                {
                    Id = table.Column<Guid> (type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid> (type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime> (type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string> (type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool> (type: "bit", nullable: false),
                    Status = table.Column<int> (type: "int", nullable: false),
                    StudentId = table.Column<Guid> (type: "uniqueidentifier", nullable: false),
                    UpdatedAt = table.Column<DateTime> (type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string> (type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey ("PK_StudentInClass", x => x.Id);
                });

            migrationBuilder.CreateIndex (
                name: "IX_StudentInClass_ClassId",
                table: "StudentInClass",
                column: "ClassId");

            migrationBuilder.CreateIndex (
                name: "IX_StudentInClass_StudentId",
                table: "StudentInClass",
                column: "StudentId");
        }
    }
}
