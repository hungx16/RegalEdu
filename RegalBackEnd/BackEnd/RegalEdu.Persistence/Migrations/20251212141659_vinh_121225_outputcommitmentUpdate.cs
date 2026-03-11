using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RegalEdu.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class vinh_121225_outputcommitmentUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "TotalRegisteredFee",
                table: "OutputCommitment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "TotalRegisteredMonths",
                table: "OutputCommitment",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalRegisteredFee",
                table: "OutputCommitment");

            migrationBuilder.DropColumn(
                name: "TotalRegisteredMonths",
                table: "OutputCommitment");
        }
    }
}
