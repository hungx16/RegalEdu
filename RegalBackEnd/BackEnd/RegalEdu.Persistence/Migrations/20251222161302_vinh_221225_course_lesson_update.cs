using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_221225_course_lesson_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CourseLessonHomeworkId",
                table: "Attachment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CourseLessonReferenceId",
                table: "Attachment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_CourseLessonHomeworkId",
                table: "Attachment",
                column: "CourseLessonHomeworkId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachment_CourseLessonReferenceId",
                table: "Attachment",
                column: "CourseLessonReferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_CourseLesson_CourseLessonHomeworkId",
                table: "Attachment",
                column: "CourseLessonHomeworkId",
                principalTable: "CourseLesson",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachment_CourseLesson_CourseLessonReferenceId",
                table: "Attachment",
                column: "CourseLessonReferenceId",
                principalTable: "CourseLesson",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_CourseLesson_CourseLessonHomeworkId",
                table: "Attachment");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachment_CourseLesson_CourseLessonReferenceId",
                table: "Attachment");

            migrationBuilder.DropIndex(
                name: "IX_Attachment_CourseLessonHomeworkId",
                table: "Attachment");

            migrationBuilder.DropIndex(
                name: "IX_Attachment_CourseLessonReferenceId",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "CourseLessonHomeworkId",
                table: "Attachment");

            migrationBuilder.DropColumn(
                name: "CourseLessonReferenceId",
                table: "Attachment");
        }
    }
}
