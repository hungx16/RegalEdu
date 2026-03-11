using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_101225_update_Student_learningRoadMapId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LearningRoadMapId",
                table: "Students",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_LearningRoadMapId",
                table: "Students",
                column: "LearningRoadMapId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_LearningRoadMap_LearningRoadMapId",
                table: "Students",
                column: "LearningRoadMapId",
                principalTable: "LearningRoadMap",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_LearningRoadMap_LearningRoadMapId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_LearningRoadMapId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "LearningRoadMapId",
                table: "Students");
        }
    }
}
