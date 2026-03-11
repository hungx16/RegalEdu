using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_10122025_AffiliatePartner_update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InterestRate",
                table: "AffiliatePartner",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinancialCompany",
                table: "AffiliatePartner",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LoanBenefits",
                table: "AffiliatePartner",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LoanTerm",
                table: "AffiliatePartner",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<byte>(
                name: "SchoolLevel",
                table: "AffiliatePartner",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteKeys",
                table: "AffiliatePartner",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InterestRate",
                table: "AffiliatePartner");

            migrationBuilder.DropColumn(
                name: "IsFinancialCompany",
                table: "AffiliatePartner");

            migrationBuilder.DropColumn(
                name: "LoanBenefits",
                table: "AffiliatePartner");

            migrationBuilder.DropColumn(
                name: "LoanTerm",
                table: "AffiliatePartner");

            migrationBuilder.DropColumn(
                name: "SchoolLevel",
                table: "AffiliatePartner");

            migrationBuilder.DropColumn(
                name: "WebsiteKeys",
                table: "AffiliatePartner");
        }
    }
}
