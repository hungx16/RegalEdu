using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationLocalizationFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MessageEn",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageVi",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleEn",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TitleVi",
                table: "Notification",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageEn",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "MessageVi",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "TitleEn",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "TitleVi",
                table: "Notification");
        }
    }
}
