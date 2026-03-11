using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_10122025_AffiliatePartner_update_en : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "InterestRate",
                table: "AffiliatePartner",
                type: "decimal(18,1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "EnLoanBenefits",
                table: "AffiliatePartner",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnWebsiteKeys",
                table: "AffiliatePartner",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnLoanBenefits",
                table: "AffiliatePartner");

            migrationBuilder.DropColumn(
                name: "EnWebsiteKeys",
                table: "AffiliatePartner");

            migrationBuilder.AlterColumn<int>(
                name: "InterestRate",
                table: "AffiliatePartner",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,1)");
        }
    }
}
