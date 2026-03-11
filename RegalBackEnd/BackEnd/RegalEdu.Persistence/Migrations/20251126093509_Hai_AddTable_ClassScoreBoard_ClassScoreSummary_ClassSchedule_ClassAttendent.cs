using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Hai_AddTable_ClassScoreBoard_ClassScoreSummary_ClassSchedule_ClassAttendent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CourseLessonId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    DayOfWeek = table.Column<byte>(type: "tinyint", nullable: false),
                    ClassScheduleStatus = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionAttendanceStatus = table.Column<int>(type: "int", nullable: false),
                    Attender = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Plan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeworkPlusName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeworkPlusContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionAttendanceLockStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassSchedules_CourseLesson_CourseLessonId",
                        column: x => x.CourseLessonId,
                        principalTable: "CourseLesson",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassSchedules_Employees_Attender",
                        column: x => x.Attender,
                        principalTable: "Employees",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassSchedules_Teachers_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Teachers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ClassScoreBoard",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ScoreType = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Score = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassScoreBoard", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassScoreBoard_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassScoreBoard_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassScoreBoard_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassScoreSummary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MidtermAvg = table.Column<double>(type: "float", nullable: true),
                    EndTermAvg = table.Column<double>(type: "float", nullable: true),
                    SummaryScore = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassScoreSummary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassScoreSummary_Class_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Class",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassScoreSummary_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassAttendent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClassScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentParticipationStatus = table.Column<int>(type: "int", nullable: false),
                    StudentHomeworkStatus = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Star = table.Column<int>(type: "int", nullable: true),
                    Attachment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HomeworkScore = table.Column<double>(type: "float", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassAttendent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassAttendent_ClassSchedules_ClassScheduleId",
                        column: x => x.ClassScheduleId,
                        principalTable: "ClassSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassAttendent_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassAttendent_ClassScheduleId",
                table: "ClassAttendent",
                column: "ClassScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassAttendent_StudentId",
                table: "ClassAttendent",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_Attender",
                table: "ClassSchedules",
                column: "Attender");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_ClassId",
                table: "ClassSchedules",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_CourseLessonId",
                table: "ClassSchedules",
                column: "CourseLessonId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassSchedules_TeacherId",
                table: "ClassSchedules",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassScoreBoard_CategoryId",
                table: "ClassScoreBoard",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassScoreBoard_ClassId",
                table: "ClassScoreBoard",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassScoreBoard_StudentId",
                table: "ClassScoreBoard",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassScoreSummary_ClassId",
                table: "ClassScoreSummary",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassScoreSummary_StudentId",
                table: "ClassScoreSummary",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassAttendent");

            migrationBuilder.DropTable(
                name: "ClassScoreBoard");

            migrationBuilder.DropTable(
                name: "ClassScoreSummary");

            migrationBuilder.DropTable(
                name: "ClassSchedules");
        }
    }
}
