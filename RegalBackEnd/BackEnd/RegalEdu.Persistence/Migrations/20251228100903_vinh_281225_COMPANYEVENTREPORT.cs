using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_281225_COMPANYEVENTREPORT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyEventId",
                table: "EventPublication",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyEventReportId",
                table: "EventPublication",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyEventId",
                table: "EventParticipant",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyEventReportId",
                table: "EventParticipant",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyEventId",
                table: "EventCash",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyEventReportId",
                table: "EventCash",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyEventReportId",
                table: "Attachment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_CompanyEventReportId",
                table: "Attachment",
                column: "CompanyEventReportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_CompanyEventReport_CompanyEventReportId",
                table: "Attachment",
                column: "CompanyEventReportId",
                principalTable: "CompanyEventReport",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventCash_CompanyEvent_CompanyEventId",
                table: "EventCash",
                column: "CompanyEventId",
                principalTable: "CompanyEvent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventParticipant_CompanyEvent_CompanyEventId",
                table: "EventParticipant",
                column: "CompanyEventId",
                principalTable: "CompanyEvent",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventPublication_CompanyEvent_CompanyEventId",
                table: "EventPublication",
                column: "CompanyEventId",
                principalTable: "CompanyEvent",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_CompanyEventReport_CompanyEventReportId",
                table: "Attachment");

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
                name: "IX_Attachment_CompanyEventReportId",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "CompanyEventReportId",
                table: "EventPublication");

            migrationBuilder.DropColumn(
                name: "CompanyEventReportId",
                table: "EventParticipant");

            migrationBuilder.DropColumn(
                name: "CompanyEventReportId",
                table: "EventCash");

            migrationBuilder.DropColumn(
                name: "CompanyEventReportId",
                table: "Attachment");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyEventId",
                table: "EventPublication",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyEventId",
                table: "EventParticipant",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompanyEventId",
                table: "EventCash",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

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
    }
}
