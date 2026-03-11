using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_281225_COMPANYEVENTREPORT_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EventPublication_CompanyEventReportId",
                table: "EventPublication",
                column: "CompanyEventReportId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipant_CompanyEventReportId",
                table: "EventParticipant",
                column: "CompanyEventReportId");

            migrationBuilder.CreateIndex(
                name: "IX_EventCash_CompanyEventReportId",
                table: "EventCash",
                column: "CompanyEventReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventCash_CompanyEventReport_CompanyEventReportId",
                table: "EventCash",
                column: "CompanyEventReportId",
                principalTable: "CompanyEventReport",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventParticipant_CompanyEventReport_CompanyEventReportId",
                table: "EventParticipant",
                column: "CompanyEventReportId",
                principalTable: "CompanyEventReport",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventPublication_CompanyEventReport_CompanyEventReportId",
                table: "EventPublication",
                column: "CompanyEventReportId",
                principalTable: "CompanyEventReport",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventCash_CompanyEventReport_CompanyEventReportId",
                table: "EventCash");

            migrationBuilder.DropForeignKey(
                name: "FK_EventParticipant_CompanyEventReport_CompanyEventReportId",
                table: "EventParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_EventPublication_CompanyEventReport_CompanyEventReportId",
                table: "EventPublication");

            migrationBuilder.DropIndex(
                name: "IX_EventPublication_CompanyEventReportId",
                table: "EventPublication");

            migrationBuilder.DropIndex(
                name: "IX_EventParticipant_CompanyEventReportId",
                table: "EventParticipant");

            migrationBuilder.DropIndex(
                name: "IX_EventCash_CompanyEventReportId",
                table: "EventCash");
        }
    }
}
