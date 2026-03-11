using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_05012026_phanbochitieu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WorkStage",
                table: "AdmissionsQuotaEmployee");

            migrationBuilder.RenameColumn(
                name: "ProbationEnd",
                table: "AdmissionsQuotaEmployee",
                newName: "AllocationStartAt");

            migrationBuilder.RenameColumn(
                name: "EndAt",
                table: "AdmissionsQuotaEmployee",
                newName: "AllocationEndAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AllocationStartAt",
                table: "AdmissionsQuotaEmployee",
                newName: "ProbationEnd");

            migrationBuilder.RenameColumn(
                name: "AllocationEndAt",
                table: "AdmissionsQuotaEmployee",
                newName: "EndAt");

            migrationBuilder.AddColumn<byte>(
                name: "WorkStage",
                table: "AdmissionsQuotaEmployee",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
