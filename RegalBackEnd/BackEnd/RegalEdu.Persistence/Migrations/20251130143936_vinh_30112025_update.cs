using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_30112025_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EventPublication_CompanyEventId",
                table: "EventPublication",
                column: "CompanyEventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventParticipant_CompanyEventId",
                table: "EventParticipant",
                column: "CompanyEventId");

            migrationBuilder.CreateIndex(
                name: "IX_EventCash_CompanyEventId",
                table: "EventCash",
                column: "CompanyEventId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventCash_CompanyEvent_CompanyEventId",
                table: "EventCash",
                column: "CompanyEventId",
                principalTable: "CompanyEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventParticipant_CompanyEvent_CompanyEventId",
                table: "EventParticipant",
                column: "CompanyEventId",
                principalTable: "CompanyEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventPublication_CompanyEvent_CompanyEventId",
                table: "EventPublication",
                column: "CompanyEventId",
                principalTable: "CompanyEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventCash_CompanyEvent_CompanyEventId",
                table: "EventCash");

            migrationBuilder.DropForeignKey(
                name: "FK_EventParticipant_CompanyEvent_CompanyEventId",
                table: "EventParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_EventPublication_CompanyEvent_CompanyEventId",
                table: "EventPublication");

            migrationBuilder.DropIndex(
                name: "IX_EventPublication_CompanyEventId",
                table: "EventPublication");

            migrationBuilder.DropIndex(
                name: "IX_EventParticipant_CompanyEventId",
                table: "EventParticipant");

            migrationBuilder.DropIndex(
                name: "IX_EventCash_CompanyEventId",
                table: "EventCash");
        }
    }
}
