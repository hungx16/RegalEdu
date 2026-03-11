using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_261225_courseLesson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseLesson_LectureType_LectureTypeId",
                table: "CourseLesson");

            migrationBuilder.AlterColumn<Guid>(
                name: "LectureTypeId",
                table: "CourseLesson",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseLesson_LectureType_LectureTypeId",
                table: "CourseLesson",
                column: "LectureTypeId",
                principalTable: "LectureType",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseLesson_LectureType_LectureTypeId",
                table: "CourseLesson");

            migrationBuilder.AlterColumn<Guid>(
                name: "LectureTypeId",
                table: "CourseLesson",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseLesson_LectureType_LectureTypeId",
                table: "CourseLesson",
                column: "LectureTypeId",
                principalTable: "LectureType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
