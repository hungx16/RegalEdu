using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_041225_capnhat_classSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ClassScheduleId",
                table: "Attachment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_ClassScheduleId",
                table: "Attachment",
                column: "ClassScheduleId",
                unique: true,
                filter: "[ClassScheduleId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_ClassSchedules_ClassScheduleId",
                table: "Attachment",
                column: "ClassScheduleId",
                principalTable: "ClassSchedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_ClassSchedules_ClassScheduleId",
                table: "Attachment");

            migrationBuilder.DropIndex(
                name: "IX_Attachment_ClassScheduleId",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "ClassScheduleId",
                table: "Attachment");
        }
    }
}
