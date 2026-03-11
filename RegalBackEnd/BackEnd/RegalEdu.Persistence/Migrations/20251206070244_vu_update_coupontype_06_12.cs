using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vu_update_coupontype_06_12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "CouponType",
                newName: "CouponTypeStatus");

            migrationBuilder.AlterColumn<int>(
                name: "DueType",
                table: "CouponType",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IssueType",
                table: "CouponIssue",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IssueStatus",
                table: "CouponIssue",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CouponStatus",
                table: "Coupon",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssueStatus",
                table: "CouponIssue");

            migrationBuilder.DropColumn(
                name: "CouponStatus",
                table: "Coupon");

            migrationBuilder.RenameColumn(
                name: "CouponTypeStatus",
                table: "CouponType",
                newName: "Type");

            migrationBuilder.AlterColumn<string>(
                name: "DueType",
                table: "CouponType",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IssueType",
                table: "CouponIssue",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
