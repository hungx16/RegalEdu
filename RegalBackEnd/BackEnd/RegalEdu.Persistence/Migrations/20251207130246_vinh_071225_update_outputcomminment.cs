using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_071225_update_outputcomminment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OutputCommitment_Company_CompanyId",
                table: "OutputCommitment");

            migrationBuilder.DropForeignKey(
                name: "FK_OutputCommitment_Employees_ConsultantId",
                table: "OutputCommitment");

            migrationBuilder.DropForeignKey(
                name: "FK_OutputCommitment_Region_RegionId",
                table: "OutputCommitment");

            migrationBuilder.DropIndex(
                name: "IX_OutputCommitment_CompanyId",
                table: "OutputCommitment");

            migrationBuilder.DropIndex(
                name: "IX_OutputCommitment_ConsultantId",
                table: "OutputCommitment");

            migrationBuilder.DropIndex(
                name: "IX_OutputCommitment_RegionId",
                table: "OutputCommitment");

            migrationBuilder.DropColumn(
                name: "CommitmentType",
                table: "OutputCommitment");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "OutputCommitment");

            migrationBuilder.DropColumn(
                name: "CompletionDate",
                table: "OutputCommitment");

            migrationBuilder.DropColumn(
                name: "ConsultantId",
                table: "OutputCommitment");

            migrationBuilder.DropColumn(
                name: "RegionId",
                table: "OutputCommitment");

            migrationBuilder.AlterColumn<int>(
                name: "OutputCommitmentStatus",
                table: "OutputCommitment",
                type: "int",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "BeginningLevel",
                table: "OutputCommitment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FinalLevel",
                table: "OutputCommitment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OutputCommitmentInfo",
                table: "OutputCommitment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeginningLevel",
                table: "OutputCommitment");

            migrationBuilder.DropColumn(
                name: "FinalLevel",
                table: "OutputCommitment");

            migrationBuilder.DropColumn(
                name: "OutputCommitmentInfo",
                table: "OutputCommitment");

            migrationBuilder.AlterColumn<string>(
                name: "OutputCommitmentStatus",
                table: "OutputCommitment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "CommitmentType",
                table: "OutputCommitment",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyId",
                table: "OutputCommitment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CompletionDate",
                table: "OutputCommitment",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ConsultantId",
                table: "OutputCommitment",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RegionId",
                table: "OutputCommitment",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_OutputCommitment_CompanyId",
                table: "OutputCommitment",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_OutputCommitment_ConsultantId",
                table: "OutputCommitment",
                column: "ConsultantId");

            migrationBuilder.CreateIndex(
                name: "IX_OutputCommitment_RegionId",
                table: "OutputCommitment",
                column: "RegionId");

            migrationBuilder.AddForeignKey(
                name: "FK_OutputCommitment_Company_CompanyId",
                table: "OutputCommitment",
                column: "CompanyId",
                principalTable: "Company",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OutputCommitment_Employees_ConsultantId",
                table: "OutputCommitment",
                column: "ConsultantId",
                principalTable: "Employees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OutputCommitment_Region_RegionId",
                table: "OutputCommitment",
                column: "RegionId",
                principalTable: "Region",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
