using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vu_creatPromotionStudent_20_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Promotions_Students_StudentId",
            //    table: "Promotions");

            //migrationBuilder.DropIndex(
            //    name: "IX_Promotions_StudentId",
            //    table: "Promotions");

            //migrationBuilder.DropColumn(
            //    name: "StudentId",
            //    table: "Promotions");

            migrationBuilder.CreateTable(
                name: "PromotionStudent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PromotionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromotionStudent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromotionStudent_Promotions_PromotionId",
                        column: x => x.PromotionId,
                        principalTable: "Promotions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PromotionStudent_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PromotionStudent_PromotionId",
                table: "PromotionStudent",
                column: "PromotionId");

            migrationBuilder.CreateIndex(
                name: "IX_PromotionStudent_StudentId",
                table: "PromotionStudent",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PromotionStudent");

            migrationBuilder.AddColumn<Guid>(
                name: "StudentId",
                table: "Promotions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_StudentId",
                table: "Promotions",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Students_StudentId",
                table: "Promotions",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id");
        }
    }
}
