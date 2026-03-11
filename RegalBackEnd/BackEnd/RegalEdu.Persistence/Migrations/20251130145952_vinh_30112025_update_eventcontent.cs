using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_30112025_update_eventcontent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventContent",
                table: "CompanyEvent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventContent",
                table: "CompanyEvent",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
